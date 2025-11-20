using Microsoft.EntityFrameworkCore;
using MyUsers.Api.Data;
using MyUsers.Api.Models;

namespace MyUsers.Api.Services;

/// <summary>
/// Database seeder to create initial users
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedDatabase(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            // Check if any users exist
            if (await context.Users.AnyAsync())
            {
                logger.LogInformation("Users already exist. Skipping database seeding.");
                return;
            }

            logger.LogInformation("Creating initial users...");

            // Create SuperAdmin
            var superAdmin = new User
            {
                Email = "seannytheirish@gmail.com",
                Name = "Sean Knudson",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@123!"),
                Role = Roles.SuperAdmin,
                CreatedAt = DateTime.UtcNow
            };

            // Create Admin
            var admin = new User
            {
                Email = "knudsonfc@yahoo.com",
                Name = "Knudson Family Construction",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = Roles.Admin,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(superAdmin);
            context.Users.Add(admin);
            await context.SaveChangesAsync();

            logger.LogInformation("Default admin users created successfully!");
            logger.LogWarning("CRITICAL SECURITY: Change default passwords immediately via admin panel!");
            logger.LogInformation("Access credentials are documented in .env.development file");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
        }
    }
}

