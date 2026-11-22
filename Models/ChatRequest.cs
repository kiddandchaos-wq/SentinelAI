namespace SentinelAPI.Models;

public class ChatRequest
{
    public string Message { get; set; } = "";
    public string UserId { get; set; } = "anonymous";
    public string Role { get; set; } = "employee";
}