using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyUsers.Api.Data;
using MyUsers.Api.DTOs;
using MyUsers.Api.Models;
using MyUsers.Api.Services;

namespace MyUsers.Api.Controllers;

/// <summary>
/// Admin controller for user management (Admin and SuperAdmin only)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminController> _logger;
    private readonly IActivityLogger _activityLogger;

    public AdminController(AppDbContext context, ILogger<AdminController> logger, IActivityLogger activityLogger)
    {
        _context = context;
        _logger = logger;
        _activityLogger = activityLogger;
    }

    /// <summary>
    /// Get all users (Admin and SuperAdmin only)
    /// </summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                LockoutEnd = u.LockoutEnd,
                LastLoginIp = u.LastLoginIp,
                EmailVerified = u.EmailVerified
            })
            .ToListAsync();

        return Ok(users);
    }

    /// <summary>
    /// Update user role (SuperAdmin only can assign SuperAdmin role)
    /// </summary>
    [HttpPut("users/role")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateUserRole([FromBody] UpdateRoleRequest request)
    {
        // Validate role
        if (!Roles.IsValidRole(request.Role))
        {
            return BadRequest(new { message = "Invalid role. Valid roles: SuperAdmin, Admin, User" });
        }

        // Only SuperAdmin can assign SuperAdmin role
        if (request.Role == Roles.SuperAdmin && !User.IsInRole(Roles.SuperAdmin))
        {
            return StatusCode(StatusCodes.Status403Forbidden, 
                new { message = "Only SuperAdmin can assign SuperAdmin role" });
        }

        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Prevent changing own role
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (user.Id == currentUserId)
        {
            return BadRequest(new { message = "Cannot change your own role" });
        }

        user.Role = request.Role;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} role updated to {Role} by {AdminId}", 
            user.Id, request.Role, currentUserId);

        // Log activity
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Admin";
        await _activityLogger.LogAsync(
            ActivityLogType.Admin,
            $"Updated user role to {request.Role}",
            currentUserId.ToString(),
            adminName,
            null,
            $"Changed {user.Name}'s role to {request.Role}"
        );

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            LastLoginIp = user.LastLoginIp
        });
    }

    /// <summary>
    /// Delete user (Admin and SuperAdmin only, cannot delete SuperAdmin)
    /// </summary>
    [HttpDelete("users/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Prevent deleting own account
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (user.Id == currentUserId)
        {
            return BadRequest(new { message = "Cannot delete your own account" });
        }

        // Only SuperAdmin can delete SuperAdmin accounts
        if (user.Role == Roles.SuperAdmin && !User.IsInRole(Roles.SuperAdmin))
        {
            return StatusCode(StatusCodes.Status403Forbidden, 
                new { message = "Only SuperAdmin can delete SuperAdmin accounts" });
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} deleted by {AdminId}", userId, currentUserId);

        // Log activity
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Admin";
        await _activityLogger.LogAsync(
            ActivityLogType.Admin,
            "Deleted user account",
            currentUserId.ToString(),
            adminName,
            null,
            $"Deleted user: {user.Name} ({user.Email})"
        );

        return Ok(new { message = "User deleted successfully" });
    }

    /// <summary>
    /// Lock user account (Admin and SuperAdmin only)
    /// </summary>
    [HttpPost("users/{userId}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> LockUser(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Prevent locking own account
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (user.Id == currentUserId)
        {
            return BadRequest(new { message = "Cannot lock your own account" });
        }

        // Only SuperAdmin can lock SuperAdmin accounts
        if (user.Role == Roles.SuperAdmin && !User.IsInRole(Roles.SuperAdmin))
        {
            return StatusCode(StatusCodes.Status403Forbidden, 
                new { message = "Only SuperAdmin can lock SuperAdmin accounts" });
        }

        // Lock for 1 year (effectively permanent)
        user.LockoutEnd = DateTime.UtcNow.AddYears(1);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} locked by {AdminId}", userId, currentUserId);

        // Log activity
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Admin";
        await _activityLogger.LogAsync(
            ActivityLogType.Security,
            "Locked user account",
            currentUserId.ToString(),
            adminName,
            null,
            $"Locked account: {user.Name} ({user.Email})"
        );

        return Ok(new { message = "User account locked successfully" });
    }

    /// <summary>
    /// Unlock user account (Admin and SuperAdmin only)
    /// </summary>
    [HttpPost("users/{userId}/unlock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UnlockUser(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Clear lockout
        user.LockoutEnd = null;
        user.FailedLoginAttempts = 0;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} unlocked by admin", userId);

        // Log activity
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Admin";
        await _activityLogger.LogAsync(
            ActivityLogType.Security,
            "Unlocked user account",
            adminName,
            adminName,
            null,
            $"Unlocked account: {user.Name} ({user.Email})"
        );

        return Ok(new { message = "User account unlocked successfully" });
    }

    /// <summary>
    /// Update user name (SuperAdmin only)
    /// </summary>
    [HttpPut("users/{userId}/name")]
    [Authorize(Roles = Roles.SuperAdmin)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> UpdateUserName(int userId, [FromBody] UpdateNameRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Name is required" });
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var oldName = user.Name;
        user.Name = request.Name.Trim();
        await _context.SaveChangesAsync();

        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        _logger.LogInformation("User {UserId} name updated from '{OldName}' to '{NewName}' by {AdminId}", 
            user.Id, oldName, user.Name, currentUserId);

        // Log activity
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "SuperAdmin";
        await _activityLogger.LogAsync(
            ActivityLogType.Admin,
            "Updated user name",
            currentUserId.ToString(),
            adminName,
            null,
            $"Changed {oldName} to {user.Name} ({user.Email})"
        );

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            LastLoginIp = user.LastLoginIp,
            EmailVerified = user.EmailVerified
        });
    }

    /// <summary>
    /// Manually verify user's email (Admin and SuperAdmin only)
    /// </summary>
    [HttpPost("users/{userId}/verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> VerifyUserEmail(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        if (user.EmailVerified)
        {
            return Ok(new { message = "Email already verified" });
        }

        user.EmailVerified = true;
        user.VerificationToken = null;
        user.VerificationTokenExpiry = null;
        await _context.SaveChangesAsync();

        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        _logger.LogInformation("User {UserId} email manually verified by admin {AdminId}", userId, currentUserId);

        // Log activity
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Admin";
        await _activityLogger.LogAsync(
            ActivityLogType.Admin,
            "Manually verified user email",
            currentUserId.ToString(),
            adminName,
            null,
            $"Verified email for: {user.Name} ({user.Email})"
        );

        return Ok(new { message = "Email verified successfully" });
    }

    /// <summary>
    /// Force password reset for a user (Admin and SuperAdmin only)
    /// </summary>
    [HttpPost("users/reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ForcePasswordReset([FromBody] ForcePasswordResetRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Prevent resetting own password through this endpoint
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (user.Id == currentUserId)
        {
            return BadRequest(new { message = "Cannot reset your own password through this endpoint" });
        }

        // Only SuperAdmin can reset SuperAdmin passwords
        if (user.Role == Roles.SuperAdmin && !User.IsInRole(Roles.SuperAdmin))
        {
            return StatusCode(StatusCodes.Status403Forbidden, 
                new { message = "Only SuperAdmin can reset SuperAdmin passwords" });
        }

        // Hash the new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Password reset for user {UserId} by admin {AdminId}", 
            user.Id, currentUserId);

        // Log activity
        var adminName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Admin";
        await _activityLogger.LogAsync(
            ActivityLogType.Admin,
            "Force reset user password",
            currentUserId.ToString(),
            adminName,
            null,
            $"Reset password for: {user.Name} ({user.Email})"
        );

        return Ok(new { message = "Password reset successfully" });
    }

    /// <summary>
    /// Get user count (for cross-API communication)
    /// </summary>
    [HttpGet("users/count")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetUserCount()
    {
        var count = await _context.Users.CountAsync();
        return Ok(new { count });
    }

    /// <summary>
    /// Get system start time (for cross-API communication)
    /// </summary>
    [HttpGet("statistics/start-time")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult GetStartTime()
    {
        var startTime = System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();
        return Ok(new { startTime });
    }

    /// <summary>
    /// Get statistics (Admin and SuperAdmin only)
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetStatistics()
    {
        var totalUsers = await _context.Users.CountAsync();
        var usersByRole = await _context.Users
            .GroupBy(u => u.Role)
            .Select(g => new { Role = g.Key, Count = g.Count() })
            .ToListAsync();

        var recentUsers = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(5)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                LockoutEnd = u.LockoutEnd,
                LastLoginIp = u.LastLoginIp,
                EmailVerified = u.EmailVerified
            })
            .ToListAsync();

        return Ok(new
        {
            totalUsers,
            usersByRole,
            recentUsers
        });
    }

    /// <summary>
    /// Get activity logs (Admin and SuperAdmin only)
    /// </summary>
    [HttpGet("activity-logs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetActivityLogs([FromQuery] string? type = null, [FromQuery] int limit = 100)
    {
        var query = _context.ActivityLogs.AsQueryable();

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(log => log.Type == type);
        }

        var logs = await query
            .OrderByDescending(log => log.Timestamp)
            .Take(limit)
            .ToListAsync();

        return Ok(logs);
    }
}

// DTOs
public class UpdateRoleRequest
{
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class UpdateNameRequest
{
    public string Name { get; set; } = string.Empty;
}

public class ForcePasswordResetRequest
{
    public int UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}
