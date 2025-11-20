using MyUsers.Api.DTOs;

namespace MyUsers.Api.Services;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
}

