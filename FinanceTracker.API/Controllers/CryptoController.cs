using System.Security.Claims;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CryptoController : ControllerBase
{
    private readonly ICryptoService _service;
    public CryptoController(ICryptoService service) => _service = service;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get all crypto assets for the current user.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CryptoAssetResponse>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync(CurrentUserId);
        return Ok(result);
    }

    /// <summary>Add a crypto asset manually.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CryptoAssetResponse), 201)]
    public async Task<IActionResult> Create([FromBody] CreateCryptoRequest request)
    {
        var result = await _service.CreateAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetAll), result);
    }

    /// <summary>Update an existing crypto asset.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CryptoAssetResponse), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCryptoRequest request)
    {
        var result = await _service.UpdateAsync(CurrentUserId, id, request);
        return Ok(result);
    }

    /// <summary>Delete a crypto asset.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(CurrentUserId, id);
        return NoContent();
    }

    /// <summary>
    /// Portfolio summary: total invested, current value (mock prices),
    /// profit/loss per position and overall.
    /// </summary>
    [HttpGet("portfolio-summary")]
    [ProducesResponseType(typeof(PortfolioSummaryResponse), 200)]
    public async Task<IActionResult> PortfolioSummary()
    {
        var result = await _service.GetPortfolioSummaryAsync(CurrentUserId);
        return Ok(result);
    }
}
