namespace MySettings.Api.Models;

public class QuoteRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty; // Residential, Commercial, Renovation
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Timeline { get; set; } = string.Empty;
    public string Budget { get; set; } = string.Empty;
    public string Status { get; set; } = "New"; // New, Contacted, Quoted, Won, Lost
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
