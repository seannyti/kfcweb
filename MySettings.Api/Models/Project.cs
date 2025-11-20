namespace MySettings.Api.Models;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "Residential"; // Residential, Commercial, Renovation, etc.
    public string Location { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string GalleryImages { get; set; } = string.Empty; // JSON string of image URLs
    public DateTime? CompletionDate { get; set; }
    public bool IsFeatured { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
