using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySettings.Api.Data;
using MySettings.Api.Models;

namespace MySettings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(AppDbContext context, ILogger<SettingsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get general settings (Public - no auth required)
    /// </summary>
    [HttpGet("general")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetGeneralSettings()
    {
        try
        {
            // Clean up any duplicates first
            var allGeneral = await _context.GeneralSettings.ToListAsync();
            if (allGeneral.Count > 1)
            {
                _context.GeneralSettings.RemoveRange(allGeneral.Skip(1));
                await _context.SaveChangesAsync();
            }
            var allSite = await _context.SiteSettings.ToListAsync();
            if (allSite.Count > 1)
            {
                _context.SiteSettings.RemoveRange(allSite.Skip(1));
                await _context.SaveChangesAsync();
            }

            var settings = await _context.GeneralSettings.FirstOrDefaultAsync();
            var siteSettings = await _context.SiteSettings.FirstOrDefaultAsync();
            
            if (settings == null)
            {
                settings = new GeneralSettings();
                _context.GeneralSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            if (siteSettings == null)
            {
                siteSettings = new SiteSettings();
                _context.SiteSettings.Add(siteSettings);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                id = settings.Id,
                siteName = settings.SiteName,
                tagline = settings.Tagline,
                description = settings.Description,
                logo = settings.Logo,
                favicon = settings.Favicon,
                timezone = settings.Timezone,
                dateFormat = settings.DateFormat,
                allowRegistration = settings.AllowRegistration,
                forceHttps = settings.ForceHttps,
                updatedAt = settings.UpdatedAt,
                maintenanceMode = siteSettings.MaintenanceMode,
                maintenanceMessage = siteSettings.MaintenanceMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting general settings");
            return StatusCode(500, new { error = "Failed to retrieve settings", details = ex.Message });
        }
    }

    /// <summary>
    /// Update general settings (Admin only)
    /// </summary>
    [HttpPut("general")]
    [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GeneralSettings>> UpdateGeneralSettings([FromBody] GeneralSettings request)
    {
        _logger.LogInformation("Received update request - SiteName: {SiteName}, Tagline: {Tagline}", 
            request.SiteName, request.Tagline);

        // Delete all existing settings first to ensure only one row
        var allSettings = await _context.GeneralSettings.ToListAsync();
        if (allSettings.Count > 1)
        {
            _logger.LogWarning("Found {Count} GeneralSettings rows, cleaning up duplicates", allSettings.Count);
            _context.GeneralSettings.RemoveRange(allSettings);
            await _context.SaveChangesAsync();
        }

        var settings = await _context.GeneralSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            _logger.LogInformation("No existing settings found, creating new record");
            settings = new GeneralSettings
            {
                SiteName = request.SiteName,
                Tagline = request.Tagline,
                Description = request.Description,
                Logo = request.Logo,
                Favicon = request.Favicon,
                Timezone = request.Timezone,
                DateFormat = request.DateFormat,
                AllowRegistration = request.AllowRegistration,
                ForceHttps = request.ForceHttps,
                UpdatedAt = DateTime.UtcNow
            };
            _context.GeneralSettings.Add(settings);
        }
        else
        {
            _logger.LogInformation("Updating existing settings with Id: {Id}", settings.Id);
            settings.SiteName = request.SiteName;
            settings.Tagline = request.Tagline;
            settings.Description = request.Description;
            settings.Logo = request.Logo;
            settings.Favicon = request.Favicon;
            settings.Timezone = request.Timezone;
            settings.DateFormat = request.DateFormat;
            settings.AllowRegistration = request.AllowRegistration;
            settings.ForceHttps = request.ForceHttps;
            settings.UpdatedAt = DateTime.UtcNow;
        }

        var changeCount = await _context.SaveChangesAsync();
        _logger.LogInformation("General settings updated successfully. Changes saved: {ChangeCount}", changeCount);

        return Ok(settings);
    }

    /// <summary>
    /// Get all settings for inter-API communication (Public - no auth required)
    /// Used by MyUsers.Api to check registration, security, and maintenance settings
    /// </summary>
    [HttpGet("all")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAllSettings()
    {
        try
        {
            // Clean up any duplicates first
            var allGeneral = await _context.GeneralSettings.ToListAsync();
            if (allGeneral.Count > 1)
            {
                _context.GeneralSettings.RemoveRange(allGeneral.Skip(1));
                await _context.SaveChangesAsync();
            }
            var allSite = await _context.SiteSettings.ToListAsync();
            if (allSite.Count > 1)
            {
                _context.SiteSettings.RemoveRange(allSite.Skip(1));
                await _context.SaveChangesAsync();
            }

            var generalSettings = await _context.GeneralSettings.FirstOrDefaultAsync();
            var siteSettings = await _context.SiteSettings.FirstOrDefaultAsync();
            
            if (generalSettings == null)
            {
                generalSettings = new GeneralSettings();
                _context.GeneralSettings.Add(generalSettings);
                await _context.SaveChangesAsync();
            }

            if (siteSettings == null)
            {
                siteSettings = new SiteSettings();
                _context.SiteSettings.Add(siteSettings);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                // General settings
                allowRegistration = generalSettings.AllowRegistration,
                
                // Security settings from SiteSettings
                maxLoginAttempts = siteSettings.MaxLoginAttempts,
                enableIpWhitelist = siteSettings.EnableIpWhitelist,
                whitelistedIps = siteSettings.WhitelistedIps,
                minPasswordLength = siteSettings.MinPasswordLength,
                requireUppercase = siteSettings.RequireUppercase,
                requireNumbers = siteSettings.RequireNumbers,
                requireSpecialChars = siteSettings.RequireSpecialChars,
                
                // Maintenance settings
                maintenanceMode = siteSettings.MaintenanceMode,
                enableApiAccess = siteSettings.EnableApiAccess
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all settings");
            // Return defaults on error
            return Ok(new
            {
                allowRegistration = true,
                maxLoginAttempts = 5,
                enableIpWhitelist = false,
                whitelistedIps = "",
                minPasswordLength = 8,
                requireUppercase = true,
                requireNumbers = true,
                requireSpecialChars = true,
                maintenanceMode = false,
                enableApiAccess = true
            });
        }
    }

    /// <summary>
    /// Get theme settings (Public - no auth required)
    /// </summary>
    [HttpGet("theme")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetThemeSettings()
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            // Return default theme settings
            return Ok(new
            {
                primaryColor = "#f97316",
                secondaryColor = "#0d9488",
                successColor = "#22c55e",
                dangerColor = "#ef4444",
                warningColor = "#f59e0b",
                infoColor = "#3b82f6",
                darkBg = "#1a1a1a",
                darkSurface = "#2d2d2d",
                darkText = "#f5f5f5",
                lightBg = "#ffffff",
                lightSurface = "#f8f9fa",
                lightText = "#212529",
                borderRadius = "8px",
                fontFamily = "Inter, system-ui, -apple-system, sans-serif",
                darkModeDefault = false,
                forceDarkMode = false,
                useGradientBg = false,
                useGlassmorphism = false,
                animatedBg = false,
                accentGradient = "linear-gradient(135deg, #f97316 0%, #fb923c 100%)",
                customCss = ""
            });
        }

        return Ok(new
        {
            primaryColor = settings.PrimaryColor,
            secondaryColor = settings.SecondaryColor,
            successColor = settings.SuccessColor,
            dangerColor = settings.DangerColor,
            warningColor = settings.WarningColor,
            infoColor = settings.InfoColor,
            darkBg = settings.DarkBg,
            darkSurface = settings.DarkSurface,
            darkText = settings.DarkText,
            lightBg = settings.LightBg,
            lightSurface = settings.LightSurface,
            lightText = settings.LightText,
            borderRadius = settings.BorderRadius,
            fontFamily = settings.FontFamily,
            darkModeDefault = settings.DarkModeDefault,
            forceDarkMode = settings.ForceDarkMode,
            useGradientBg = settings.UseGradientBg,
            useGlassmorphism = settings.UseGlassmorphism,
            animatedBg = settings.AnimatedBg,
            accentGradient = settings.AccentGradient,
            customCss = settings.CustomCss
        });
    }
}

