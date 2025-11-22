namespace MyUsers.Api.Services;

/// <summary>
/// Interface for email operations
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send verification email to user
    /// </summary>
    Task SendVerificationEmailAsync(string toEmail, string toName, string verificationToken);
    
    /// <summary>
    /// Send password reset email to user
    /// </summary>
    Task SendPasswordResetEmailAsync(string toEmail, string toName, string resetToken);
}
