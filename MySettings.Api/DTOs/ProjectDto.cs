using System.ComponentModel.DataAnnotations;

namespace MySettings.Api.DTOs;

/// <summary>
/// DTO for project creation and updates with validation
/// </summary>
public class ProjectDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    [StringLength(100, ErrorMessage = "Category must not exceed 100 characters")]
    public string Category { get; set; } = string.Empty;

    [StringLength(150, ErrorMessage = "Location must not exceed 150 characters")]
    public string? Location { get; set; }

    [Url(ErrorMessage = "Image URL must be valid")]
    [StringLength(500, ErrorMessage = "Image URL must not exceed 500 characters")]
    public string? ImageUrl { get; set; }

    [StringLength(2000, ErrorMessage = "Gallery images must not exceed 2000 characters")]
    public string? GalleryImages { get; set; }

    public DateTime? CompletionDate { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsActive { get; set; } = true;

    [Range(0, 9999, ErrorMessage = "Display order must be between 0 and 9999")]
    public int DisplayOrder { get; set; }
}
