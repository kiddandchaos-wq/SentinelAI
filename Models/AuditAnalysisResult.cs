namespace SentinelAPI.Models;

public class AuditAnalysisResult
{
    public string RiskScore { get; set; } = "";

    public string Summary { get; set; } = "";

    public List<string> Findings { get; set; } = new();

    public List<string> Recommendations { get; set; } = new();
}