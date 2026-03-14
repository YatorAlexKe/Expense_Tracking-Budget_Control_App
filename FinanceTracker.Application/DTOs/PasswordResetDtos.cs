namespace FinanceTracker.Application.DTOs;

public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NewPassword);