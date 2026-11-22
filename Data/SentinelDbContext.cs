using Microsoft.EntityFrameworkCore;

namespace SentinelAPI.Data;

public class SentinelDbContext(DbContextOptions<SentinelDbContext> options) : DbContext(options)
{
    public DbSet<AuditLogEntry> AuditLog => Set<AuditLogEntry>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>().HasIndex(u => u.Username).IsUnique();
    }
}