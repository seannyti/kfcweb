namespace MyUsers.Api.Models;

public class Backup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Type { get; set; } = string.Empty; // "manual" or "automatic"
    public string DatabaseName { get; set; } = string.Empty;
    public string Status { get; set; } = "completed"; // "completed", "failed", "in-progress"
}
