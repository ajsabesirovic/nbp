using System.Security.Claims;
using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitJourney.Application.DTOs;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;

namespace FitJourney.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/measurements")]
[Authorize]
public class BodyMeasurementsController(IBodyMeasurementRepository repo, IMapper mapper) : ControllerBase
{
    private string Uid() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int limit = 100)
    {
        var items = await repo.GetByUserAsync(Uid(), limit);
        return Ok(new { items = mapper.Map<List<BodyMeasurementDto>>(items) });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBodyMeasurementRequest req)
    {
        var now = DateTime.UtcNow;
        var m = new BodyMeasurement
        {
            UserId = Uid(),
            Date = req.RecordedAt ?? now,
            WeightKg = req.WeightKg,
            WaistCm = req.WaistCm,
            ChestCm = req.ChestCm,
            ArmCm = req.ArmCm,
            ThighCm = req.ThighCm,
            BodyFatPct = req.BodyFatPct,
            Note = req.Note,
            CreatedAt = now,
        };
        var saved = await repo.CreateAsync(m);
        return Ok(mapper.Map<BodyMeasurementDto>(saved));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await repo.DeleteAsync(id, Uid());
        return NoContent();
    }
}
