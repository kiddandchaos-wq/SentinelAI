using Microsoft.EntityFrameworkCore;
using SentinelAPI.Security;

namespace SentinelAPI.Data;

// Development only: creates four demo users the first time the app starts.
public static class DemoUserSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var log = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DemoUserSeeder");
        try
        {
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var password = config["Seed:DemoPassword"];
            if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
            {
                log.LogWarning("Seed:DemoPassword is missing or shorter than 12 characters. No demo users were created.");
                return;
            }

            var db = scope.ServiceProvider.GetRequiredService<SentinelDbContext>();
            if (await db.Users.AnyAsync()) return;

            var hasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();
            foreach (var (name, role) in new[] { ("alice", "employee"), ("bob", "hr"), ("carol", "finance"), ("dave", "it") })
            {
                db.Users.Add(new AppUser { Username = name, Role = role, PasswordHash = hasher.Hash(password) });
            }

            await db.SaveChangesAsync();
            log.LogInformation("Created demo users: alice (employee), bob (hr), carol (finance), dave (it).");
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Could not create demo users. Was the AddUsers migration applied and the grants run?");
        }
    }
}