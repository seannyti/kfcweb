using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySettings.Api.Data;
using MySettings.Api.Models;
using MySettings.Api.DTOs;

namespace MySettings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(AppDbContext context, ILogger<ProjectsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Public endpoint - returns only active projects
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Project>>> GetActiveProjects()
    {
        try
        {
            var projects = await _context.Projects
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            return Ok(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects");
            return StatusCode(500, "An error occurred while retrieving projects");
        }
    }

    // Public endpoint - returns only featured projects
    [HttpGet("featured")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Project>>> GetFeaturedProjects()
    {
        try
        {
            var projects = await _context.Projects
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderBy(p => p.DisplayOrder)
                .Take(6)
                .ToListAsync();

            return Ok(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving featured projects");
            return StatusCode(500, "An error occurred while retrieving featured projects");
        }
    }

    // Admin endpoint - returns all projects
    [HttpGet("admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<IEnumerable<Project>>> GetAllProjects()
    {
        try
        {
            var projects = await _context.Projects
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            return Ok(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all projects");
            return StatusCode(500, "An error occurred while retrieving projects");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        try
        {
            var project = await _context.Projects.FindAsync(id);
            
            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project {ProjectId}", id);
            return StatusCode(500, "An error occurred while retrieving the project");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<Project>> CreateProject([FromBody] ProjectDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var project = new Project
            {
                Title = dto.Title ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                Category = dto.Category ?? string.Empty,
                Location = dto.Location ?? string.Empty,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                GalleryImages = dto.GalleryImages ?? string.Empty,
                CompletionDate = dto.CompletionDate,
                IsFeatured = dto.IsFeatured,
                IsActive = dto.IsActive,
                DisplayOrder = dto.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            return StatusCode(500, "An error occurred while creating the project");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var existing = await _context.Projects.FindAsync(id);
            
            if (existing == null)
            {
                return NotFound();
            }

            existing.Title = dto.Title ?? string.Empty;
            existing.Description = dto.Description ?? string.Empty;
            existing.Category = dto.Category ?? string.Empty;
            existing.Location = dto.Location ?? string.Empty;
            existing.ImageUrl = dto.ImageUrl ?? string.Empty;
            existing.GalleryImages = dto.GalleryImages ?? string.Empty;
            existing.CompletionDate = dto.CompletionDate;
            existing.IsFeatured = dto.IsFeatured;
            existing.IsActive = dto.IsActive;
            existing.DisplayOrder = dto.DisplayOrder;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Project updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project {ProjectId}", id);
            return StatusCode(500, "An error occurred while updating the project");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            var project = await _context.Projects.FindAsync(id);
            
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Project deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project {ProjectId}", id);
            return StatusCode(500, "An error occurred while deleting the project");
        }
    }
}
