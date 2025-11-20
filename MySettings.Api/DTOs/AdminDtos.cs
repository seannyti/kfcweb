namespace MySettings.Api.DTOs;

public class SiteSettingsDto
{
    // General Settings
    public string SiteName { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public string? Favicon { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public string DateFormat { get; set; } = string.Empty;
    public bool AllowRegistration { get; set; }
    public bool ForceHttps { get; set; }
    
    // Email Settings
    public bool EmailEnabled { get; set; }
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public bool UseSsl { get; set; }
    public string SmtpUsername { get; set; } = string.Empty;
    public string? SmtpPassword { get; set; }
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    
    // Maintenance Settings
    public bool MaintenanceMode { get; set; }
    public string MaintenanceMessage { get; set; } = string.Empty;
    public bool EnableApiAccess { get; set; }
    
    // Security Settings
    public bool Enforce2FA { get; set; }
    public int SessionTimeout { get; set; }
    public int MaxLoginAttempts { get; set; }
    public bool EnableIpWhitelist { get; set; }
    public string WhitelistedIps { get; set; } = string.Empty;
    public int MinPasswordLength { get; set; }
    public bool RequireUppercase { get; set; }
    public bool RequireNumbers { get; set; }
    public bool RequireSpecialChars { get; set; }
    public bool EnableRecaptcha { get; set; }
    public string RecaptchaSiteKey { get; set; } = string.Empty;
}

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int EmailsSentToday { get; set; }
    public int EmailsTotal { get; set; }
    public string Uptime { get; set; } = string.Empty;
    public string LastBackup { get; set; } = string.Empty;
}

public class SystemHealthDto
{
    public int Cpu { get; set; }
    public int Memory { get; set; }
    public int Disk { get; set; }
    public int Uptime { get; set; }
}

public class MaintenanceToggleRequest
{
    public bool Enabled { get; set; }
}

public class ThemeSettingsDto
{
    public string PrimaryColor { get; set; } = "#f97316";
    public string SecondaryColor { get; set; } = "#0d9488";
    public string SuccessColor { get; set; } = "#22c55e";
    public string DangerColor { get; set; } = "#ef4444";
    public string WarningColor { get; set; } = "#f59e0b";
    public string InfoColor { get; set; } = "#3b82f6";
    public string DarkBg { get; set; } = "#1a1a1a";
    public string DarkSurface { get; set; } = "#2d2d2d";
    public string DarkText { get; set; } = "#f5f5f5";
    public string LightBg { get; set; } = "#ffffff";
    public string LightSurface { get; set; } = "#f8f9fa";
    public string LightText { get; set; } = "#212529";
    public string BorderRadius { get; set; } = "8px";
    public string FontFamily { get; set; } = "Inter, system-ui, -apple-system, sans-serif";
    public bool DarkModeDefault { get; set; } = false;
    public bool ForceDarkMode { get; set; } = false;
    public bool UseGradientBg { get; set; } = false;
    public bool UseGlassmorphism { get; set; } = false;
    public bool AnimatedBg { get; set; } = false;
    public string AccentGradient { get; set; } = "linear-gradient(135deg, #f97316 0%, #fb923c 100%)";
    public string CustomCss { get; set; } = string.Empty;
}


