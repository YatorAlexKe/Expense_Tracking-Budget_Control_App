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
    private readonly IUserRepository     _users;
    private readonly IConfiguration      _config;
    private readonly IEmailService       _email;
    private readonly ICategoryRepository _categories;

    public AuthService(
        IUserRepository users,
        IConfiguration config,
        IEmailService email,
        ICategoryRepository categories)
    {
        _users      = users;
        _config     = config;
        _email      = email;
        _categories = categories;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check email is not already taken
        var existing = await _users.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        // Validate password strength
        ValidatePasswordStrength(request.Password);

        var requireVerification = _config.GetValue<bool>("Features:RequireEmailVerification");

        // Generate verification token
        var verificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        var user = new User
        {
            Email                  = request.Email,
            PasswordHash           = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsEmailVerified        = !requireVerification,
            EmailVerificationToken = requireVerification ? verificationToken : null
        };

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        // ── Seed default categories for new user ──────────────────
        var defaultCategories = new[]
        {
            "Rent", "Water Bill", "Electricity Bill", "Internet",
            "Groceries", "Dining Out", "Transport", "Fuel",
            "Health", "Subscriptions", "Shopping", "Education",
            "Insurance", "Entertainment", "Clothing"
        };

        foreach (var name in defaultCategories)
        {
            await _categories.AddAsync(new Category
            {
                Name   = name,
                UserId = user.Id
            });
        }
        await _categories.SaveChangesAsync();

        // Send verification email only if feature is enabled
        if (requireVerification)
        {
            var clientUrl  = _config["ClientUrl"] ?? "http://localhost:5500";
            var verifyLink = $"{clientUrl}/finance-tracker-ui.html?verify={verificationToken}";
            await _email.SendVerificationEmailAsync(user.Email, verifyLink);
        }

        return new AuthResponse(GenerateToken(user), user.Email, user.Id, user.IsEmailVerified);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email);

        // Check if account exists
        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        // Check if account is currently locked
        if (user.LockoutUntil.HasValue && user.LockoutUntil > DateTime.UtcNow)
        {
            var remaining = (int)(user.LockoutUntil.Value - DateTime.UtcNow).TotalMinutes + 1;
            throw new UnauthorizedAccessException(
                $"Account locked. Try again in {remaining} minute(s).");
        }

        // Check if email is verified (only if feature is enabled)
        var requireVerification = _config.GetValue<bool>("Features:RequireEmailVerification");
        if (requireVerification && !user.IsEmailVerified)
            throw new UnauthorizedAccessException(
                "Please verify your email before logging in. Check your inbox.");

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutUntil = DateTime.UtcNow.AddMinutes(15);
                await _users.SaveChangesAsync();
                throw new UnauthorizedAccessException(
                    "Too many failed attempts. Account locked for 15 minutes.");
            }

            var attemptsLeft = 5 - user.FailedLoginAttempts;
            await _users.SaveChangesAsync();
            throw new UnauthorizedAccessException(
                $"Invalid credentials. {attemptsLeft} attempt(s) remaining before lockout.");
        }

        // Successful login — reset lockout fields
        user.FailedLoginAttempts = 0;
        user.LockoutUntil        = null;
        user.LastLoginAt         = DateTime.UtcNow;
        await _users.SaveChangesAsync();

        return new AuthResponse(GenerateToken(user), user.Email, user.Id, user.IsEmailVerified);
    }

    public async Task VerifyEmailAsync(string token)
    {
        var user = await _users.GetByVerificationTokenAsync(token);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid verification link.");

        user.IsEmailVerified        = true;
        user.EmailVerificationToken = null;
        await _users.SaveChangesAsync();
    }

    public async Task ResendVerificationEmailAsync(string email)
    {
        var user = await _users.GetByEmailAsync(email);
        if (user is null) return;

        if (user.IsEmailVerified)
            throw new ConflictException("Email is already verified.");

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        user.EmailVerificationToken = token;
        await _users.SaveChangesAsync();

        var clientUrl  = _config["ClientUrl"] ?? "http://localhost:5500";
        var verifyLink = $"{clientUrl}/finance-tracker-ui.html?verify={token}";
        await _email.SendVerificationEmailAsync(user.Email, verifyLink);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email);
        if (user is null) return;

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        user.PasswordResetToken       = token;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);
        await _users.SaveChangesAsync();

        var clientUrl = _config["ClientUrl"] ?? "http://localhost:5500";
        var resetLink = $"{clientUrl}/finance-tracker-ui.html?token={token}";
        await _email.SendPasswordResetEmailAsync(user.Email, resetLink);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _users.GetByResetTokenAsync(request.Token);

        if (user is null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired reset token.");

        ValidatePasswordStrength(request.NewPassword);

        user.PasswordHash             = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetToken       = null;
        user.PasswordResetTokenExpiry = null;
        await _users.SaveChangesAsync();
    }

    // ── Password Strength Validation ──────────────────────────────
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

    // ── JWT Token Generation ───────────────────────────────────────
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