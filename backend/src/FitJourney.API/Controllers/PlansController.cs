using System.Security.Claims;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Plans.Commands;
using FitJourney.Application.Features.Plans.Queries;

namespace FitJourney.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/plans")]
[Authorize]
public class PlansController(IMediator mediator) : ControllerBase
{
    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] string? visibility = null,
        [FromQuery] bool mine = false,
        [FromQuery] bool assignedToMe = false)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new GetPlansQuery(userId, visibility, mine, assignedToMe, page, limit));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await mediator.Send(new GetPlanByIdQuery(id, GetUserId(), User.IsInRole("admin")));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanRequest request)
    {
        var userId = GetUserId();
        var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? User.FindFirst("name")?.Value ?? "Unknown";
        var result = await mediator.Send(new CreatePlanCommand(request, userId, userName));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdatePlanRequest request)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new UpdatePlanCommand(id, request, userId));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = GetUserId();
        await mediator.Send(new DeletePlanCommand(id, userId));
        return NoContent();
    }
}
