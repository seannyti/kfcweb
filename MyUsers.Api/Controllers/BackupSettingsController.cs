using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyUsers.Api.Data;
using MyUsers.Api.Models;
using MyUsers.Api.Services;

namespace MyUsers.Api.Controllers;

[ApiController]
[Route("api/backup-settings")]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
public class BackupSettingsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IBackupScheduler _backupScheduler;
    private readonly ILogger<BackupSettingsController> _logger;

    public BackupSettingsController(
        AppDbContext context,
        IBackupScheduler backupScheduler,
        ILogger<BackupSettingsController> logger)
    {
        _context = context;
        _backupScheduler = backupScheduler;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<BackupSettingsDto>> GetSettings()
    {
        var settings = await _context.BackupSettings.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            // Return default settings
            return Ok(new BackupSettingsDto
            {
                AutoBackupEnabled = false,
                Frequency = "daily",
                ScheduledTime = "02:00"
            });
        }

        return Ok(new BackupSettingsDto
        {
            AutoBackupEnabled = settings.AutoBackupEnabled,
            Frequency = settings.Frequency,
            ScheduledTime = settings.ScheduledTime.ToString(@"hh\:mm"),
            LastBackupDate = settings.LastBackupDate
        });
    }

    [HttpPost]
    public async Task<ActionResult> UpdateSettings([FromBody] BackupSettingsDto dto)
    {
        try
        {
            if (!TimeSpan.TryParse(dto.ScheduledTime, out var scheduledTime))
            {
                return BadRequest(new { message = "Invalid time format. Use HH:mm" });
            }

            await _backupScheduler.UpdateScheduleAsync(
                dto.AutoBackupEnabled,
                dto.Frequency,
                scheduledTime
            );

            _logger.LogInformation("Backup settings updated: Enabled={Enabled}, Frequency={Frequency}, Time={Time}",
                dto.AutoBackupEnabled, dto.Frequency, scheduledTime);

            return Ok(new { message = "Backup settings updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update backup settings");
            return StatusCode(500, new { message = "Failed to update backup settings" });
        }
    }
}

public class BackupSettingsDto
{
    public bool AutoBackupEnabled { get; set; }
    public string Frequency { get; set; } = "daily";
    public string ScheduledTime { get; set; } = "02:00";
    public DateTime? LastBackupDate { get; set; }
}
