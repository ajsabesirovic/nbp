using System.Security.Claims;
using AutoMapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitJourney.Application.DTOs;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Enums;
using FitJourney.Domain.Interfaces;

namespace FitJourney.API.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/messages")]
[Authorize]
public class MessagesController(
    IMessageRepository messages,
    IUserRepository users,
    ITrainerProfileRepository trainers,
    IMapper mapper) : ControllerBase
{
    private string Uid() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? string.Empty;
    private string Uname() =>
        User.FindFirst(ClaimTypes.Name)?.Value ?? User.FindFirst("name")?.Value ?? "Unknown";

    [HttpGet("threads")]
    public async Task<IActionResult> Threads()
    {
        var me = Uid();
        var inbox = await messages.GetInboxAsync(me);
        var threads = inbox
            .GroupBy(m => m.FromUserId == me ? m.ToUserId : m.FromUserId)
            .Select(g =>
            {
                var last = g.OrderByDescending(m => m.CreatedAt).First();
                var otherId = g.Key;
                var otherName = last.FromUserId == me ? last.ToName : last.FromName;
                var unread = g.Count(m => m.ToUserId == me && m.ReadAt == null);
                return new ThreadSummaryDto(otherId, otherName, last.Body, last.CreatedAt, unread);
            })
            .OrderByDescending(t => t.LastAt)
            .ToList();
        return Ok(new { items = threads });
    }

    [HttpGet("thread/{otherId}")]
    public async Task<IActionResult> Thread(string otherId, [FromQuery] int limit = 200)
    {
        var me = Uid();
        var items = await messages.GetThreadAsync(me, otherId, limit);
        await messages.MarkThreadReadAsync(me, otherId);
        return Ok(new { items = mapper.Map<List<MessageDto>>(items) });
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount()
    {
        var n = await messages.CountUnreadAsync(Uid());
        return Ok(new { count = n });
    }

    [HttpGet("contacts")]
    public async Task<IActionResult> Contacts()
    {
        var me = await users.GetByIdAsync(Uid());
        if (me == null) return Unauthorized();
        var contacts = await AllowedContactsAsync(me);
        return Ok(new { items = contacts.Select(u => new { id = u.Id, name = u.Name, role = u.Role.ToString() }) });
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendMessageRequest req)
    {
        var meId = Uid();
        if (string.IsNullOrWhiteSpace(req.Body)) return BadRequest(new { error = "Empty message" });
        var me = await users.GetByIdAsync(meId);
        if (me == null) return BadRequest(new { error = "Sender not found" });
        var to = await users.GetByIdAsync(req.ToUserId);
        if (to == null) return BadRequest(new { error = "Recipient not found" });

        if (!await CanMessageAsync(me, to))
            return Forbid();

        var m = new Message
        {
            FromUserId = meId,
            ToUserId = req.ToUserId,
            FromName = Uname(),
            ToName = to.Name,
            Body = req.Body.Trim(),
            CreatedAt = DateTime.UtcNow,
        };
        var saved = await messages.CreateAsync(m);
        return Ok(mapper.Map<MessageDto>(saved));
    }

    private async Task<bool> CanMessageAsync(User me, User other)
    {

        if (me.Role == UserRole.admin || other.Role == UserRole.admin) return true;

        if (me.Role == UserRole.trainer && other.Role == UserRole.user)
        {
            var profile = await trainers.GetByUserIdAsync(me.Id);
            return profile?.ClientIds.Contains(other.Id) ?? false;
        }

        if (me.Role == UserRole.user && other.Role == UserRole.trainer)
        {
            var profile = await trainers.GetByUserIdAsync(other.Id);
            return profile?.ClientIds.Contains(me.Id) ?? false;
        }

        return false;
    }

    private async Task<List<User>> AllowedContactsAsync(User me)
    {

        if (me.Role == UserRole.admin)
        {
            var (all, _) = await users.ListAsync(null, null, 1, 1000);
            return all.Where(u => u.Id != me.Id).ToList();
        }

        var (admins, _) = await users.ListAsync(null, UserRole.admin.ToString(), 1, 1000);
        var result = new List<User>(admins);

        if (me.Role == UserRole.trainer)
        {

            var profile = await trainers.GetByUserIdAsync(me.Id);
            foreach (var clientId in profile?.ClientIds ?? [])
            {
                var c = await users.GetByIdAsync(clientId);
                if (c != null) result.Add(c);
            }
        }
        else
        {

            foreach (var tp in await trainers.GetTrainersForClientAsync(me.Id))
            {
                var t = await users.GetByIdAsync(tp.UserId);
                if (t != null) result.Add(t);
            }
        }

        return result
            .Where(u => u.Id != me.Id)
            .GroupBy(u => u.Id)
            .Select(g => g.First())
            .ToList();
    }
}
