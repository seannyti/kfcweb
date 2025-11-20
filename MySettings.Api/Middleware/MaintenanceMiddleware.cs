using Microsoft.EntityFrameworkCore;
using MySettings.Api.Data;
using MySettings.Api.Models;

namespace MySettings.Api.Middleware;

public class MaintenanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MaintenanceMiddleware> _logger;

    public MaintenanceMiddleware(RequestDelegate next, ILogger<MaintenanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        // Skip maintenance check for public endpoints
        var path = context.Request.Path.Value?.ToLower() ?? "";
        
        // Allow public settings endpoints (needed for maintenance page)
        // and auth endpoints (login needed for admins to bypass maintenance)
        if (path.Contains("/settings/general") || 
            path.Contains("/settings/theme") ||
            path.Contains("/swagger"))
        {
            await _next(context);
            return;
        }

        // Get maintenance settings
        var siteSettings = await dbContext.SiteSettings.FirstOrDefaultAsync();
        
        if (siteSettings?.MaintenanceMode == true && siteSettings?.EnableApiAccess == false)
        {
            // Check if user is admin
            var isAdmin = context.User?.IsInRole(Roles.Admin) == true || 
                          context.User?.IsInRole(Roles.SuperAdmin) == true;

            if (!isAdmin)
            {
                _logger.LogWarning("API access blocked during maintenance mode for non-admin user");
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Service Unavailable",
                    message = "The API is currently unavailable due to maintenance. Please try again later.",
                    maintenanceMode = true
                });
                return;
            }
        }

        await _next(context);
    }
}
