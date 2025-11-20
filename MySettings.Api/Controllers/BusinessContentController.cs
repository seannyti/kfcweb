using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySettings.Api.Data;
using MySettings.Api.Models;

namespace MySettings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusinessContentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<BusinessContentController> _logger;

    public BusinessContentController(AppDbContext context, ILogger<BusinessContentController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Public endpoint - no auth required
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<BusinessContent>> GetBusinessContent()
    {
        try
        {
            var content = await _context.BusinessContent.FirstOrDefaultAsync();
            
            if (content == null)
            {
                // Return default content if none exists
                content = new BusinessContent();
            }

            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving business content");
            return StatusCode(500, "An error occurred while retrieving business content");
        }
    }

    // Admin endpoint - requires authentication
    [HttpPut]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateBusinessContent([FromBody] BusinessContent content)
    {
        try
        {
            var existing = await _context.BusinessContent.FirstOrDefaultAsync();
            
            if (existing == null)
            {
                // Create new record
                content.CreatedAt = DateTime.UtcNow;
                content.UpdatedAt = DateTime.UtcNow;
                _context.BusinessContent.Add(content);
            }
            else
            {
                // Update existing record
                existing.AboutTitle = content.AboutTitle;
                existing.AboutDescription = content.AboutDescription;
                existing.AboutImage = content.AboutImage;
                existing.MissionStatement = content.MissionStatement;
                existing.YearsInBusiness = content.YearsInBusiness;
                existing.ProjectsCompleted = content.ProjectsCompleted;
                existing.HappyClients = content.HappyClients;
                existing.TeamMembers = content.TeamMembers;
                existing.ContactPhone = content.ContactPhone;
                existing.ContactEmail = content.ContactEmail;
                existing.ContactAddress = content.ContactAddress;
                existing.BusinessHours = content.BusinessHours;
                existing.GoogleMapsUrl = content.GoogleMapsUrl;
                existing.FacebookUrl = content.FacebookUrl;
                existing.InstagramUrl = content.InstagramUrl;
                existing.LinkedInUrl = content.LinkedInUrl;
                existing.TwitterUrl = content.TwitterUrl;
                existing.QuoteFormTitle = content.QuoteFormTitle;
                existing.QuoteFormDescription = content.QuoteFormDescription;
                existing.EnableQuoteRequests = content.EnableQuoteRequests;
                existing.QuoteNotificationEmail = content.QuoteNotificationEmail;
                existing.HeroTitle = content.HeroTitle;
                existing.HeroSubtitle = content.HeroSubtitle;
                existing.HeroButtonText = content.HeroButtonText;
                existing.HeroImage = content.HeroImage;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Business content updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating business content");
            return StatusCode(500, "An error occurred while updating business content");
        }
    }
}
