namespace SentinelAPI.Data;

public class AuditLogEntry
{
    public long Id { get; set; }
    public DateTime TimestampUtc { get; set; }
    public string AgentId { get; set; } = "";
    public string EventType { get; set; } = "";
    public string Payload { get; set; } = "";
    public int RiskScore { get; set; }
    public string PrevHash { get; set; } = "";
    public string Hash { get; set; } = "";
}