using Microsoft.EntityFrameworkCore;
using MyUsers.Api.Data;
using MyUsers.Api.DTOs;
using MyUsers.Api.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace MyUsers.Api.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISettingsClient _settingsClient;

    public AuthService(
        AppDbContext context,
        ITokenService tokenService,
        ILogger<AuthService> logger,
        IHttpContextAccessor httpContextAccessor,
        ISettingsClient settingsClient)
    {
        _context = context;
        _tokenService = tokenService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _settingsClient = settingsClient;
    }

    private async Task<SettingsDto> GetSettingsAsync()
    {
        return await _settingsClient.GetSettingsAsync();
    }

    private string? GetClientIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        // Check for forwarded IP first (when behind proxy/load balancer)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    private async Task<bool> IsIpAllowedAsync(string? ipAddress)
    {
        var settings = await GetSettingsAsync();
        
        if (!settings.EnableIpWhitelist || string.IsNullOrEmpty(settings.WhitelistedIps))
        {
            return true; // IP whitelist disabled
        }

        if (string.IsNullOrEmpty(ipAddress))
        {
            return false; // No IP address, deny
        }

        var whitelist = settings.WhitelistedIps
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(ip => ip.Trim());

        return whitelist.Contains(ipAddress);
    }

    private async Task<string> ValidatePasswordRequirementsAsync(string password)
    {
        var settings = await GetSettingsAsync();
        var errors = new List<string>();

        if (password.Length < settings.MinPasswordLength)
        {
            errors.Add($"Password must be at least {settings.MinPasswordLength} characters long");
        }

        if (settings.RequireUppercase && !password.Any(char.IsUpper))
        {
            errors.Add("Password must contain at least one uppercase letter");
        }

        if (settings.RequireNumbers && !password.Any(char.IsDigit))
        {
            errors.Add("Password must contain at least one number");
        }

        if (settings.RequireSpecialChars && !Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
        {
            errors.Add("Password must contain at least one special character");
        }

        return errors.Count > 0 ? string.Join("; ", errors) : string.Empty;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var settings = await GetSettingsAsync();

        // Check if registration is allowed
        if (!settings.AllowRegistration)
        {
            throw new InvalidOperationException("New user registration is currently disabled");
        }

        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Validate password requirements
        var passwordError = await ValidatePasswordRequirementsAsync(request.Password);
        if (!string.IsNullOrEmpty(passwordError))
        {
            throw new InvalidOperationException(passwordError);
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user with default User role
        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            PasswordHash = passwordHash,
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            LastLoginIp = GetClientIpAddress()
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User registered successfully: {Email}", user.Email);

        // Generate tokens
        var token = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            User = MapToUserDto(user)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var settings = await GetSettingsAsync();
        var clientIp = GetClientIpAddress();

        // Check IP whitelist for admin users
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user != null && (user.Role == Roles.Admin || user.Role == Roles.SuperAdmin))
        {
            if (!await IsIpAllowedAsync(clientIp))
            {
                _logger.LogWarning("Admin login attempt from unauthorized IP: {IP}", clientIp);
                throw new UnauthorizedAccessException("Access denied from this IP address");
            }
        }

        // Find user
        if (user == null)
        {
            _logger.LogWarning("Login attempt for non-existent user: {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check if account is locked
        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            var remainingMinutes = Math.Ceiling((user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes);
            _logger.LogWarning("Login attempt for locked account: {Email}", user.Email);
            throw new UnauthorizedAccessException($"Account is locked. Try again in {remainingMinutes} minutes");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            // Increment failed login attempts
            user.FailedLoginAttempts++;
            
            // Lock account if max attempts exceeded
            if (user.FailedLoginAttempts >= settings.MaxLoginAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(30); // Lock for 30 minutes
                _logger.LogWarning("Account locked due to too many failed attempts: {Email}", user.Email);
            }
            
            await _context.SaveChangesAsync();
            
            var attemptsRemaining = settings.MaxLoginAttempts - user.FailedLoginAttempts;
            if (attemptsRemaining > 0)
            {
                throw new UnauthorizedAccessException($"Invalid email or password. {attemptsRemaining} attempts remaining");
            }
            else
            {
                throw new UnauthorizedAccessException("Account locked due to too many failed login attempts");
            }
        }

        // Successful login - reset failed attempts
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;
        user.LastLoginIp = clientIp;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User logged in successfully: {Email} from IP: {IP}", user.Email, clientIp);

        // Generate tokens
        var token = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            User = MapToUserDto(user)
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        // In a production app, you would validate the refresh token against a stored token
        // For this implementation, we'll decode the refresh token to get the user ID
        var userId = _tokenService.ValidateRefreshToken(refreshToken);
        
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        // Generate new tokens
        var newToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        return new AuthResponse
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
            User = MapToUserDto(user)
        };
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}

