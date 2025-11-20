using System.Text.Json;

namespace MyUsers.Api.Services;

/// <summary>
/// Client for communicating with MySettings.Api
/// </summary>
public interface ISettingsClient
{
    Task<SettingsDto> GetSettingsAsync();
}

public class SettingsClient : ISettingsClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SettingsClient> _logger;
    private readonly IConfiguration _configuration;

    public SettingsClient(HttpClient httpClient, ILogger<SettingsClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        
        // Set base address for MySettings.Api
        var settingsApiUrl = configuration["SettingsApiUrl"] ?? "http://localhost:5001";
        _httpClient.BaseAddress = new Uri(settingsApiUrl);
    }

    public async Task<SettingsDto> GetSettingsAsync()
    {
        try
        {
            // Use public endpoint that doesn't require authentication
            var response = await _httpClient.GetAsync("/api/settings/all");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var settings = JsonSerializer.Deserialize<SettingsDto>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            return settings ?? new SettingsDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch settings from MySettings.Api");
            // Return default settings if API call fails
            return new SettingsDto();
        }
    }
}

/// <summary>
/// DTO for settings from MySettings.Api
/// </summary>
public class SettingsDto
{
    public bool AllowRegistration { get; set; } = true;
    public int MaxLoginAttempts { get; set; } = 5;
    public bool EnableIpWhitelist { get; set; } = false;
    public string WhitelistedIps { get; set; } = string.Empty;
    public int MinPasswordLength { get; set; } = 8;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireNumbers { get; set; } = true;
    public bool RequireSpecialChars { get; set; } = true;
    public bool MaintenanceMode { get; set; } = false;
    public bool EnableApiAccess { get; set; } = true;
}
