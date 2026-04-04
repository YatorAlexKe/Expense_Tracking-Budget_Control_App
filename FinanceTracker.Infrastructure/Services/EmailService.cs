using FinanceTracker.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FinanceTracker.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var apiKey = _config["SendGrid:ApiKey"];
        var from   = _config["SendGrid:FromEmail"];
        var name   = _config["SendGrid:FromName"];

        _logger.LogInformation("Sending reset email to {Email}", toEmail);

        if (string.IsNullOrEmpty(apiKey) || apiKey == "SG.your-api-key-here")
        {
            _logger.LogWarning("SendGrid API key is missing or not configured!");
            return;
        }

        var client = new SendGridClient(apiKey);
        var msg    = new SendGridMessage
        {
            From             = new EmailAddress(from, name),
            Subject          = "Reset Your FinanceTracker Password",
            PlainTextContent = $"Click the link below to reset your password:\n\n{resetLink}\n\nThis link expires in 30 minutes.",
            HtmlContent      = $@"
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
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation("SendGrid reset response: {StatusCode}", response.StatusCode);
    }

    public async Task SendVerificationEmailAsync(string toEmail, string verifyLink)
    {
        var apiKey = _config["SendGrid:ApiKey"];
        var from   = _config["SendGrid:FromEmail"];
        var name   = _config["SendGrid:FromName"];

        _logger.LogInformation("Sending verification email to {Email}", toEmail);

        if (string.IsNullOrEmpty(apiKey) || apiKey == "SG.your-api-key-here")
        {
            _logger.LogWarning("SendGrid API key is missing or not configured!");
            return;
        }

        var client = new SendGridClient(apiKey);
        var msg    = new SendGridMessage
        {
            From             = new EmailAddress(from, name),
            Subject          = "Verify Your FinanceTracker Email",
            PlainTextContent = $"Click the link below to verify your email:\n\n{verifyLink}\n\nThis link expires in 24 hours.",
            HtmlContent      = $@"
                <div style='font-family:sans-serif;max-width:480px;margin:auto;padding:32px;background:#f9f9f9;border-radius:12px'>
                    <h2 style='color:#0f0f13'>Verify Your Email</h2>
                    <p style='color:#555'>Welcome to FinanceTracker! Please verify your email address to activate your account.</p>
                    <a href='{verifyLink}'
                       style='display:inline-block;margin:24px 0;padding:14px 28px;background:#c8f04a;color:#0f0f13;font-weight:700;border-radius:8px;text-decoration:none'>
                       Verify Email Address
                    </a>
                    <p style='color:#999;font-size:13px'>This link expires in <strong>24 hours</strong>. If you did not create an account, ignore this email.</p>
                </div>"
        };

        msg.AddTo(new EmailAddress(toEmail));
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation("SendGrid verification response: {StatusCode}", response.StatusCode);
    }
}