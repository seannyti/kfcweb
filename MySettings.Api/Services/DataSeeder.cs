using Microsoft.EntityFrameworkCore;
using MySettings.Api.Data;
using MySettings.Api.Models;

namespace MySettings.Api.Services;

public class DataSeeder
{
    private readonly AppDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(AppDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedBusinessContent()
    {
        try
        {
            var existingContent = await _context.BusinessContent.FirstOrDefaultAsync();
            
            if (existingContent == null)
            {
                _logger.LogInformation("Seeding default business content...");
                
                var businessContent = new BusinessContent
                {
                    HeroTitle = "Building Your Dreams Into Reality",
                    HeroSubtitle = "Professional construction services with quality craftsmanship and attention to detail.",
                    HeroButtonText = "Get a Free Quote",
                    HeroImage = "",
                    AboutTitle = "About Knudson Family Construction",
                    AboutDescription = "We are a family-owned construction company with over 20 years of experience delivering quality projects across residential, commercial, and renovation sectors. Our commitment to excellence and customer satisfaction has made us a trusted name in the industry.",
                    AboutImage = "",
                    MissionStatement = "To deliver exceptional construction services with integrity, quality, and customer satisfaction at the forefront of everything we do.",
                    YearsInBusiness = 20,
                    ProjectsCompleted = 150,
                    HappyClients = 200,
                    TeamMembers = 15,
                    ContactPhone = "(555) 123-4567",
                    ContactEmail = "info@knudsonfamilyconstruction.com",
                    ContactAddress = "123 Main Street, Anytown, ST 12345",
                    BusinessHours = "Monday - Friday: 8:00 AM - 5:00 PM",
                    GoogleMapsUrl = "",
                    FacebookUrl = "",
                    InstagramUrl = "",
                    LinkedInUrl = "",
                    TwitterUrl = "",
                    QuoteFormTitle = "Request a Quote",
                    QuoteFormDescription = "Tell us about your project and we'll get back to you with a free quote within 24 hours.",
                    EnableQuoteRequests = true,
                    QuoteNotificationEmail = "quotes@knudsonfamilyconstruction.com",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.BusinessContent.Add(businessContent);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Business content seeded successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding business content");
        }
    }

    public async Task SeedSampleServices()
    {
        try
        {
            var existingServices = await _context.Services.AnyAsync();
            
            if (!existingServices)
            {
                _logger.LogInformation("Seeding sample services...");
                
                var services = new List<Service>
                {
                    new Service
                    {
                        Title = "Residential Construction",
                        Description = "Custom home building, additions, and remodels. We work with you from design to completion to create your dream home.",
                        Icon = "bi-house-fill",
                        Image = "",
                        DisplayOrder = 1,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Title = "Commercial Construction",
                        Description = "Office buildings, retail spaces, and industrial facilities. Professional construction services for businesses of all sizes.",
                        Icon = "bi-building",
                        Image = "",
                        DisplayOrder = 2,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Title = "Renovations & Remodeling",
                        Description = "Transform your existing space with our expert renovation services. Kitchen, bathroom, basement, and whole-home remodels.",
                        Icon = "bi-hammer",
                        Image = "",
                        DisplayOrder = 3,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Title = "Design & Planning",
                        Description = "Comprehensive design and planning services to bring your vision to life. Work with our experienced team from concept to completion.",
                        Icon = "bi-rulers",
                        Image = "",
                        DisplayOrder = 4,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Title = "Project Management",
                        Description = "Professional project management to keep your construction on time and on budget. Coordinating all aspects of your build.",
                        Icon = "bi-clipboard-check",
                        Image = "",
                        DisplayOrder = 5,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Title = "General Contracting",
                        Description = "Full-service general contracting for projects of all sizes. Licensed, insured, and experienced in all phases of construction.",
                        Icon = "bi-tools",
                        Image = "",
                        DisplayOrder = 6,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                _context.Services.AddRange(services);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Seeded {services.Count} sample services");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding sample services");
        }
    }
}
