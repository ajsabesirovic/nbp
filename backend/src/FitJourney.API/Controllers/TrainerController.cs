using System.Security.Claims;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Trainer.Commands;
using FitJourney.Application.Features.Trainer.Queries;

namespace FitJourney.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/trainer")]
[Authorize(Policy = "TrainerOnly")]
public class TrainerController(IMediator mediator) : ControllerBase
{
    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? string.Empty;

    [HttpGet("clients")]
    public async Task<IActionResult> GetClients()
    {
        var items = await mediator.Send(new GetMyClientsQuery(GetUserId()));
        return Ok(new { items });
    }

    [HttpPost("clients")]
    public async Task<IActionResult> AddClient([FromBody] ManageClientRequest request)
    {
        await mediator.Send(new AddClientCommand(GetUserId(), request.ClientId));
        return NoContent();
    }

    [HttpDelete("clients/{clientId}")]
    public async Task<IActionResult> RemoveClient(string clientId)
    {
        await mediator.Send(new RemoveClientCommand(GetUserId(), clientId));
        return NoContent();
    }

    [HttpGet("clients/{clientId}/summary")]
    public async Task<IActionResult> GetClientSummary(string clientId)
    {
        var result = await mediator.Send(new GetClientSummaryQuery(GetUserId(), clientId));
        return Ok(result);
    }

    [HttpGet("clients/{clientId}/sessions")]
    public async Task<IActionResult> GetClientSessions(string clientId, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var result = await mediator.Send(new GetClientSessionsQuery(GetUserId(), clientId, page, limit));
        return Ok(result);
    }

    [HttpGet("clients/{clientId}/sessions/{sessionId}")]
    public async Task<IActionResult> GetClientSession(string clientId, string sessionId)
    {
        var result = await mediator.Send(new GetClientSessionByIdQuery(GetUserId(), clientId, sessionId));
        return Ok(result);
    }

    [HttpGet("clients/{clientId}/measurements")]
    public async Task<IActionResult> GetClientMeasurements(string clientId, [FromQuery] int limit = 100)
    {
        var items = await mediator.Send(new GetClientMeasurementsQuery(GetUserId(), clientId, limit));
        return Ok(new { items });
    }

    [HttpPost("clients/{clientId}/measurements")]
    public async Task<IActionResult> CreateClientMeasurement(string clientId, [FromBody] CreateBodyMeasurementRequest request)
    {
        var result = await mediator.Send(new CreateClientMeasurementCommand(GetUserId(), clientId, request));
        return Ok(result);
    }

    [HttpGet("plans/{planId}/clients")]
    public async Task<IActionResult> GetPlanClients(string planId)
    {
        var items = await mediator.Send(new GetPlanAssignedClientsQuery(GetUserId(), planId));
        return Ok(new { items });
    }

    [HttpPost("plans/{planId}/assign")]
    public async Task<IActionResult> AssignPlan(string planId, [FromBody] AssignPlanRequest request)
    {
        var result = await mediator.Send(new AssignPlanToClientCommand(GetUserId(), planId, request.ClientId));
        return Ok(result);
    }

    [HttpPost("plans/{planId}/unassign")]
    public async Task<IActionResult> UnassignPlan(string planId, [FromBody] AssignPlanRequest request)
    {
        await mediator.Send(new UnassignPlanFromClientCommand(GetUserId(), planId, request.ClientId));
        return NoContent();
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await mediator.Send(new GetTrainerProfileQuery(GetUserId()));
        return Ok(result);
    }

    [HttpPatch("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateTrainerProfileRequest request)
    {
        var result = await mediator.Send(new UpdateTrainerProfileCommand(GetUserId(), request));
        return Ok(result);
    }
}
