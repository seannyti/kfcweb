using System.ComponentModel.DataAnnotations;

namespace MyUsers.Api.DTOs;

/// <summary>
/// Request model for user registration
/// </summary>
public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
    
    [Required]
    [MinLength(2)]
    public required string Name { get; set; }
}

