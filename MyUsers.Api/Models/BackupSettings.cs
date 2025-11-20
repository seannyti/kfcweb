namespace MyUsers.Api.Models;

public class BackupSettings
{
    public int Id { get; set; }
    public bool AutoBackupEnabled { get; set; }
    public string Frequency { get; set; } = "daily"; // daily, weekly, monthly
    public TimeSpan ScheduledTime { get; set; } = TimeSpan.FromHours(2); // Default 2:00 AM
    public DateTime? LastBackupDate { get; set; }
    public DateTime UpdatedAt { get; set; }
}
