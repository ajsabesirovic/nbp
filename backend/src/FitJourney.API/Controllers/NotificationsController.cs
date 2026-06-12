using System.Security.Claims;
using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitJourney.Application.DTOs;
using FitJourney.Domain.Interfaces;

namespace FitJourney.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/notifications")]
[Authorize]
public class NotificationsController(
    INotificationRepository repo,
    FitJourney.Application.Services.IReminderService reminders,
    IMapper mapper) : ControllerBase
{
    private string Uid() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int limit = 50)
    {
        var uid = Uid();

        await reminders.EvaluateAsync(uid);
        var items = await repo.GetByUserAsync(uid, limit);
        return Ok(new { items = mapper.Map<List<NotificationDto>>(items) });
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount()
    {
        var n = await repo.CountUnreadAsync(Uid());
        return Ok(new { count = n });
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkRead(string id)
    {
        await repo.MarkReadAsync(id, Uid());
        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        await repo.MarkAllReadAsync(Uid());
        return NoContent();
    }
}
