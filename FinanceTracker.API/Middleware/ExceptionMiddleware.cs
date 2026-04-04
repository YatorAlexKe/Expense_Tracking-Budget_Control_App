using System.Net;
using System.Text.Json;
using FinanceTracker.Application.Common;

namespace FinanceTracker.API.Middleware;

/// <summary>
/// Catches all unhandled exceptions and maps them to appropriate HTTP responses.
/// This keeps controllers clean — they never need try/catch blocks.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleAsync(context, ex);
        }
    }

    private static Task HandleAsync(HttpContext context, Exception ex)
    {
        var (status, message) = ex switch
        {
            NotFoundException           e => (HttpStatusCode.NotFound, e.Message),
            ForbiddenException          e => (HttpStatusCode.Forbidden, e.Message),
            ConflictException           e => (HttpStatusCode.Conflict, e.Message),
            UnauthorizedAccessException e => (HttpStatusCode.Unauthorized, e.Message),
            ArgumentException           e => (HttpStatusCode.BadRequest, e.Message),
            _                             => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)status;

        var body = JsonSerializer.Serialize(new { error = message });
        return context.Response.WriteAsync(body);
    }
}
