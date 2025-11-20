using Hangfire.Dashboard;

namespace MyUsers.Api.Filters;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        
        // Only allow SuperAdmin users to access Hangfire Dashboard
        return httpContext.User.Identity?.IsAuthenticated == true && 
               httpContext.User.IsInRole("SuperAdmin");
    }
}
