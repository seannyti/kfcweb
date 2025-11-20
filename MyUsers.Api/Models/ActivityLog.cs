namespace MyUsers.Api.Models;

/// <summary>
/// Activity log entity for tracking user and system actions
/// </summary>
public class ActivityLog
{
    public int Id { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public required string Type { get; set; } // user, admin, system, security
    
    public required string Action { get; set; }
    
    public string? UserId { get; set; }
    
    public string? UserName { get; set; }
    
    public string? IpAddress { get; set; }
    
    public string? Details { get; set; }
}

/// <summary>
/// Activity log types
/// </summary>
public static class ActivityLogType
{
    public const string User = "user";
    public const string Admin = "admin";
    public const string System = "system";
    public const string Security = "security";
}
