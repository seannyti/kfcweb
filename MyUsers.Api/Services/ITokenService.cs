using MyUsers.Api.Models;

namespace MyUsers.Api.Services;

/// <summary>
/// Token service interface for JWT operations
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    int ValidateRefreshToken(string refreshToken);
}

