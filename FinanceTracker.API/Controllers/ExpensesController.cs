using System.Security.Claims;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _service;
    public ExpensesController(IExpenseService service) => _service = service;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get expenses, optionally filtered by date range and category.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExpenseResponse>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? categoryId)
    {
        var filter = new ExpenseFilterRequest(from, to, categoryId);
        var result = await _service.GetFilteredAsync(CurrentUserId, filter);
        return Ok(result);
    }

    /// <summary>Create a new expense.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
    {
        var result = await _service.CreateAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetAll), result);
    }

    /// <summary>Update an expense.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ExpenseResponse), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpenseRequest request)
    {
        var result = await _service.UpdateAsync(CurrentUserId, id, request);
        return Ok(result);
    }

    /// <summary>Delete an expense.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(CurrentUserId, id);
        return NoContent();
    }

    /// <summary>Export filtered expenses as a CSV file download.</summary>
    [HttpGet("export")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    public async Task<IActionResult> Export(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? categoryId)
    {
        var filter = new ExpenseFilterRequest(from, to, categoryId);
        var csv = await _service.ExportCsvAsync(CurrentUserId, filter);
        return File(csv, "text/csv", $"expenses_{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
