namespace SentinelAPI.Models;

public class ChatResponse
{
    public string Response { get; set; } = "";
    public int RiskScore { get; set; }
    public bool IsFlagged { get; set; }
}