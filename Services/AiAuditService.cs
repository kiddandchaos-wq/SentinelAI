using SentinelAPI.Models;

namespace SentinelAPI.Services;

public class AiAuditService
{
    public AuditAnalysisResult Analyze(string auditText)
    {
        return new AuditAnalysisResult
        {
            RiskScore = "Medium",

            Summary = "AI analysis completed successfully.",

            Findings = new List<string>
            {
                "Repeated failed login attempts detected.",
                "Administrative role modifications observed."
            },

            Recommendations = new List<string>
            {
                "Enable MFA for administrator accounts.",
                "Review recent role changes."
            }
        };
    }
}
