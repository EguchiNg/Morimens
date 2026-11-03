using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Morimens.Api.DTOs.Requests;
using Morimens.Api.DTOs.Responses;
using Morimens.Core.Interfaces;
using Morimens.Core.Models;

namespace Morimens.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    private readonly ISavedSimulationRepository _simulations;

    public SimulationController(ISavedSimulationRepository simulations) =>
        _simulations = simulations;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? throw new InvalidOperationException("No user ID in token.");

    // ── GET /api/simulation ───────────────────────────────────────────────────
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SavedSimulationResponse>>> GetAll()
    {
        var sims = await _simulations.GetByUserIdAsync(UserId);
        return Ok(sims.Select(Map));
    }

    // ── GET /api/simulation/{id} ──────────────────────────────────────────────
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SavedSimulationResponse>> GetById(int id)
    {
        var sim = await _simulations.GetByIdAsync(id, UserId);
        return sim is null ? NotFound() : Ok(Map(sim));
    }

    // ── POST /api/simulation ──────────────────────────────────────────────────
    [HttpPost]
    public async Task<ActionResult<SavedSimulationResponse>> Create([FromBody] SaveSimulationRequest req)
    {
        var sim = new SavedSimulation
        {
            UserId     = UserId,
            Name       = req.Name,
            InputsJson = JsonSerializer.Serialize(req.InputsJson),
            ResultJson = JsonSerializer.Serialize(req.ResultJson),
        };

        var created = await _simulations.CreateAsync(sim);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, Map(created));
    }

    // ── PUT /api/simulation/{id} ──────────────────────────────────────────────
    [HttpPut("{id:int}")]
    public async Task<ActionResult<SavedSimulationResponse>> Update(int id, [FromBody] SaveSimulationRequest req)
    {
        var sim = await _simulations.GetByIdAsync(id, UserId);
        if (sim is null) return NotFound();

        sim.Name       = req.Name;
        sim.InputsJson = JsonSerializer.Serialize(req.InputsJson);
        sim.ResultJson = JsonSerializer.Serialize(req.ResultJson);

        var updated = await _simulations.UpdateAsync(sim);
        return Ok(Map(updated));
    }

    // ── DELETE /api/simulation/{id} ───────────────────────────────────────────
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _simulations.DeleteAsync(id, UserId);
        return deleted ? NoContent() : NotFound();
    }

    // ── Mapping ───────────────────────────────────────────────────────────────

    private static SavedSimulationResponse Map(SavedSimulation s) => new()
    {
        Id        = s.Id,
        Name      = s.Name,
        CreatedAt = s.CreatedAt.ToString("o"),
        UpdatedAt = s.UpdatedAt.ToString("o"),
        Inputs    = TryDeserialize(s.InputsJson),
        Result    = TryDeserialize(s.ResultJson),
    };

    private static object? TryDeserialize(string json)
    {
        try { return JsonSerializer.Deserialize<JsonElement>(json); }
        catch { return null; }
    }
}
