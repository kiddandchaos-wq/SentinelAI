using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SentinelAPI.Data;
using SentinelAPI.Security;
using SentinelAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<SentinelDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<AuditLogService>();

builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<TokenService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.MapInboundClaims = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = TokenService.Issuer,
            ValidateAudience = true,
            ValidAudience = TokenService.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = "sub",
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization();

// At most 10 login attempts per minute from one address.
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.AddPolicy("login", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});

builder.Services.AddOpenApi();

builder.Services.AddSingleton<RiskScoringService>();
builder.Services.AddSingleton<PolicyService>();

builder.Services.AddHttpClient<AnthropicAiService>(c => c.Timeout = TimeSpan.FromSeconds(30));
if (!string.IsNullOrWhiteSpace(builder.Configuration["Anthropic:ApiKey"]))
    builder.Services.AddScoped<IAiService>(sp => sp.GetRequiredService<AnthropicAiService>());
else
    builder.Services.AddSingleton<IAiService, MockAiService>();

var app = builder.Build();

var jwtKey = app.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
    throw new InvalidOperationException(
        "Jwt:Key is missing or shorter than 32 characters. Set it with: dotnet user-secrets set \"Jwt:Key\" \"<random value>\"");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await DemoUserSeeder.SeedAsync(app.Services);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();