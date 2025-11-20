using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using MySettings.Api.Data;

namespace MySettings.Api.Services;

/// <summary>
/// SMTP email service implementation - reads from database settings with env fallback
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;
    private readonly AppDbContext _context;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger, AppDbContext context)
    {
        _configuration = configuration;
        _logger = logger;
        _context = context;
    }

    public async Task SendQuoteNotificationAsync(string customerName, string customerEmail, string companyName, string phoneNumber, string message)
    {
        try
        {
            // Get settings from database first, fallback to environment variables
            var siteSettings = await _context.SiteSettings.FirstOrDefaultAsync();
            
            var smtpHost = siteSettings?.SmtpServer ?? _configuration["SmtpSettings:Host"] ?? _configuration["SMTP_SERVER"];
            var smtpPort = siteSettings?.SmtpPort ?? int.Parse(_configuration["SmtpSettings:Port"] ?? _configuration["SMTP_PORT"] ?? "587");
            var smtpUsername = siteSettings?.SmtpUsername ?? _configuration["SmtpSettings:Username"] ?? _configuration["SMTP_USERNAME"];
            var smtpPassword = siteSettings?.SmtpPassword ?? _configuration["SmtpSettings:Password"] ?? _configuration["SMTP_PASSWORD"];
            var smtpFromEmail = siteSettings?.FromEmail ?? _configuration["SmtpSettings:FromEmail"] ?? _configuration["SMTP_FROM_EMAIL"];
            var smtpFromName = siteSettings?.FromName ?? _configuration["SmtpSettings:FromName"] ?? _configuration["SMTP_FROM_NAME"] ?? "Knudson Family Construction";
            var useSsl = siteSettings?.UseSsl ?? bool.Parse(_configuration["SmtpSettings:UseSsl"] ?? _configuration["SMTP_USE_SSL"] ?? "true");
            var emailEnabled = siteSettings?.EmailEnabled ?? true;
            
            // Quote notification goes to the admin email (FromEmail is typically the admin's email)
            var quoteNotificationEmail = smtpFromEmail ?? _configuration["QuoteSettings:QuoteNotificationEmail"];

            // Validate SMTP configuration
            if (!emailEnabled)
            {
                _logger.LogWarning("Email system is disabled in settings. Email notification skipped.");
                return;
            }

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(quoteNotificationEmail))
            {
                _logger.LogWarning("SMTP configuration incomplete. Email notification skipped.");
                return;
            }

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = useSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpFromEmail ?? smtpUsername!, smtpFromName),
                Subject = $"New Quote Request from {customerName}",
                Body = $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #f97316;'>New Quote Request</h2>
    <p>A new quote request has been submitted on your website.</p>
    
    <table style='border-collapse: collapse; margin: 20px 0;'>
        <tr>
            <td style='padding: 8px; font-weight: bold; border-bottom: 1px solid #ddd;'>Customer Name:</td>
            <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{customerName}</td>
        </tr>
        <tr>
            <td style='padding: 8px; font-weight: bold; border-bottom: 1px solid #ddd;'>Email:</td>
            <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{customerEmail}</td>
        </tr>
        <tr>
            <td style='padding: 8px; font-weight: bold; border-bottom: 1px solid #ddd;'>Phone:</td>
            <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{phoneNumber}</td>
        </tr>
        <tr>
            <td style='padding: 8px; font-weight: bold; border-bottom: 1px solid #ddd;'>Project Type:</td>
            <td style='padding: 8px; border-bottom: 1px solid #ddd;'>{companyName}</td>
        </tr>
    </table>
    
    <h3>Description:</h3>
    <p style='background-color: #f9fafb; padding: 15px; border-left: 4px solid #f97316;'>{message}</p>
    
    <hr style='border: 1px solid #e5e7eb; margin: 20px 0;'>
    <p style='color: #6b7280; font-size: 12px;'>
        Submitted at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
    </p>
</body>
</html>
",
                IsBodyHtml = true
            };

            mailMessage.To.Add(quoteNotificationEmail);

            await smtpClient.SendMailAsync(mailMessage);
            
            _logger.LogInformation("Quote notification email sent successfully to {Email} using database settings", quoteNotificationEmail);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the quote request if email fails
            _logger.LogError(ex, "Failed to send quote notification email");
        }
    }
}
