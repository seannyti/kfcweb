using Hangfire;
using MyUsers.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace MyUsers.Api.Services;

public interface IBackupScheduler
{
    Task ScheduleBackupsAsync();
    Task UpdateScheduleAsync(bool enabled, string frequency, TimeSpan scheduledTime);
}

public class BackupScheduler : IBackupScheduler
{
    private readonly IBackupService _backupService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackupScheduler> _logger;
    private const string RecurringJobId = "automatic-database-backup";

    public BackupScheduler(
        IBackupService backupService,
        IServiceProvider serviceProvider,
        ILogger<BackupScheduler> logger)
    {
        _backupService = backupService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ScheduleBackupsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var settings = await context.BackupSettings.FirstOrDefaultAsync();
        if (settings == null || !settings.AutoBackupEnabled)
        {
            RecurringJob.RemoveIfExists(RecurringJobId);
            _logger.LogInformation("Automatic backups disabled");
            return;
        }

        var cronExpression = GetCronExpression(settings.Frequency, settings.ScheduledTime);
        RecurringJob.AddOrUpdate(
            RecurringJobId,
            () => CreateScheduledBackup(),
            cronExpression,
            new RecurringJobOptions { TimeZone = TimeZoneInfo.Local }
        );

        _logger.LogInformation("Scheduled automatic backups: {Frequency} at {Time}", 
            settings.Frequency, settings.ScheduledTime);
    }

    public async Task UpdateScheduleAsync(bool enabled, string frequency, TimeSpan scheduledTime)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var settings = await context.BackupSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new Models.BackupSettings();
            context.BackupSettings.Add(settings);
        }

        settings.AutoBackupEnabled = enabled;
        settings.Frequency = frequency;
        settings.ScheduledTime = scheduledTime;
        settings.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        await ScheduleBackupsAsync();
    }

    public async Task CreateScheduledBackup()
    {
        try
        {
            _logger.LogInformation("Starting scheduled backup...");
            await _backupService.CreateBackupAsync("automatic");
            
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var settings = await context.BackupSettings.FirstOrDefaultAsync();
            if (settings != null)
            {
                settings.LastBackupDate = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
            
            _logger.LogInformation("Scheduled backup completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scheduled backup failed");
        }
    }

    private string GetCronExpression(string frequency, TimeSpan scheduledTime)
    {
        var hour = scheduledTime.Hours;
        var minute = scheduledTime.Minutes;

        return frequency.ToLower() switch
        {
            "daily" => $"{minute} {hour} * * *",           // Every day at specified time
            "weekly" => $"{minute} {hour} * * 0",          // Every Sunday at specified time
            "monthly" => $"{minute} {hour} 1 * *",         // 1st of each month at specified time
            _ => $"{minute} {hour} * * *"                  // Default to daily
        };
    }
}
