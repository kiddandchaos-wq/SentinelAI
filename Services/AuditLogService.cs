using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SentinelAPI.Data;

namespace SentinelAPI.Services;

public record ChainVerificationResult(bool Valid, long EntriesChecked, long? FirstBrokenEntryId, string? Reason);

public class AuditLogService(SentinelDbContext db)
{
    private const string Genesis = "0000000000000000000000000000000000000000000000000000000000000000";
    private const long ChainLockKey = 727001;

    public async Task<AuditLogEntry> AppendAsync(string agentId, string eventType, string payload, int riskScore)
    {
        await using var tx = await db.Database.BeginTransactionAsync();

        // One writer at a time, so two requests can't chain off the same previous entry.
        await db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock({ChainLockKey})");

        var prevHash = await db.AuditLog
            .OrderByDescending(e => e.Id)
            .Select(e => e.Hash)
            .FirstOrDefaultAsync() ?? Genesis;

        var now = DateTime.UtcNow;
        var entry = new AuditLogEntry
        {
            // Postgres keeps microseconds, so truncate before hashing or verification would fail.
            TimestampUtc = new DateTime(now.Ticks - now.Ticks % 10, DateTimeKind.Utc),
            AgentId = agentId,
            EventType = eventType,
            Payload = payload,
            RiskScore = riskScore,
            PrevHash = prevHash
        };
        entry.Hash = ComputeHash(entry);

        db.AuditLog.Add(entry);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return entry;
    }

    public async Task<ChainVerificationResult> VerifyAsync()
    {
        var expectedPrev = Genesis;
        long count = 0;

        await foreach (var e in db.AuditLog.AsNoTracking().OrderBy(e => e.Id).AsAsyncEnumerable())
        {
            count++;
            if (e.PrevHash != expectedPrev)
                return new(false, count, e.Id, "Link to the previous entry is broken (an entry was removed or reordered).");
            if (ComputeHash(e) != e.Hash)
                return new(false, count, e.Id, "Entry contents do not match its stored hash (it was modified).");
            expectedPrev = e.Hash;
        }
        return new(true, count, null, null);
    }

    public Task<List<AuditLogEntry>> GetRecentAsync(int count = 20) =>
        db.AuditLog.AsNoTracking().OrderByDescending(e => e.Id).Take(Math.Clamp(count, 1, 200)).ToListAsync();

    public static string Sha256Hex(string text) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();

    private static string ComputeHash(AuditLogEntry e) =>
        Sha256Hex(JsonSerializer.Serialize(new
        {
            e.PrevHash,
            Timestamp = e.TimestampUtc.ToString("O"),
            e.AgentId,
            e.EventType,
            e.Payload,
            e.RiskScore
        }));
}