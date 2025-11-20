using System.ComponentModel.DataAnnotations;

namespace MyUsers.Api.DTOs;

/// <summary>
/// Request model for token refresh
/// </summary>
public class RefreshTokenRequest
{
    [Required]
    public required string RefreshToken { get; set; }
}

