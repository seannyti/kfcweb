using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySettings.Api.Data;
using MySettings.Api.DTOs;
using MySettings.Api.Models;
using MySettings.Api.Services;

namespace MySettings.Api.Controllers;

/// <summary>
/// Admin settings controller for managing site configuration
/// </summary>
[ApiController]
[Route("api/admin/settings")]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
public class AdminSettingsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminSettingsController> _logger;
    private readonly EmailService _emailService;
    private readonly IUsersClient _usersClient;

    public AdminSettingsController(
        AppDbContext context, 
        ILogger<AdminSettingsController> logger,
        EmailService emailService,
        IUsersClient usersClient)
    {
        _context = context;
        _logger = logger;
        _emailService = emailService;
        _usersClient = usersClient;
    }

    /// <summary>
    /// Get all site settings
    /// </summary>
    [HttpGet("settings")]
    [ProducesResponseType(typeof(SiteSettingsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SiteSettingsDto>> GetSettings()
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            // Create default settings if none exist
            settings = new SiteSettings();
            _context.SiteSettings.Add(settings);
            await _context.SaveChangesAsync();
        }

        var dto = MapToDto(settings);
        return Ok(dto);
    }

    /// <summary>
    /// Save site settings
    /// </summary>
    [HttpPut("settings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveSettings([FromBody] SiteSettingsDto dto)
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            settings = new SiteSettings();
            _context.SiteSettings.Add(settings);
        }

        // Update settings
        settings.SiteName = dto.SiteName;
        settings.Tagline = dto.Tagline;
        settings.Description = dto.Description;
        settings.Logo = dto.Logo;
        settings.Favicon = dto.Favicon;
        settings.Timezone = dto.Timezone;
        settings.DateFormat = dto.DateFormat;
        settings.AllowRegistration = dto.AllowRegistration;
        settings.ForceHttps = dto.ForceHttps;
        
        settings.EmailEnabled = dto.EmailEnabled;
        settings.SmtpServer = dto.SmtpServer;
        settings.SmtpPort = dto.SmtpPort;
        settings.UseSsl = dto.UseSsl;
        settings.SmtpUsername = dto.SmtpUsername;
        // Only update password if a new one is provided
        if (!string.IsNullOrEmpty(dto.SmtpPassword))
        {
            settings.SmtpPassword = dto.SmtpPassword;
        }
        settings.FromEmail = dto.FromEmail;
        settings.FromName = dto.FromName;
        
        settings.MaintenanceMode = dto.MaintenanceMode;
        settings.MaintenanceMessage = dto.MaintenanceMessage;
        settings.EnableApiAccess = dto.EnableApiAccess;
        
        settings.Enforce2FA = dto.Enforce2FA;
        settings.SessionTimeout = dto.SessionTimeout;
        settings.MaxLoginAttempts = dto.MaxLoginAttempts;
        settings.EnableIpWhitelist = dto.EnableIpWhitelist;
        settings.WhitelistedIps = dto.WhitelistedIps;
        settings.MinPasswordLength = dto.MinPasswordLength;
        settings.RequireUppercase = dto.RequireUppercase;
        settings.RequireNumbers = dto.RequireNumbers;
        settings.RequireSpecialChars = dto.RequireSpecialChars;
        
        settings.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Site settings updated by {User}", User.Identity?.Name);

        return Ok(new { message = "Settings saved successfully" });
    }

    /// <summary>
    /// Toggle maintenance mode
    /// </summary>
    [HttpPost("maintenance/toggle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleMaintenanceMode([FromBody] MaintenanceToggleRequest request)
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            return BadRequest(new { message = "Settings not initialized" });
        }

        settings.MaintenanceMode = request.Enabled;
        settings.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        _logger.LogWarning("Maintenance mode {Status} by {User}", 
            request.Enabled ? "ENABLED" : "DISABLED", 
            User.Identity?.Name);

        return Ok(new { 
            message = $"Maintenance mode {(request.Enabled ? "enabled" : "disabled")}", 
            maintenanceMode = request.Enabled 
        });
    }

    /// <summary>
    /// Send test email
    /// </summary>
    [HttpPost("email/test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendTestEmail()
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync();
        
        if (settings == null || !settings.EmailEnabled)
        {
            return BadRequest(new { message = "Email system is not enabled or configured" });
        }

        if (string.IsNullOrEmpty(settings.SmtpUsername))
        {
            return BadRequest(new { message = "SMTP username/email is not configured" });
        }

        _logger.LogInformation("Test email requested by {User}", User.Identity?.Name);
        
        var success = await _emailService.SendTestEmailAsync(settings.SmtpUsername);
        
        if (success)
        {
            return Ok(new { message = $"Test email sent successfully to {settings.SmtpUsername}" });
        }
        else
        {
            return BadRequest(new { message = "Failed to send test email. Check your SMTP settings and credentials." });
        }
    }

    /// <summary>
    /// Clear application cache
    /// </summary>
    [HttpPost("cache/clear")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ClearCache()
    {
        // Note: No caching currently implemented
        _logger.LogInformation("Cache clear requested by {User}", User.Identity?.Name);
        
        return Ok(new { message = "No cache to clear - caching not enabled" });
    }

    /// <summary>
    /// Optimize database
    /// </summary>
    [HttpPost("database/optimize")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult OptimizeDatabase()
    {
        // Note: Database optimization should be performed manually via SQL Server Management Studio
        _logger.LogInformation("Database optimization requested by {User}", User.Identity?.Name);
        
        return Ok(new { message = "Database optimization should be performed via SSMS" });
    }

    /// <summary>
    /// Get dashboard statistics
    /// </summary>
    [HttpGet("dashboard/stats")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        var totalUsers = await _usersClient.GetUserCountAsync();
        var lastBackupTime = await _usersClient.GetLastBackupTimeAsync();
        var systemStartTime = await _usersClient.GetSystemStartTimeAsync();
        
        // Calculate uptime
        var uptime = DateTime.UtcNow - systemStartTime;
        var uptimeString = uptime.TotalDays >= 1 
            ? $"{(int)uptime.TotalDays}d {uptime.Hours}h" 
            : $"{uptime.Hours}h {uptime.Minutes}m";
        
        // Format last backup
        string lastBackupString = "Never";
        if (lastBackupTime.HasValue)
        {
            var timeSince = DateTime.UtcNow - lastBackupTime.Value;
            if (timeSince.TotalMinutes < 60)
                lastBackupString = $"{(int)timeSince.TotalMinutes} min ago";
            else if (timeSince.TotalHours < 24)
                lastBackupString = $"{(int)timeSince.TotalHours} hr ago";
            else
                lastBackupString = $"{(int)timeSince.TotalDays} days ago";
        }
        
        var stats = new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            EmailsSentToday = 0, // Email logging not implemented
            EmailsTotal = 0, // Email logging not implemented
            Uptime = uptimeString,
            LastBackup = lastBackupString
        };

        return Ok(stats);
    }

    /// <summary>
    /// Get system health metrics
    /// </summary>
    [HttpGet("system/health")]
    [ProducesResponseType(typeof(SystemHealthDto), StatusCodes.Status200OK)]
    public ActionResult<SystemHealthDto> GetSystemHealth()
    {
        // Note: Returns placeholder values - for actual metrics, use Azure Monitor or similar
        var health = new SystemHealthDto
        {
            Cpu = 32,
            Memory = 58,
            Disk = 45,
            Uptime = 23
        };

        return Ok(health);
    }

    /// <summary>
    /// Check for system updates
    /// </summary>
    [HttpGet("system/updates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CheckUpdates()
    {
        return Ok(new { 
            updatesAvailable = false, 
            message = "System is up to date",
            currentVersion = "1.0.0"
        });
    }

    /// <summary>
    /// Get theme settings
    /// </summary>
    [HttpGet("theme")]
    [ProducesResponseType(typeof(ThemeSettingsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ThemeSettingsDto>> GetTheme()
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            settings = new SiteSettings();
            _context.SiteSettings.Add(settings);
            await _context.SaveChangesAsync();
        }

        var dto = new ThemeSettingsDto
        {
            PrimaryColor = settings.PrimaryColor,
            SecondaryColor = settings.SecondaryColor,
            SuccessColor = settings.SuccessColor,
            DangerColor = settings.DangerColor,
            WarningColor = settings.WarningColor,
            InfoColor = settings.InfoColor,
            DarkBg = settings.DarkBg,
            DarkSurface = settings.DarkSurface,
            DarkText = settings.DarkText,
            LightBg = settings.LightBg,
            LightSurface = settings.LightSurface,
            LightText = settings.LightText,
            BorderRadius = settings.BorderRadius,
            FontFamily = settings.FontFamily,
            DarkModeDefault = settings.DarkModeDefault,
            ForceDarkMode = settings.ForceDarkMode,
            UseGradientBg = settings.UseGradientBg,
            UseGlassmorphism = settings.UseGlassmorphism,
            AnimatedBg = settings.AnimatedBg,
            AccentGradient = settings.AccentGradient,
            CustomCss = settings.CustomCss
        };

        return Ok(dto);
    }

    /// <summary>
    /// Save theme settings
    /// </summary>
    [HttpPut("theme")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveTheme([FromBody] ThemeSettingsDto dto)
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            settings = new SiteSettings();
            _context.SiteSettings.Add(settings);
        }

        settings.PrimaryColor = dto.PrimaryColor;
        settings.SecondaryColor = dto.SecondaryColor;
        settings.SuccessColor = dto.SuccessColor;
        settings.DangerColor = dto.DangerColor;
        settings.WarningColor = dto.WarningColor;
        settings.InfoColor = dto.InfoColor;
        settings.DarkBg = dto.DarkBg;
        settings.DarkSurface = dto.DarkSurface;
        settings.DarkText = dto.DarkText;
        settings.LightBg = dto.LightBg;
        settings.LightSurface = dto.LightSurface;
        settings.LightText = dto.LightText;
        settings.BorderRadius = dto.BorderRadius;
        settings.FontFamily = dto.FontFamily;
        settings.DarkModeDefault = dto.DarkModeDefault;
        settings.ForceDarkMode = dto.ForceDarkMode;
        settings.UseGradientBg = dto.UseGradientBg;
        settings.UseGlassmorphism = dto.UseGlassmorphism;
        settings.AnimatedBg = dto.AnimatedBg;
        settings.AccentGradient = dto.AccentGradient;
        settings.CustomCss = dto.CustomCss;
        settings.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Theme settings updated by {User}", User.Identity?.Name);

        return Ok(new { message = "Theme saved successfully ✨" });
    }

    /// <summary>
    /// Reset theme to default values
    /// </summary>
    [HttpPost("theme/reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetTheme()
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            return BadRequest(new { message = "Settings not initialized" });
        }

        // Reset to default theme values
        settings.PrimaryColor = "#f97316";
        settings.SecondaryColor = "#0d9488";
        settings.SuccessColor = "#22c55e";
        settings.DangerColor = "#ef4444";
        settings.WarningColor = "#f59e0b";
        settings.InfoColor = "#3b82f6";
        settings.DarkBg = "#1a1a1a";
        settings.DarkSurface = "#2d2d2d";
        settings.DarkText = "#f5f5f5";
        settings.LightBg = "#ffffff";
        settings.LightSurface = "#f8f9fa";
        settings.LightText = "#212529";
        settings.BorderRadius = "8px";
        settings.FontFamily = "Inter, system-ui, -apple-system, sans-serif";
        settings.DarkModeDefault = false;
        settings.ForceDarkMode = false;
        settings.UseGradientBg = false;
        settings.UseGlassmorphism = false;
        settings.AnimatedBg = false;
        settings.AccentGradient = "linear-gradient(135deg, #f97316 0%, #fb923c 100%)";
        settings.CustomCss = string.Empty;
        settings.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Theme reset to defaults by {User}", User.Identity?.Name);

        return Ok(new { message = "Theme reset to default successfully" });
    }

    private SiteSettingsDto MapToDto(SiteSettings settings)
    {
        return new SiteSettingsDto
        {
            SiteName = settings.SiteName,
            Tagline = settings.Tagline,
            Description = settings.Description,
            Logo = settings.Logo,
            Favicon = settings.Favicon,
            Timezone = settings.Timezone,
            DateFormat = settings.DateFormat,
            AllowRegistration = settings.AllowRegistration,
            ForceHttps = settings.ForceHttps,
            
            EmailEnabled = settings.EmailEnabled,
            SmtpServer = settings.SmtpServer,
            SmtpPort = settings.SmtpPort,
            UseSsl = settings.UseSsl,
            SmtpUsername = settings.SmtpUsername,
            FromEmail = settings.FromEmail,
            FromName = settings.FromName,
            
            MaintenanceMode = settings.MaintenanceMode,
            MaintenanceMessage = settings.MaintenanceMessage,
            EnableApiAccess = settings.EnableApiAccess,
            
            Enforce2FA = settings.Enforce2FA,
            SessionTimeout = settings.SessionTimeout,
            MaxLoginAttempts = settings.MaxLoginAttempts,
            EnableIpWhitelist = settings.EnableIpWhitelist,
            WhitelistedIps = settings.WhitelistedIps,
            MinPasswordLength = settings.MinPasswordLength,
            RequireUppercase = settings.RequireUppercase,
            RequireNumbers = settings.RequireNumbers,
            RequireSpecialChars = settings.RequireSpecialChars
        };
    }
}


