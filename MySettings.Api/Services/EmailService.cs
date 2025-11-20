using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using MySettings.Api.Data;

namespace MySettings.Api.Services;

public class EmailService
{
    private readonly AppDbContext _context;
    private readonly ILogger<EmailService> _logger;

    public EmailService(AppDbContext context, ILogger<EmailService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync();
        if (settings == null || !settings.EmailEnabled)
        {
            _logger.LogWarning("Email system is disabled or not configured");
            return false;
        }

        try
        {
            using var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort)
            {
                EnableSsl = settings.UseSsl,
                Credentials = new NetworkCredential(settings.SmtpUsername, settings.SmtpPassword),
                Timeout = 30000 // 30 seconds
            };

            var message = new MailMessage
            {
                From = new MailAddress(settings.FromEmail, settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(to);

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {To}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            return false;
        }
    }

    public async Task<bool> SendTestEmailAsync(string to)
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync();
        var siteName = settings?.FromName ?? "Your Application";
        
        var subject = $"Test Email from {siteName}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #f97316;'>Email Configuration Test</h2>
                <p>This is a test email from your <strong>{siteName}</strong> admin panel.</p>
                <p>If you're reading this, your email configuration is working correctly!</p>
                <hr style='border: 1px solid #e5e7eb; margin: 20px 0;'>
                <p style='color: #6b7280; font-size: 12px;'>
                    Sent at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC<br>
                    SMTP Server: {settings?.SmtpServer ?? "Not configured"}<br>
                    Port: {settings?.SmtpPort ?? 0}<br>
                    SSL Enabled: {settings?.UseSsl ?? false}
                </p>
            </body>
            </html>
        ";

        return await SendEmailAsync(to, subject, body);
    }
}
