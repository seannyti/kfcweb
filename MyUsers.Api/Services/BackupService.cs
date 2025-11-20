using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyUsers.Api.Data;
using MyUsers.Api.Models;
using System.Text.Json;

namespace MyUsers.Api.Services;

public interface IBackupService
{
    Task<Backup> CreateBackupAsync(string type = "manual");
    Task<List<Backup>> GetBackupsAsync();
    Task<bool> DeleteBackupAsync(int backupId);
    Task<string> GetBackupFilePathAsync(int backupId);
}

public class BackupService : IBackupService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BackupService> _logger;
    private readonly string _backupDirectory;

    public BackupService(AppDbContext context, IConfiguration configuration, ILogger<BackupService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        
        // Use application directory for backups
        _backupDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Backups");
        if (!Directory.Exists(_backupDirectory))
        {
            Directory.CreateDirectory(_backupDirectory);
        }
        
        _logger.LogInformation("Backup directory: {BackupDirectory}", _backupDirectory);
    }

    public async Task<Backup> CreateBackupAsync(string type = "manual")
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss");
        var fileName = $"UsersDb_backup_{timestamp}.json";
        var filePath = Path.Combine(_backupDirectory, fileName);
        
        var backup = new Backup
        {
            Name = fileName,
            FileName = fileName,
            CreatedAt = DateTime.UtcNow,
            Type = type,
            DatabaseName = "KnudsonFamilyConstructionUsersDb",
            Status = "in-progress"
        };

        try
        {
            // Export data as JSON
            var users = await _context.Users.ToListAsync();
            var activityLogs = await _context.ActivityLogs.OrderByDescending(a => a.Timestamp).Take(1000).ToListAsync();
            
            var backupData = new
            {
                ExportDate = DateTime.UtcNow,
                DatabaseName = "KnudsonFamilyConstructionUsersDb",
                Users = users,
                ActivityLogs = activityLogs
            };

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var jsonData = JsonSerializer.Serialize(backupData, jsonOptions);
            await File.WriteAllTextAsync(filePath, jsonData);

            // Get file size
            var fileInfo = new FileInfo(filePath);
            backup.SizeInBytes = fileInfo.Length;
            backup.Status = "completed";

            _logger.LogInformation("Backup created successfully: {FileName}, Size: {Size} bytes", 
                fileName, backup.SizeInBytes);
            
            // Save backup record to database
            _context.Backups.Add(backup);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create backup: {FileName}. Error: {ErrorMessage}", fileName, ex.Message);
            backup.Status = "failed";
            
            // Clean up failed backup file
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception deleteEx)
                {
                    _logger.LogWarning(deleteEx, "Failed to delete incomplete backup file: {FilePath}", filePath);
                }
            }
            
            // Re-throw to let controller handle it
            throw new InvalidOperationException($"Backup creation failed: {ex.Message}", ex);
        }

        return backup;
    }

    public async Task<List<Backup>> GetBackupsAsync()
    {
        return await _context.Backups
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> DeleteBackupAsync(int backupId)
    {
        var backup = await _context.Backups.FindAsync(backupId);
        if (backup == null)
        {
            return false;
        }

        try
        {
            // Delete physical file
            var filePath = Path.Combine(_backupDirectory, backup.FileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Deleted backup file: {FilePath}", filePath);
            }

            // Delete database record
            _context.Backups.Remove(backup);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete backup: {BackupId}", backupId);
            return false;
        }
    }

    public async Task<string> GetBackupFilePathAsync(int backupId)
    {
        var backup = await _context.Backups.FindAsync(backupId);
        if (backup == null)
        {
            throw new FileNotFoundException("Backup not found");
        }

        var filePath = Path.Combine(_backupDirectory, backup.FileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Backup file not found");
        }

        return filePath;
    }
}
