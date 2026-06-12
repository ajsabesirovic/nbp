using System.Security.Claims;
using AutoMapper;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Progress.Queries;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;

namespace FitJourney.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/progress")]
[Authorize]
public class ProgressController(IMediator mediator, IBodyMeasurementRepository measurements, IMapper mapper) : ControllerBase
{
    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? string.Empty;

    [HttpGet("streak")]
    public async Task<IActionResult> GetStreak()
    {
        var userId = GetUserId();
        var result = await mediator.Send(new GetStreakQuery(userId));
        return Ok(result);
    }

    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeeklyVolume([FromQuery] int weeks = 12)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new GetWeeklyVolumeQuery(userId, weeks));
        return Ok(result);
    }

    [HttpGet("muscle-balance")]
    public async Task<IActionResult> GetMuscleBalance([FromQuery] int weeks = 12)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new GetMuscleBalanceQuery(userId, weeks));
        return Ok(result);
    }

    [HttpGet("progression/{exerciseId}")]
    public async Task<IActionResult> GetProgression(string exerciseId)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new GetProgressionQuery(userId, exerciseId));
        return Ok(result);
    }

    [HttpGet("prs")]
    public async Task<IActionResult> GetPersonalRecords()
    {
        var userId = GetUserId();
        var result = await mediator.Send(new GetPersonalRecordsQuery(userId));
        return Ok(result);
    }

    [HttpGet("measurements")]
    public async Task<IActionResult> ListMeasurements([FromQuery] int limit = 100)
    {
        var items = await measurements.GetByUserAsync(GetUserId(), limit);
        return Ok(new { items = mapper.Map<List<BodyMeasurementDto>>(items) });
    }

    [HttpPost("measurements")]
    public async Task<IActionResult> CreateMeasurement([FromBody] CreateBodyMeasurementRequest req)
    {
        var now = DateTime.UtcNow;
        var m = new BodyMeasurement
        {
            UserId = GetUserId(),
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
        var saved = await measurements.CreateAsync(m);
        return Ok(mapper.Map<BodyMeasurementDto>(saved));
    }
}
