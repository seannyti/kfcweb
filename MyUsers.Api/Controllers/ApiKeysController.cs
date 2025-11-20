using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyUsers.Api.Models;
using MyUsers.Api.Services;

namespace MyUsers.Api.Controllers;

[ApiController]
[Route("api/api-keys")]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
public class ApiKeysController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;
    private readonly IActivityLogger _activityLogger;
    private readonly ILogger<ApiKeysController> _logger;

    public ApiKeysController(
        IApiKeyService apiKeyService,
        IActivityLogger activityLogger,
        ILogger<ApiKeysController> logger)
    {
        _apiKeyService = apiKeyService;
        _activityLogger = activityLogger;
        _logger = logger;
    }

    /// <summary>
    /// Get all API keys
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ApiKeyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ApiKeyDto>>> GetApiKeys()
    {
        var apiKeys = await _apiKeyService.GetAllApiKeysAsync();
        
        var dtos = apiKeys.Select(k => new ApiKeyDto
        {
            Id = k.Id,
            Name = k.Name,
            Key = k.Key, // Masked version
            Permissions = k.Permissions,
            IsActive = k.IsActive,
            CreatedAt = k.CreatedAt,
            LastUsedAt = k.LastUsedAt,
            ExpiresAt = k.ExpiresAt
        }).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Create a new API key
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateApiKeyResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CreateApiKeyResponse>> CreateApiKey([FromBody] CreateApiKeyRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Admin";

            var (apiKey, plainKey) = await _apiKeyService.CreateApiKeyAsync(
                request.Name,
                request.Permissions,
                userId,
                request.ExpiresAt
            );

            // Log activity
            await _activityLogger.LogAsync(
                ActivityLogType.Admin,
                "Created API key",
                userId.ToString(),
                userName,
                null,
                $"Created API key: {request.Name}"
            );

            return Ok(new CreateApiKeyResponse
            {
                Id = apiKey.Id,
                Name = apiKey.Name,
                Key = plainKey, // Full key shown only once
                Permissions = apiKey.Permissions,
                IsActive = apiKey.IsActive,
                CreatedAt = apiKey.CreatedAt,
                ExpiresAt = apiKey.ExpiresAt,
                Message = "⚠️ Save this key now! You won't be able to see it again."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create API key");
            return StatusCode(500, new { message = "Failed to create API key" });
        }
    }

    /// <summary>
    /// Toggle API key active status
    /// </summary>
    [HttpPost("{id}/toggle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ToggleApiKey(int id)
    {
        var success = await _apiKeyService.ToggleApiKeyAsync(id);
        
        if (!success)
            return NotFound(new { message = "API key not found" });

        // Log activity
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
        var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Admin";
        await _activityLogger.LogAsync(
            ActivityLogType.Admin,
            "Toggled API key status",
            userId,
            userName,
            null,
            $"Toggled API key ID: {id}"
        );

        return Ok(new { message = "API key status updated" });
    }

    /// <summary>
    /// Delete API key
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteApiKey(int id)
    {
        var success = await _apiKeyService.DeleteApiKeyAsync(id);
        
        if (!success)
            return NotFound(new { message = "API key not found" });

        // Log activity
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0";
        var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Admin";
        await _activityLogger.LogAsync(
            ActivityLogType.Admin,
            "Deleted API key",
            userId,
            userName,
            null,
            $"Deleted API key ID: {id}"
        );

        return Ok(new { message = "API key deleted successfully" });
    }
}

public class CreateApiKeyRequest
{
    public string Name { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public DateTime? ExpiresAt { get; set; }
}

public class CreateApiKeyResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ApiKeyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty; // Masked
    public List<string> Permissions { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
