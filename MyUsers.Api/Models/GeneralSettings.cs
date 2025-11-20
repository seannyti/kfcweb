namespace MyUsers.Api.Models;

public class GeneralSettings
{
    public int Id { get; set; }
    public string SiteName { get; set; } = "Knudson Family Construction";
    public string Tagline { get; set; } = "Building Excellence Together";
    public string Description { get; set; } = "Professional construction services";
    public string? Logo { get; set; }
    public string? Favicon { get; set; }
    public string Timezone { get; set; } = "America/New_York";
    public string DateFormat { get; set; } = "MM/DD/YYYY";
    public bool AllowRegistration { get; set; } = true;
    public bool ForceHttps { get; set; } = false;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

