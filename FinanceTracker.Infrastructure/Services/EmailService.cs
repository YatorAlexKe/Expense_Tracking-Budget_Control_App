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

    public async Task SendMonthlySummaryEmailAsync(string toEmail, MonthlySummaryEmailData data)
    {
        var apiKey = _config["SendGrid:ApiKey"];
        var from   = _config["SendGrid:FromEmail"];
        var name   = _config["SendGrid:FromName"];

        _logger.LogInformation("Sending monthly summary to {Email}", toEmail);

        if (string.IsNullOrEmpty(apiKey) || apiKey == "SG.your-api-key-here")
        {
            _logger.LogWarning("SendGrid API key not configured — skipping monthly summary email.");
            return;
        }

        // Build top categories HTML
        var topCategoriesHtml = string.Join("", data.TopCategories.Select((c, i) => $@"
            <tr>
                <td style='padding:10px 16px;border-bottom:1px solid #e5e7eb;color:#6b7280'>{i + 1}</td>
                <td style='padding:10px 16px;border-bottom:1px solid #e5e7eb;font-weight:600'>{c.Name}</td>
                <td style='padding:10px 16px;border-bottom:1px solid #e5e7eb;font-weight:700;color:#111827'>${c.Amount:N2}</td>
            </tr>"));

        // Budget status color
        var statusColor = data.BudgetStatus switch
        {
            "Exceeded" => "#ef4444",
            "Warning"  => "#f59e0b",
            _          => "#10b981"
        };

        // Net savings color
        var savingsColor = data.NetSavings >= 0 ? "#10b981" : "#ef4444";

        var client = new SendGridClient(apiKey);
        var msg    = new SendGridMessage
        {
            From    = new EmailAddress(from, name),
            Subject = $"Your FinanceTracker Summary — {data.MonthName} {data.Year}",
            PlainTextContent = $@"
    FinanceTracker Monthly Summary — {data.MonthName} {data.Year}

    Total Spending:  ${data.TotalSpending:N2}
    Total Budget:    ${data.TotalBudget:N2}
    Utilization:     {data.UtilizationPct:N1}%
    Budget Status:   {data.BudgetStatus}

    Total Income:    ${data.TotalIncome:N2}
    Net Savings:     ${data.NetSavings:N2}

    Top Categories:
    {string.Join("\n", data.TopCategories.Select((c, i) => $"{i + 1}. {c.Name}: ${c.Amount:N2}"))}

    Login to view your full dashboard: http://localhost:5500/finance-tracker-ui.html",

            HtmlContent = $@"
    <!DOCTYPE html>
    <html>
    <head><meta charset='utf-8'/></head>
    <body style='margin:0;padding:0;background:#f3f4f6;font-family:sans-serif'>
        <div style='max-width:560px;margin:40px auto;background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08)'>

            <!-- Header -->
            <div style='background:#0f0f13;padding:32px;text-align:center'>
                <div style='font-size:24px;font-weight:700;color:#c8f04a;margin-bottom:4px'>💰 FinanceTracker</div>
                <div style='color:#9ca3af;font-size:14px'>Monthly Summary — {data.MonthName} {data.Year}</div>
            </div>

            <!-- Body -->
            <div style='padding:32px'>

                <p style='color:#374151;font-size:15px;margin-bottom:24px'>
                    Hi <strong>{data.UserEmail}</strong>, here's your financial summary for <strong>{data.MonthName} {data.Year}</strong>.
                </p>

                <!-- Stats Grid -->
                <div style='display:grid;grid-template-columns:1fr 1fr;gap:16px;margin-bottom:24px'>

                    <div style='background:#f9fafb;border-radius:12px;padding:16px;border:1px solid #e5e7eb'>
                        <div style='font-size:11px;color:#6b7280;text-transform:uppercase;letter-spacing:.08em;margin-bottom:4px'>Total Spending</div>
                        <div style='font-size:22px;font-weight:700;color:#111827'>${data.TotalSpending:N2}</div>
                        <div style='font-size:12px;color:#6b7280'>of ${data.TotalBudget:N2} budget</div>
                    </div>

                    <div style='background:#f9fafb;border-radius:12px;padding:16px;border:1px solid #e5e7eb'>
                        <div style='font-size:11px;color:#6b7280;text-transform:uppercase;letter-spacing:.08em;margin-bottom:4px'>Budget Status</div>
                        <div style='font-size:22px;font-weight:700;color:{statusColor}'>{data.BudgetStatus}</div>
                        <div style='font-size:12px;color:#6b7280'>{data.UtilizationPct:N1}% utilized</div>
                    </div>

                    <div style='background:#f9fafb;border-radius:12px;padding:16px;border:1px solid #e5e7eb'>
                        <div style='font-size:11px;color:#6b7280;text-transform:uppercase;letter-spacing:.08em;margin-bottom:4px'>Total Income</div>
                        <div style='font-size:22px;font-weight:700;color:#3b82f6'>${data.TotalIncome:N2}</div>
                        <div style='font-size:12px;color:#6b7280'>this month</div>
                    </div>

                    <div style='background:#f9fafb;border-radius:12px;padding:16px;border:1px solid #e5e7eb'>
                        <div style='font-size:11px;color:#6b7280;text-transform:uppercase;letter-spacing:.08em;margin-bottom:4px'>Net Savings</div>
                        <div style='font-size:22px;font-weight:700;color:{savingsColor}'>${data.NetSavings:N2}</div>
                        <div style='font-size:12px;color:#6b7280'>income minus expenses</div>
                    </div>

                </div>

                <!-- Top Categories -->
                {(data.TopCategories.Any() ? $@"
                <div style='margin-bottom:24px'>
                    <div style='font-size:13px;font-weight:700;color:#111827;margin-bottom:12px;text-transform:uppercase;letter-spacing:.05em'>
                        Top Spending Categories
                    </div>
                    <table style='width:100%;border-collapse:collapse;background:#f9fafb;border-radius:12px;overflow:hidden;border:1px solid #e5e7eb'>
                        <thead>
                            <tr style='background:#f3f4f6'>
                                <th style='padding:10px 16px;text-align:left;font-size:11px;color:#6b7280;font-weight:600'>#</th>
                                <th style='padding:10px 16px;text-align:left;font-size:11px;color:#6b7280;font-weight:600'>CATEGORY</th>
                                <th style='padding:10px 16px;text-align:left;font-size:11px;color:#6b7280;font-weight:600'>AMOUNT</th>
                            </tr>
                        </thead>
                        <tbody>{topCategoriesHtml}</tbody>
                    </table>
                </div>" : "")}

                <!-- CTA Button -->
                <div style='text-align:center;margin-top:24px'>
                    <a href='http://localhost:5500/finance-tracker-ui.html'
                    style='display:inline-block;padding:14px 32px;background:#c8f04a;color:#0f0f13;font-weight:700;border-radius:10px;text-decoration:none;font-size:14px'>
                        View Full Dashboard
                    </a>
                </div>

            </div>

            <!-- Footer -->
            <div style='background:#f9fafb;padding:20px 32px;text-align:center;border-top:1px solid #e5e7eb'>
                <div style='font-size:12px;color:#9ca3af'>
                    © {data.Year} FinanceTracker — You're receiving this because you have an account.
                </div>
                <div style='font-size:12px;color:#9ca3af;margin-top:4px'>
                    Questions? <a href='mailto:Finance@track.com' style='color:#c8f04a'>Finance@track.com</a>
                </div>
            </div>

        </div>
    </body>
    </html>"
        };

        msg.AddTo(new EmailAddress(toEmail));
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation("Monthly summary sent to {Email} — Status: {Status}", toEmail, response.StatusCode);
    }
}