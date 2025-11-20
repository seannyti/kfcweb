namespace MyUsers.Api.Models;

public class ApiKey
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty; // Store hashed version for security
    public List<string> Permissions { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
