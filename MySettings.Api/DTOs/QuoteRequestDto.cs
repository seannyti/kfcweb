using System.ComponentModel.DataAnnotations;

namespace MySettings.Api.DTOs;

/// <summary>
/// DTO for public quote request submission - only accepts necessary fields
/// </summary>
public class QuoteRequestDto
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(150, ErrorMessage = "Email must not exceed 150 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20, ErrorMessage = "Phone must not exceed 20 characters")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Address must not exceed 200 characters")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Project type is required")]
    [StringLength(100, ErrorMessage = "Project type must not exceed 100 characters")]
    public string ProjectType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Project description is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
    public string Description { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Budget must not exceed 50 characters")]
    public string? Budget { get; set; }

    [StringLength(50, ErrorMessage = "Timeline must not exceed 50 characters")]
    public string? Timeline { get; set; }
}

/// <summary>
/// DTO for updating quote request status (admin only)
/// </summary>
public class UpdateQuoteRequestDto
{
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Notes { get; set; }
}
