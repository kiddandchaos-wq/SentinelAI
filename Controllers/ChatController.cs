using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SentinelAPI.Models;
using SentinelAPI.Services;

namespace SentinelAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly RiskScoringService _riskService;
    private readonly AuditLogService _audit;

    public ChatController(RiskScoringService riskService, AuditLogService audit)
    {
        _riskService = riskService;
        _audit = audit;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Analyze(ChatRequest request)
    {
        int risk = _riskService.CalculateRisk(request.Message);
        bool flagged = risk >= 80;

        var response = new ChatResponse
        {
            Response = "Message analyzed.",
            RiskScore = risk,
            IsFlagged = flagged
        };

        // Record a fingerprint of the message, never the message itself.
        var payload = JsonSerializer.Serialize(new
        {
            messageSha256 = AuditLogService.Sha256Hex(request.Message),
            messageLength = request.Message.Length,
            flagged
        });

        await _audit.AppendAsync("chat-api", flagged ? "message.flagged" : "message.analyzed", payload, risk);

        return Ok(response);
    }
}