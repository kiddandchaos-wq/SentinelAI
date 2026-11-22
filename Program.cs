using Microsoft.EntityFrameworkCore;
using SentinelAPI.Data;
using SentinelAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<SentinelDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<AuditLogService>();

builder.Services.AddOpenApi();

builder.Services.AddSingleton<RiskScoringService>();
builder.Services.AddSingleton<PolicyService>();

builder.Services.AddHttpClient<AnthropicAiService>(c => c.Timeout = TimeSpan.FromSeconds(30));
if (!string.IsNullOrWhiteSpace(builder.Configuration["Anthropic:ApiKey"]))
    builder.Services.AddScoped<IAiService>(sp => sp.GetRequiredService<AnthropicAiService>());
else
    builder.Services.AddSingleton<IAiService, MockAiService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllers();

app.Run();