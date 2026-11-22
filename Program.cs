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
var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
 
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();
 
app.Run();