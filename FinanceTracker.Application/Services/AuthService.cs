using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using FinanceTracker.Application.Common;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinanceTracker.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IConfiguration _config;
    private readonly IEmailService _email;

    public AuthService(IUserRepository users, IConfiguration config, IEmailService email)
    {
        _users  = users;
        _config = config;
        _email  = email;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check email is not already taken
        var existing = await _users.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        // Validate password strength
        ValidatePasswordStrength(request.Password);

        var user = new User
        {
            Email        = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        return new AuthResponse(GenerateToken(user), user.Email, user.Id);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email);

        // ── Check if account exists ───────────────────────────
        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        // ── Check if account is currently locked ──────────────
        if (user.LockoutUntil.HasValue && user.LockoutUntil > DateTime.UtcNow)
        {
            var remaining = (int)(user.LockoutUntil.Value - DateTime.UtcNow).TotalMinutes + 1;
            throw new UnauthorizedAccessException(
                $"Account locked. Try again in {remaining} minute(s).");
        }

        // ── Verify password ───────────────────────────────────
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            // Increment failed attempts
            user.FailedLoginAttempts++;

            // Lock account after 5 failed attempts
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutUntil = DateTime.UtcNow.AddMinutes(15);
                await _users.SaveChangesAsync();
                throw new UnauthorizedAccessException(
                    "Too many failed attempts. Account locked for 15 minutes.");
            }

            // Show how many attempts are left
            var attemptsLeft = 5 - user.FailedLoginAttempts;
            await _users.SaveChangesAsync();
            throw new UnauthorizedAccessException(
                $"Invalid credentials. {attemptsLeft} attempt(s) remaining before lockout.");
        }

        // ── Successful login — reset lockout & update timestamp ──
        user.FailedLoginAttempts = 0;
        user.LockoutUntil        = null;
        user.LastLoginAt         = DateTime.UtcNow;
        await _users.SaveChangesAsync();

        return new AuthResponse(GenerateToken(user), user.Email, user.Id);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email);

        // Always return success even if email not found (security best practice)
        if (user is null) return;

        // Generate a secure random token
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        user.PasswordResetToken       = token;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);

        await _users.SaveChangesAsync();

        // Build reset link
        var clientUrl = _config["ClientUrl"] ?? "http://localhost:5500";
        var resetLink = $"{clientUrl}/finance-tracker-ui.html?token={token}";

        await _email.SendPasswordResetEmailAsync(user.Email, resetLink);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _users.GetByResetTokenAsync(request.Token);

        if (user is null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired reset token.");

        // Validate new password strength
        ValidatePasswordStrength(request.NewPassword);

        user.PasswordHash             = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetToken       = null;
        user.PasswordResetTokenExpiry = null;

        await _users.SaveChangesAsync();
    }

    // ── Password Strength Validation ─────────────────────────────
    private static void ValidatePasswordStrength(string password)
    {
        var errors = new List<string>();

        if (password.Length < 8)
            errors.Add("at least 8 characters");
        if (!Regex.IsMatch(password, @"[A-Z]"))
            errors.Add("one uppercase letter");
        if (!Regex.IsMatch(password, @"[a-z]"))
            errors.Add("one lowercase letter");
        if (!Regex.IsMatch(password, @"\d"))
            errors.Add("one number");

        if (errors.Any())
            throw new ArgumentException(
                $"Password must contain {string.Join(", ", errors)}.");
    }

    // ── JWT Token Generation ──────────────────────────────────────
    private string GenerateToken(User user)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             jwtSection["Issuer"],
            audience:           jwtSection["Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}