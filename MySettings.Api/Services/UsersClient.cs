using System.Text.Json;

namespace MySettings.Api.Services;

/// <summary>
/// Client for communicating with MyUsers.Api
/// </summary>
public interface IUsersClient
{
    Task<int> GetUserCountAsync();
    Task<DateTime?> GetLastBackupTimeAsync();
    Task<DateTime> GetSystemStartTimeAsync();
}

public class UsersClient : IUsersClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersClient> _logger;
    private readonly IConfiguration _configuration;

    public UsersClient(HttpClient httpClient, ILogger<UsersClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        
        // Set base address for MyUsers.Api
        var usersApiUrl = configuration["UsersApiUrl"] ?? "http://localhost:5000";
        _httpClient.BaseAddress = new Uri(usersApiUrl);
    }

    public async Task<int> GetUserCountAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/admin/users/count");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<UserCountDto>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch user count from MyUsers.Api");
            return 0;
        }
    }

    public async Task<DateTime?> GetLastBackupTimeAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/backups/latest");
            if (!response.IsSuccessStatusCode)
                return null;
            
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BackupDto>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            return result?.CreatedAt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch last backup time from MyUsers.Api");
            return null;
        }
    }

    public async Task<DateTime> GetSystemStartTimeAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/admin/statistics/start-time");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SystemStartTimeDto>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            return result?.StartTime ?? DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch system start time from MyUsers.Api");
            return DateTime.UtcNow;
        }
    }
}

public class UserCountDto
{
    public int Count { get; set; }
}

public class BackupDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SystemStartTimeDto
{
    public DateTime StartTime { get; set; }
}
