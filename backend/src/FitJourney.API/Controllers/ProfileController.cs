using System.Security.Claims;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Profile.Commands;
using FitJourney.Application.Features.Profile.Queries;

namespace FitJourney.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/profile")]
[Authorize]
public class ProfileController(IMediator mediator) : ControllerBase
{
    private string GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? string.Empty;

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var result = await mediator.Send(new GetMyProfileQuery(GetUserId()));
        return Ok(result);
    }

    [HttpPatch("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileRequest request)
    {
        var result = await mediator.Send(new UpdateMyProfileCommand(GetUserId(), request));
        return Ok(result);
    }

    [HttpPut("me/active-plan")]
    public async Task<IActionResult> SetActivePlan([FromBody] SetActivePlanRequest request)
    {
        var result = await mediator.Send(new SetActivePlanCommand(GetUserId(), request.PlanId));
        return Ok(result);
    }
}
