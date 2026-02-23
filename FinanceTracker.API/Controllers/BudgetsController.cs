using System.Security.Claims;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _service;
    public BudgetsController(IBudgetService service) => _service = service;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get monthly budgets with current spending and status flags.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BudgetResponse>), 200)]
    public async Task<IActionResult> GetMonthly([FromQuery] int? month, [FromQuery] int? year)
    {
        var now = DateTime.UtcNow;
        var result = await _service.GetMonthlyBudgetsAsync(
            CurrentUserId, month ?? now.Month, year ?? now.Year);
        return Ok(result);
    }

    /// <summary>Set (create or update) a monthly budget for a category.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(BudgetResponse), 200)]
    public async Task<IActionResult> Set([FromBody] BudgetRequest request)
    {
        var result = await _service.SetBudgetAsync(CurrentUserId, request);
        return Ok(result);
    }

    /// <summary>Update an existing budget entry.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BudgetResponse), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] BudgetRequest request)
    {
        var result = await _service.UpdateBudgetAsync(CurrentUserId, id, request);
        return Ok(result);
    }

    /// <summary>Delete a budget entry.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteBudgetAsync(CurrentUserId, id);
        return NoContent();
    }
}
