using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelAPI.Data;
using System.Text.Json;
using SentinelAPI.Services;

namespace SentinelAPI.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly SentinelDbContext _db;
    private readonly AuditLogService _audit;

    public AdminController(
        SentinelDbContext db,
        AuditLogService audit)
    {
        _db = db;
        _audit = audit;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Users
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.Role,
                u.IsActive,
                u.CreatedUtc,
                u.LastLoginUtc
            })
            .ToListAsync();



        return Ok(users);
    }
    [HttpPut("users/{id}/disable")]
    public async Task<IActionResult> DisableUser(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        user.IsActive = false;

        await _db.SaveChangesAsync();

        await _audit.AppendAsync(
            "admin-api",
            "user.disabled",
            JsonSerializer.Serialize(new
            {
                targetUser = user.Username
            }),
            50);



        return Ok(new
        {
            message = $"User {user.Username} disabled."
        });
        }   // closes DisableUser
    [HttpPut("users/{id}/enable")]
    public async Task<IActionResult> EnableUser(int id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        user.IsActive = true;

        await _db.SaveChangesAsync();

        await _audit.AppendAsync(
            "admin-api",
            "user.enabled",
            JsonSerializer.Serialize(new
            {
                targetUser = user.Username
            }),
            50);


        return Ok(new
        {
            message = $"User {user.Username} enabled."
        });
    }
    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> ChangeRole(int id, ChangeRoleRequest request)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        user.Role = request.Role.Trim().ToLowerInvariant();
        
        await _audit.AppendAsync(
           "admin-api",
           "user.role.changed",
           JsonSerializer.Serialize(new
           {
               targetUser = user.Username,
               newRole = user.Role
           }),
           50);



        return Ok(new
        {
            message = $"User {user.Username} role changed to {user.Role}."
        });
    }

}   // closes AdminController
    