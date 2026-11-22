using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelAPI.Models;
using SentinelAPI.Services;

namespace SentinelAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly RiskScoringService _riskService;
    private readonly PolicyService _policy;
    private readonly AuditLogService _audit;
    private readonly IAiService _ai;
    private readonly ILogger<ChatController> _logger;

    public ChatController(RiskScoringService riskService, PolicyService policy,
        AuditLogService audit, IAiService ai, ILogger<ChatController> logger)
    {
        _riskService = riskService;
        _policy = policy;
        _audit = audit;
        _ai = ai;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Analyze(ChatRequest request)
    {
        var message = request.Message ?? "";

        // Identity comes from the signed token, never from the request body.
        var userId = User.FindFirst("sub")?.Value ?? "unknown";
        var role = User.FindFirst("role")?.Value ?? "";

        int risk = _riskService.CalculateRisk(message);
        bool flagged = risk >= 80;
        var msgFingerprint = _audit.Fingerprint(message);

        // 1. Check the question before the AI ever sees it.
        var inbound = _policy.Evaluate(message, role);
        if (!inbound.Allowed)
        {
            await LogAsync("message.blocked", risk, new
            {
                userId,
                role,
                stage = "request",
                decision = "blocked",
                categories = inbound.Detected,
                violations = inbound.Violations,
                messageHmac = msgFingerprint,
                messageLength = message.Length
            });
            return StatusCode(StatusCodes.Status403Forbidden, new ChatResponse
            {
                Response = "Request blocked by policy.",
                RiskScore = risk,
                IsFlagged = flagged,
                IsBlocked = true,
                Violations = inbound.Violations
            });
        }

        // 2. Ask the AI.
        string reply;
        try
        {
            reply = await _ai.AskAsync(message, HttpContext.RequestAborted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI call failed");
            await LogAsync("ai.error", risk, new
            {
                userId,
                role,
                stage = "ai",
                decision = "error",
                messageHmac = msgFingerprint,
                messageLength = message.Length
            });
            return StatusCode(StatusCodes.Status502BadGateway, new ChatResponse
            {
                Response = "The assistant is unavailable right now.",
                RiskScore = risk,
                IsFlagged = flagged
            });
        }

        // 3. Check the reply before the person sees it.
        var outbound = _policy.EvaluateReply(reply, role);
        if (!outbound.Allowed)
        {
            await LogAsync("reply.blocked", 100, new
            {
                userId,
                role,
                stage = "reply",
                decision = "withheld",
                categories = outbound.Detected,
                violations = outbound.Violations,
                messageHmac = msgFingerprint,
                messageLength = message.Length,
                replyHmac = _audit.Fingerprint(reply),
                replyLength = reply.Length
            });
            return StatusCode(StatusCodes.Status403Forbidden, new ChatResponse
            {
                Response = "The answer was withheld because it contained information your role can't access.",
                RiskScore = 100,
                IsFlagged = true,
                IsBlocked = true,
                Violations = outbound.Violations
            });
        }

        // 4. Allowed. Log it (never the text itself) and return the reply.
        await LogAsync(flagged ? "message.flagged" : "message.analyzed", risk, new
        {
            userId,
            role,
            stage = "reply",
            decision = "allowed",
            categories = outbound.Detected,
            messageHmac = msgFingerprint,
            messageLength = message.Length,
            replyHmac = _audit.Fingerprint(reply),
            replyLength = reply.Length
        });

        return Ok(new ChatResponse
        {
            Response = reply,
            RiskScore = risk,
            IsFlagged = flagged,
            IsBlocked = false,
            Violations = Array.Empty<string>()
        });
    }

    private Task LogAsync(string eventType, int risk, object payload) =>
        _audit.AppendAsync("chat-api", eventType, JsonSerializer.Serialize(payload), risk);
}