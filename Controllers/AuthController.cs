using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SentinelAPI.Data;
using SentinelAPI.Security;
using SentinelAPI.Services;

namespace SentinelAPI.Controllers;

public record LoginRequest(string? Username, string? Password);

[ApiController]
[Route("api/[controller]")]
public class AuthController(SentinelDbContext db, PasswordHasher hasher, TokenService tokens, AuditLogService audit)
    : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var username = (request.Username ?? "").Trim().ToLowerInvariant();
        var password = request.Password ?? "";

        if (username.Length == 0 || username.Length > 64 || password.Length == 0 || password.Length > 128)
            return Unauthorized(new { error = "Invalid username or password." });

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);

        bool ok;
        if (user is null)
        {
            hasher.BurnTime(password);
            ok = false;
        }
        else
        {
            ok = hasher.Verify(password, user.PasswordHash);
        }

        if (!ok)
        {
            // Known accounts are logged by name; unknown names only as a keyed fingerprint.
            await audit.AppendAsync("auth-api", "auth.failed", JsonSerializer.Serialize(new
            {
                userId = user?.Username ?? "(unknown)",
                attemptHmac = audit.Fingerprint(username),
                reason = "invalid_credentials"
            }), 40);
            return Unauthorized(new { error = "Invalid username or password." });
        }

        var (token, expires) = tokens.Create(user!);
        await audit.AppendAsync("auth-api", "auth.login", JsonSerializer.Serialize(new
        {
            userId = user!.Username,
            role = user.Role
        }), 0);

        return Ok(new { token, role = user.Role, expiresUtc = expires });
    }
}