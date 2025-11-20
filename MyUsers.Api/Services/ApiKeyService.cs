using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MyUsers.Api.Data;
using MyUsers.Api.Models;

namespace MyUsers.Api.Services;

public interface IApiKeyService
{
    Task<(ApiKey apiKey, string plainKey)> CreateApiKeyAsync(string name, List<string> permissions, int createdByUserId, DateTime? expiresAt = null);
    Task<List<ApiKey>> GetAllApiKeysAsync();
    Task<bool> ValidateApiKeyAsync(string key);
    Task<bool> ToggleApiKeyAsync(int id);
    Task<bool> DeleteApiKeyAsync(int id);
    Task UpdateLastUsedAsync(string keyHash);
}

public class ApiKeyService : IApiKeyService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ApiKeyService> _logger;

    public ApiKeyService(AppDbContext context, ILogger<ApiKeyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(ApiKey apiKey, string plainKey)> CreateApiKeyAsync(
        string name, 
        List<string> permissions, 
        int createdByUserId,
        DateTime? expiresAt = null)
    {
        // Generate secure random API key
        var plainKey = GenerateApiKey();
        var keyHash = HashApiKey(plainKey);

        var apiKey = new ApiKey
        {
            Name = name,
            Key = $"kfc_{plainKey.Substring(0, 8)}...{plainKey.Substring(plainKey.Length - 8)}", // Masked for display
            KeyHash = keyHash,
            Permissions = permissions,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId,
            ExpiresAt = expiresAt
        };

        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync();

        _logger.LogInformation("API key created: {Name} by user {UserId}", name, createdByUserId);

        // Return full key only once during creation
        return (apiKey, $"kfc_{plainKey}");
    }

    public async Task<List<ApiKey>> GetAllApiKeysAsync()
    {
        return await _context.ApiKeys
            .OrderByDescending(k => k.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ValidateApiKeyAsync(string key)
    {
        if (!key.StartsWith("kfc_"))
            return false;

        var keyHash = HashApiKey(key.Substring(4)); // Remove "kfc_" prefix

        var apiKey = await _context.ApiKeys
            .FirstOrDefaultAsync(k => k.KeyHash == keyHash && k.IsActive);

        if (apiKey == null)
            return false;

        // Check expiration
        if (apiKey.ExpiresAt.HasValue && apiKey.ExpiresAt.Value < DateTime.UtcNow)
        {
            _logger.LogWarning("Expired API key attempted: {Name}", apiKey.Name);
            return false;
        }

        // Update last used timestamp (fire and forget)
        _ = UpdateLastUsedAsync(keyHash);

        return true;
    }

    public async Task<bool> ToggleApiKeyAsync(int id)
    {
        var apiKey = await _context.ApiKeys.FindAsync(id);
        if (apiKey == null)
            return false;

        apiKey.IsActive = !apiKey.IsActive;
        await _context.SaveChangesAsync();

        _logger.LogInformation("API key {Name} {Status}", apiKey.Name, apiKey.IsActive ? "enabled" : "disabled");
        return true;
    }

    public async Task<bool> DeleteApiKeyAsync(int id)
    {
        var apiKey = await _context.ApiKeys.FindAsync(id);
        if (apiKey == null)
            return false;

        _context.ApiKeys.Remove(apiKey);
        await _context.SaveChangesAsync();

        _logger.LogInformation("API key deleted: {Name}", apiKey.Name);
        return true;
    }

    public async Task UpdateLastUsedAsync(string keyHash)
    {
        try
        {
            var apiKey = await _context.ApiKeys.FirstOrDefaultAsync(k => k.KeyHash == keyHash);
            if (apiKey != null)
            {
                apiKey.LastUsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update API key last used timestamp");
        }
    }

    private string GenerateApiKey()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "")
            .Substring(0, 40);
    }

    private string HashApiKey(string key)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(key);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
