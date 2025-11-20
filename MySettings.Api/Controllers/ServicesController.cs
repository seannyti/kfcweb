using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySettings.Api.Data;
using MySettings.Api.Models;
using MySettings.Api.DTOs;

namespace MySettings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ServicesController> _logger;

    public ServicesController(AppDbContext context, ILogger<ServicesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Public endpoint - returns only active services
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Service>>> GetActiveServices()
    {
        try
        {
            var services = await _context.Services
                .Where(s => s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            return Ok(services);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving services");
            return StatusCode(500, "An error occurred while retrieving services");
        }
    }

    // Admin endpoint - returns all services
    [HttpGet("admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<IEnumerable<Service>>> GetAllServices()
    {
        try
        {
            var services = await _context.Services
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            return Ok(services);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all services");
            return StatusCode(500, "An error occurred while retrieving services");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<Service>> GetService(int id)
    {
        try
        {
            var service = await _context.Services.FindAsync(id);
            
            if (service == null)
            {
                return NotFound();
            }

            return Ok(service);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving service {ServiceId}", id);
            return StatusCode(500, "An error occurred while retrieving the service");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<Service>> CreateService([FromBody] ServiceDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var service = new Service
            {
                Title = dto.Title,
                Description = dto.Description ?? string.Empty,
                Icon = dto.Icon ?? string.Empty,
                Image = dto.Image ?? string.Empty,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating service");
            return StatusCode(500, "An error occurred while creating the service");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var existing = await _context.Services.FindAsync(id);
            
            if (existing == null)
            {
                return NotFound();
            }

            existing.Title = dto.Title;
            existing.Description = dto.Description ?? string.Empty;
            existing.Icon = dto.Icon ?? string.Empty;
            existing.Image = dto.Image ?? string.Empty;
            existing.DisplayOrder = dto.DisplayOrder;
            existing.IsActive = dto.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Service updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating service {ServiceId}", id);
            return StatusCode(500, "An error occurred while updating the service");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteService(int id)
    {
        try
        {
            var service = await _context.Services.FindAsync(id);
            
            if (service == null)
            {
                return NotFound();
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Service deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting service {ServiceId}", id);
            return StatusCode(500, "An error occurred while deleting the service");
        }
    }
}
