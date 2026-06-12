using System.Security.Claims;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Sessions.Commands;
using FitJourney.Application.Features.Sessions.Queries;

namespace FitJourney.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/sessions")]
[Authorize]
public class SessionsController(IMediator mediator) : ControllerBase
{
    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? string.Empty;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new GetSessionsQuery(userId, page, limit));
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new GetSessionByIdQuery(id, userId));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateSessionRequest request)
    {
        var userId = GetUserId();
        var result = await mediator.Send(new LogSessionCommand(request, userId));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = GetUserId();
        await mediator.Send(new DeleteSessionCommand(id, userId));
        return NoContent();
    }
}
