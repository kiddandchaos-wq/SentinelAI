using Microsoft.AspNetCore.Mvc;
using SentinelAPI.Security;
using SentinelAPI.Services;

namespace SentinelAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequireApiKey]
public class AuditController(AuditLogService audit) : ControllerBase
{
    [HttpGet("verify")]
    public async Task<IActionResult> Verify() => Ok(await audit.VerifyAsync());

    [HttpGet("recent")]
    public async Task<IActionResult> Recent() => Ok(await audit.GetRecentAsync());
}