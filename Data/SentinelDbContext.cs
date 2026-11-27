using Microsoft.EntityFrameworkCore;
using SentinelAPI.Models;

namespace SentinelAPI.Data;

public class SentinelDbContext(DbContextOptions<SentinelDbContext> options) : DbContext(options)
{
    public DbSet<AuditLogEntry> AuditLog => Set<AuditLogEntry>();

    public DbSet<AppUser> Users => Set<AppUser>();

    public DbSet<KnowledgeDocument> KnowledgeDocuments => Set<KnowledgeDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
    public DbSet<KnowledgeChunk> KnowledgeChunks => Set<KnowledgeChunk>();
}