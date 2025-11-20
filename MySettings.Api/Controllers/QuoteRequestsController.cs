using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySettings.Api.Data;
using MySettings.Api.Models;
using MySettings.Api.DTOs;
using MySettings.Api.Services;

namespace MySettings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuoteRequestsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<QuoteRequestsController> _logger;
    private readonly IEmailService _emailService;

    public QuoteRequestsController(AppDbContext context, ILogger<QuoteRequestsController> logger, IEmailService emailService)
    {
        _context = context;
        _logger = logger;
        _emailService = emailService;
    }

    // Public endpoint - submit quote request (no auth)
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<QuoteRequest>> SubmitQuoteRequest([FromBody] QuoteRequestDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Check if quote requests are enabled
            var businessContent = await _context.BusinessContent.FirstOrDefaultAsync();
            if (businessContent != null && !businessContent.EnableQuoteRequests)
            {
                return BadRequest(new { message = "Quote requests are currently disabled" });
            }

            var request = new QuoteRequest
            {
                Name = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Location = dto.Address ?? string.Empty,
                ProjectType = dto.ProjectType ?? string.Empty,
                Description = dto.Description,
                Budget = dto.Budget ?? string.Empty,
                Timeline = dto.Timeline ?? string.Empty,
                Status = "New",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            _context.QuoteRequests.Add(request);
            await _context.SaveChangesAsync();

            // Send email notification
            await _emailService.SendQuoteNotificationAsync(
                request.Name,
                request.Email,
                request.ProjectType,
                request.Phone,
                request.Description
            );

            return Ok(new { message = "Quote request submitted successfully", id = request.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting quote request");
            return StatusCode(500, "An error occurred while submitting your quote request");
        }
    }

    // Admin endpoint - get all quote requests
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<IEnumerable<QuoteRequest>>> GetAllQuoteRequests()
    {
        try
        {
            var requests = await _context.QuoteRequests
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return Ok(requests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quote requests");
            return StatusCode(500, "An error occurred while retrieving quote requests");
        }
    }

    // Admin endpoint - get quote requests by status
    [HttpGet("status/{status}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<IEnumerable<QuoteRequest>>> GetQuoteRequestsByStatus(string status)
    {
        try
        {
            var requests = await _context.QuoteRequests
                .Where(q => q.Status == status)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return Ok(requests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quote requests by status {Status}", status);
            return StatusCode(500, "An error occurred while retrieving quote requests");
        }
    }

    // Admin endpoint - get single quote request
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<QuoteRequest>> GetQuoteRequest(int id)
    {
        try
        {
            var request = await _context.QuoteRequests.FindAsync(id);
            
            if (request == null)
            {
                return NotFound();
            }

            return Ok(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quote request {RequestId}", id);
            return StatusCode(500, "An error occurred while retrieving the quote request");
        }
    }

    // Admin endpoint - update quote request status/notes
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateQuoteRequest(int id, [FromBody] UpdateQuoteRequestDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var existing = await _context.QuoteRequests.FindAsync(id);
            
            if (existing == null)
            {
                return NotFound();
            }

            existing.Status = dto.Status;
            existing.Notes = dto.Notes ?? string.Empty;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Quote request updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quote request {RequestId}", id);
            return StatusCode(500, "An error occurred while updating the quote request");
        }
    }

    // Admin endpoint - delete quote request
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteQuoteRequest(int id)
    {
        try
        {
            var request = await _context.QuoteRequests.FindAsync(id);
            
            if (request == null)
            {
                return NotFound();
            }

            _context.QuoteRequests.Remove(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Quote request deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting quote request {RequestId}", id);
            return StatusCode(500, "An error occurred while deleting the quote request");
        }
    }

    // Admin endpoint - get quote request statistics
    [HttpGet("admin/stats")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult> GetQuoteRequestStats()
    {
        try
        {
            var total = await _context.QuoteRequests.CountAsync();
            var newRequests = await _context.QuoteRequests.CountAsync(q => q.Status == "New");
            var contacted = await _context.QuoteRequests.CountAsync(q => q.Status == "Contacted");
            var quoted = await _context.QuoteRequests.CountAsync(q => q.Status == "Quoted");
            var won = await _context.QuoteRequests.CountAsync(q => q.Status == "Won");
            var lost = await _context.QuoteRequests.CountAsync(q => q.Status == "Lost");

            return Ok(new
            {
                total,
                newRequests,
                contacted,
                quoted,
                won,
                lost,
                conversionRate = total > 0 ? (double)won / total * 100 : 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quote request stats");
            return StatusCode(500, "An error occurred while retrieving statistics");
        }
    }
}
