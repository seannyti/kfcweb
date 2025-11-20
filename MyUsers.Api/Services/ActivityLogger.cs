using MyUsers.Api.Data;
using MyUsers.Api.Models;

namespace MyUsers.Api.Services;

/// <summary>
/// Service for logging activities
/// </summary>
public interface IActivityLogger
{
    Task LogAsync(string type, string action, string? userId = null, string? userName = null, string? ipAddress = null, string? details = null);
}

public class ActivityLogger : IActivityLogger
{
    private readonly AppDbContext _context;
    private readonly ILogger<ActivityLogger> _logger;

    public ActivityLogger(AppDbContext context, ILogger<ActivityLogger> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogAsync(string type, string action, string? userId = null, string? userName = null, string? ipAddress = null, string? details = null)
    {
        try
        {
            var log = new ActivityLog
            {
                Type = type,
                Action = action,
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log activity: {Action}", action);
        }
    }
}
