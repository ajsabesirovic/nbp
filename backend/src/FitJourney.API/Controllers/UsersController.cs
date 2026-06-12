using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitJourney.Application.DTOs;
using FitJourney.Domain.Interfaces;

namespace FitJourney.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/users")]
[Authorize]
public class UsersController(IUserRepository users, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] string? role,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 50)
    {
        var (items, total) = await users.ListAsync(search, role, page, limit);
        return Ok(new
        {
            items = mapper.Map<List<UserDto>>(items),
            total,
            page,
            limit,
        });
    }
}
