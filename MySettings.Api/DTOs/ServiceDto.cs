using System.ComponentModel.DataAnnotations;

namespace MySettings.Api.DTOs;

/// <summary>
/// DTO for service creation and updates with validation
/// </summary>
public class ServiceDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
    public string Description { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Icon must not exceed 100 characters")]
    public string Icon { get; set; } = "bi-hammer";

    [Url(ErrorMessage = "Image must be a valid URL")]
    [StringLength(500, ErrorMessage = "Image URL must not exceed 500 characters")]
    public string? Image { get; set; }

    [Range(0, 9999, ErrorMessage = "Display order must be between 0 and 9999")]
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
