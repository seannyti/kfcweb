using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyUsers.Api.Models;
using MyUsers.Api.Services;

namespace MyUsers.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
public class BackupsController : ControllerBase
{
    private readonly IBackupService _backupService;
    private readonly IActivityLogger _activityLogger;
    private readonly ILogger<BackupsController> _logger;

    public BackupsController(
        IBackupService backupService, 
        IActivityLogger activityLogger,
        ILogger<BackupsController> logger)
    {
        _backupService = backupService;
        _activityLogger = activityLogger;
        _logger = logger;
    }

    /// <summary>
    /// Get all backups
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<Backup>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Backup>>> GetBackups()
    {
        var backups = await _backupService.GetBackupsAsync();
        return Ok(backups);
    }

    /// <summary>
    /// Get the latest backup (for cross-API communication)
    /// </summary>
    [HttpGet("latest")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Backup), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Backup>> GetLatestBackup()
    {
        var backups = await _backupService.GetBackupsAsync();
        var latest = backups.OrderByDescending(b => b.CreatedAt).FirstOrDefault();
        
        if (latest == null)
            return NotFound();
            
        return Ok(latest);
    }

    /// <summary>
    /// Create a new backup
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Backup), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Backup>> CreateBackup([FromBody] CreateBackupRequest? request = null)
    {
        try
        {
            var type = request?.Type ?? "manual";
            var backup = await _backupService.CreateBackupAsync(type);

            // Log activity
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Admin";
            await _activityLogger.LogAsync(
                ActivityLogType.Admin,
                "Created database backup",
                userId,
                userName,
                null,
                $"Created {type} backup: {backup.Name}"
            );

            return Ok(backup);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Backup operation failed");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = ex.Message, details = ex.InnerException?.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create backup");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Failed to create backup", error = ex.Message });
        }
    }

    /// <summary>
    /// Download a backup file
    /// </summary>
    [HttpGet("{id}/download")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadBackup(int id)
    {
        try
        {
            var filePath = await _backupService.GetBackupFilePathAsync(id);
            var fileName = Path.GetFileName(filePath);
            
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            
            // Log activity
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Admin";
            await _activityLogger.LogAsync(
                ActivityLogType.Admin,
                "Downloaded database backup",
                userId,
                userName,
                null,
                $"Downloaded backup: {fileName}"
            );

            return File(fileBytes, "application/octet-stream", fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { message = "Backup file not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download backup {BackupId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Failed to download backup" });
        }
    }

    /// <summary>
    /// Delete a backup
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBackup(int id)
    {
        try
        {
            var success = await _backupService.DeleteBackupAsync(id);
            
            if (!success)
            {
                return NotFound(new { message = "Backup not found" });
            }

            // Log activity
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Admin";
            await _activityLogger.LogAsync(
                ActivityLogType.Admin,
                "Deleted database backup",
                userId,
                userName,
                null,
                $"Deleted backup ID: {id}"
            );

            return Ok(new { message = "Backup deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete backup {BackupId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Failed to delete backup" });
        }
    }
}

public class CreateBackupRequest
{
    public string Type { get; set; } = "manual";
}
