namespace MyUsers.Api.Models;

/// <summary>
/// User entity representing registered users in the system
/// </summary>
public class User
{
    public int Id { get; set; }
    
    public required string Email { get; set; }
    
    public required string PasswordHash { get; set; }
    
    public required string Name { get; set; }
    
    public required string Role { get; set; } = "User";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginAt { get; set; }
    
    public int FailedLoginAttempts { get; set; } = 0;
    
    public DateTime? LockoutEnd { get; set; }
    
    public string? LastLoginIp { get; set; }
}

