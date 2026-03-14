using FinanceTracker.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FinanceTracker.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config) => _config = config;

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var apiKey  = _config["SendGrid:ApiKey"];
        var from    = _config["SendGrid:FromEmail"];
        var name    = _config["SendGrid:FromName"];

        var client  = new SendGridClient(apiKey);
        var msg     = new SendGridMessage
        {
            From       = new EmailAddress(from, name),
            Subject    = "Reset Your FinanceTracker Password",
            PlainTextContent = $"Click the link below to reset your password:\n\n{resetLink}\n\nThis link expires in 30 minutes.",
            HtmlContent = $@"
                <div style='font-family:sans-serif;max-width:480px;margin:auto;padding:32px;background:#f9f9f9;border-radius:12px'>
                    <h2 style='color:#0f0f13'>Reset Your Password</h2>
                    <p style='color:#555'>We received a request to reset your FinanceTracker password.</p>
                    <a href='{resetLink}' 
                       style='display:inline-block;margin:24px 0;padding:14px 28px;background:#c8f04a;color:#0f0f13;font-weight:700;border-radius:8px;text-decoration:none'>
                       Reset Password
                    </a>
                    <p style='color:#999;font-size:13px'>This link expires in <strong>30 minutes</strong>. If you didn't request this, ignore this email.</p>
                </div>"
        };

        msg.AddTo(new EmailAddress(toEmail));
        await client.SendEmailAsync(msg);
    }
}