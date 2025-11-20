namespace MyUsers.Api.DTOs;

/// <summary>
/// User data transfer object (without sensitive information)
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    
    public required string Email { get; set; }
    
    public required string Name { get; set; }
    
    public required string Role { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? LockoutEnd { get; set; }
    
    public string? LastLoginIp { get; set; }
}

