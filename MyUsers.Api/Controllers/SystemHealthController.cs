using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyUsers.Api.Models;
using MyUsers.Api.Services;

namespace MyUsers.Api.Controllers;

[ApiController]
[Route("api/system-health")]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
public class SystemHealthController : ControllerBase
{
    private readonly ISystemHealthService _healthService;
    private readonly ILogger<SystemHealthController> _logger;

    public SystemHealthController(
        ISystemHealthService healthService,
        ILogger<SystemHealthController> logger)
    {
        _healthService = healthService;
        _logger = logger;
    }

    /// <summary>
    /// Get system health status
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SystemHealthDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SystemHealthDto>> GetSystemHealth()
    {
        try
        {
            var health = await _healthService.GetSystemHealthAsync();
            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get system health");
            return StatusCode(500, new { message = "Failed to retrieve system health" });
        }
    }
}
