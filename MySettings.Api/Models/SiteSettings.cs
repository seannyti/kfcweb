namespace MySettings.Api.Models;

public class SiteSettings
{
    public int Id { get; set; }
    
    // General Settings
    public string SiteName { get; set; } = "Knudson Family Construction";
    public string Tagline { get; set; } = "Building Excellence Together";
    public string Description { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public string? Favicon { get; set; }
    public string Timezone { get; set; } = "America/New_York";
    public string DateFormat { get; set; } = "MM/DD/YYYY";
    public bool AllowRegistration { get; set; } = true;
    public bool ForceHttps { get; set; } = true;
    
    // Email Settings
    public bool EmailEnabled { get; set; } = true;
    public string SmtpServer { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string SmtpUsername { get; set; } = string.Empty;
    // SECURITY WARNING: This should be encrypted at rest in production
    // Consider using Azure Key Vault or similar for sensitive data
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "Knudson Family Construction";
    
    // Maintenance Settings
    public bool MaintenanceMode { get; set; } = false;
    public string MaintenanceMessage { get; set; } = "We are currently performing scheduled maintenance. Please check back soon.";
    public bool EnableApiAccess { get; set; } = true;
    
    // Security Settings
    public bool Enforce2FA { get; set; } = false;
    public int SessionTimeout { get; set; } = 60; // minutes
    public int MaxLoginAttempts { get; set; } = 5;
    public bool EnableIpWhitelist { get; set; } = false;
    public string WhitelistedIps { get; set; } = string.Empty;
    public int MinPasswordLength { get; set; } = 8;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireNumbers { get; set; } = true;
    public bool RequireSpecialChars { get; set; } = true;
    
    // Theme & Color Settings
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
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}


