using System.Security.Claims;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    public DashboardController(IDashboardService service) => _service = service;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Returns a monthly summary: total spending, total budgets,
    /// utilization percentage, and the top 3 spending categories.
    /// </summary>
    [HttpGet("monthly-summary")]
    [ProducesResponseType(typeof(MonthlySummaryResponse), 200)]
    public async Task<IActionResult> MonthlySummary()
    {
        var result = await _service.GetMonthlySummaryAsync(CurrentUserId);
        return Ok(result);
    }
}
