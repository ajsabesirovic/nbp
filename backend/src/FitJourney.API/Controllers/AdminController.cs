using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Admin.Commands;
using FitJourney.Application.Features.Admin.Queries;
using FitJourney.Application.Features.Trainer.Commands;

namespace FitJourney.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminController(IMediator mediator) : ControllerBase
{
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await mediator.Send(new GetAdminStatsQuery());
        return Ok(result);
    }

    [HttpGet("trainers")]
    public async Task<IActionResult> GetTrainers()
    {
        var result = await mediator.Send(new GetAdminTrainersQuery());
        return Ok(new { items = result });
    }

    [HttpGet("plans")]
    public async Task<IActionResult> ListPlans(
        [FromQuery] string? authorRole,
        [FromQuery] string? visibility,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 50)
    {
        var result = await mediator.Send(new AdminListPlansQuery(authorRole, visibility, page, limit));
        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<IActionResult> ListUsers(
        [FromQuery] string? search,
        [FromQuery] string? role,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20)
    {
        var result = await mediator.Send(new ListUsersQuery(search, role, page, limit));
        return Ok(result);
    }

    [HttpPatch("users/{id}/role")]
    public async Task<IActionResult> SetUserRole(string id, [FromBody] SetUserRoleRequest request)
    {
        var result = await mediator.Send(new SetUserRoleCommand(id, request.Role));
        return Ok(result);
    }

    [HttpPost("trainers/{trainerId}/clients")]
    public async Task<IActionResult> AssignClientToTrainer(string trainerId, [FromBody] ManageClientRequest request)
    {
        await mediator.Send(new AddClientCommand(trainerId, request.ClientId));
        return NoContent();
    }

    [HttpDelete("trainers/{trainerId}/clients/{clientId}")]
    public async Task<IActionResult> RemoveClientFromTrainer(string trainerId, string clientId)
    {
        await mediator.Send(new RemoveClientCommand(trainerId, clientId));
        return NoContent();
    }

    [HttpGet("plans/public")]
    public async Task<IActionResult> ListPublicPlans(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20)
    {
        var result = await mediator.Send(new ListPendingPublicPlansQuery(status, page, limit));
        return Ok(result);
    }

    [HttpPatch("plans/{id}/status")]
    public async Task<IActionResult> ModeratePlan(string id, [FromBody] ModeratePlanRequest request)
    {
        var result = await mediator.Send(new ModeratePlanCommand(id, request.Status));
        return Ok(result);
    }

    [HttpGet("logs")]
    public async Task<IActionResult> RecentLogs([FromQuery] int page = 1, [FromQuery] int limit = 25)
    {
        var result = await mediator.Send(new RecentLogsQuery(Math.Max(1, page), Math.Clamp(limit, 1, 200)));
        return Ok(result);
    }

    [HttpGet("logs/slow")]
    public async Task<IActionResult> SlowLogs(
        [FromQuery] long thresholdMs = 500,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 25)
    {
        var result = await mediator.Send(new SlowLogsQuery(thresholdMs, Math.Max(1, page), Math.Clamp(limit, 1, 200)));
        return Ok(result);
    }
}
