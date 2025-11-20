namespace MySettings.Api.Services;

/// <summary>
/// Email service interface for sending notifications
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send quote request notification email
    /// </summary>
    Task SendQuoteNotificationAsync(string customerName, string customerEmail, string companyName, string phoneNumber, string message);
}
