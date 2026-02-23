using System.Security.Claims;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

/// <summary>
/// Manage expense categories for the authenticated user.
/// This is the "full example" controller that demonstrates the complete pattern:
///   - [Authorize] at class level
///   - UserId extracted from JWT via ClaimTypes.NameIdentifier
///   - Controller delegates entirely to service (no business logic here)
///   - Proper HTTP status codes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service) => _service = service;

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>Extract authenticated user's id from the JWT sub claim.</summary>
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ── Endpoints ─────────────────────────────────────────────────────────────

    /// <summary>Get all categories belonging to the current user.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync(CurrentUserId);
        return Ok(result);
    }

    /// <summary>Create a new category.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request)
    {
        var result = await _service.CreateAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    /// <summary>Update an existing category.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryRequest request)
    {
        var result = await _service.UpdateAsync(CurrentUserId, id, request);
        return Ok(result);
    }

    /// <summary>Delete a category.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(CurrentUserId, id);
        return NoContent();
    }
}
