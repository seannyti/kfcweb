namespace MyUsers.Api.DTOs;

/// <summary>
/// Response model for successful authentication
/// </summary>
public class AuthResponse
{
    public required string Token { get; set; }
    
    public required string RefreshToken { get; set; }
    
    public required UserDto User { get; set; }
}

