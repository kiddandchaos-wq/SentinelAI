using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SentinelAPI.Data;

namespace SentinelAPI.Security;

public class TokenService(IConfiguration config)
{
    public const string Issuer = "SentinelAPI";
    public const string Audience = "SentinelAPI";
    public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(30);

    public (string Token, DateTime ExpiresUtc) Create(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var now = DateTime.UtcNow;
        var expires = now.Add(Lifetime);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            IssuedAt = now,
            Expires = expires,
            Claims = new Dictionary<string, object>
            {
                ["sub"] = user.Username,
                ["role"] = user.Role
            },
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return (new JsonWebTokenHandler().CreateToken(descriptor), expires);
    }
}