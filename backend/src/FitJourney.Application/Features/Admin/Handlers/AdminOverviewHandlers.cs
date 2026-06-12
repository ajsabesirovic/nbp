using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Admin.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Admin.Handlers;

public class GetAdminStatsQueryHandler(
    IUserRepository users,
    IWorkoutSessionRepository sessions,
    IWorkoutPlanRepository plans)
    : IRequestHandler<GetAdminStatsQuery, AdminStatsDto>
{
    public async Task<AdminStatsDto> Handle(GetAdminStatsQuery q, CancellationToken ct)
    {
        var since = DateTime.UtcNow.AddDays(-7);
        return new AdminStatsDto(
            TotalUsers: await users.CountAsync(),
            RegularUsers: await users.CountAsync("user"),
            Trainers: await users.CountAsync("trainer"),
            Admins: await users.CountAsync("admin"),
            NewUsersLast7Days: await users.CountCreatedSinceAsync(since),
            TotalSessions: await sessions.CountAsync(),
            SessionsLast7Days: await sessions.CountSinceAsync(since),
            ActiveUsersLast7Days: await sessions.CountActiveUsersSinceAsync(since),
            TotalPlans: await plans.CountAsync(),
            PublishedPublicPlans: await plans.CountByVisibilityAndStatusAsync("public", "published"));
    }
}

public class GetAdminTrainersQueryHandler(
    IUserRepository users,
    ITrainerProfileRepository profiles)
    : IRequestHandler<GetAdminTrainersQuery, List<AdminTrainerDto>>
{
    public async Task<List<AdminTrainerDto>> Handle(GetAdminTrainersQuery q, CancellationToken ct)
    {
        var trainers = await users.GetByRoleAsync("trainer");
        var result = new List<AdminTrainerDto>();

        foreach (var t in trainers)
        {
            var profile = await profiles.GetByUserIdAsync(t.Id);
            var clients = new List<AdminTrainerClientDto>();
            if (profile != null)
            {
                foreach (var clientId in profile.ClientIds)
                {
                    var c = await users.GetByIdAsync(clientId);
                    if (c != null)
                        clients.Add(new AdminTrainerClientDto(c.Id, c.Name, c.Email, c.CreatedAt));
                }
            }

            result.Add(new AdminTrainerDto(
                t.Id, t.Name, t.Email,
                profile?.Specialization,
                clients.Count,
                clients));
        }

        return result;
    }
}

public class AdminListPlansQueryHandler(IWorkoutPlanRepository plans, IUserRepository users, IMapper mapper)
    : IRequestHandler<AdminListPlansQuery, PagedResult<PlanDto>>
{
    public async Task<PagedResult<PlanDto>> Handle(AdminListPlansQuery q, CancellationToken ct)
    {

        List<string>? authorIds = null;
        if (!string.IsNullOrWhiteSpace(q.AuthorRole))
        {
            var roleUsers = await users.GetByRoleAsync(q.AuthorRole);
            authorIds = roleUsers.Select(u => u.Id).ToList();
        }

        var (items, total) = await plans.AdminListAsync(authorIds, "public", q.Page, q.Limit);
        return new PagedResult<PlanDto>(mapper.Map<List<PlanDto>>(items), total, q.Page, q.Limit);
    }
}
