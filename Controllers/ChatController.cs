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
    private readonly PolicyService _policy;
    private readonly AuditLogService _audit;

    public ChatController(RiskScoringService riskService, PolicyService policy, AuditLogService audit)
    {
        _riskService = riskService;
        _policy = policy;
        _audit = audit;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Analyze(ChatRequest request)
    {
        var message = request.Message ?? "";
        var userId = Clip(request.UserId, "anonymous");
        var role = Clip(request.Role, "employee");

        int risk = _riskService.CalculateRisk(message);
        var decision = _policy.Evaluate(message, role);
        bool flagged = risk >= 80;
        bool blocked = !decision.Allowed;

        var eventType = blocked ? "message.blocked"
                      : flagged ? "message.flagged"
                      : "message.analyzed";

        // Log who asked and what happened, but never the message itself.
        var payload = JsonSerializer.Serialize(new
        {
            userId,
            role,
            categories = decision.Detected,
            violations = decision.Violations,
            decision = blocked ? "blocked" : "allowed",
            messageHmac = _audit.Fingerprint(message),
            messageLength = message.Length
        });

        await _audit.AppendAsync("chat-api", eventType, payload, risk);

        var response = new ChatResponse
        {
            Response = blocked ? "Request blocked by policy." : "Message analyzed.",
            RiskScore = risk,
            IsFlagged = flagged,
            IsBlocked = blocked,
            Violations = decision.Violations
        };

        if (blocked)
            return StatusCode(StatusCodes.Status403Forbidden, response);

        return Ok(response);
    }

    private static string Clip(string? value, string fallback)
    {
        value = value?.Trim();
        if (string.IsNullOrEmpty(value)) return fallback;
        return value.Length > 64 ? value[..64] : value;
    }
}