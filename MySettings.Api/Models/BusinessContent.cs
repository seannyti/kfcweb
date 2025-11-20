namespace MySettings.Api.Models;

public class BusinessContent
{
    public int Id { get; set; }
    
    // About Us
    public string AboutTitle { get; set; } = "About Knudson Family Construction";
    public string AboutDescription { get; set; } = "We are a family-owned construction company with over 20 years of experience building quality homes and commercial projects.";
    public string AboutImage { get; set; } = string.Empty;
    public string MissionStatement { get; set; } = "Our mission is to deliver exceptional construction services with integrity, quality, and customer satisfaction at the forefront of everything we do.";
    public int YearsInBusiness { get; set; } = 20;
    public int ProjectsCompleted { get; set; } = 150;
    public int HappyClients { get; set; } = 200;
    public int TeamMembers { get; set; } = 15;
    
    // Contact Information
    public string ContactPhone { get; set; } = "(555) 123-4567";
    public string ContactEmail { get; set; } = "info@knudsonconstruction.com";
    public string ContactAddress { get; set; } = "123 Main Street, Anytown, USA 12345";
    public string BusinessHours { get; set; } = "Monday - Friday: 8:00 AM - 5:00 PM";
    public string GoogleMapsUrl { get; set; } = string.Empty;
    
    // Social Media
    public string FacebookUrl { get; set; } = string.Empty;
    public string InstagramUrl { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string TwitterUrl { get; set; } = string.Empty;
    
    // Quote Request Settings
    public string QuoteFormTitle { get; set; } = "Request a Free Quote";
    public string QuoteFormDescription { get; set; } = "Fill out the form below and we'll get back to you within 24 hours with a detailed estimate for your project.";
    public bool EnableQuoteRequests { get; set; } = true;
    public string QuoteNotificationEmail { get; set; } = string.Empty;
    
    // Hero Section
    public string HeroTitle { get; set; } = "Building Your Dreams Into Reality";
    public string HeroSubtitle { get; set; } = "Quality construction services for residential and commercial projects";
    public string HeroButtonText { get; set; } = "Get a Free Quote";
    public string HeroImage { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
