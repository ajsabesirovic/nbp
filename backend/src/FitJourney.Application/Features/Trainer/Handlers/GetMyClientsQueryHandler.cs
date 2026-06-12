using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Trainer.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Trainer.Handlers;

public class GetMyClientsQueryHandler(
    ITrainerProfileRepository profiles,
    IUserRepository users)
    : IRequestHandler<GetMyClientsQuery, List<ClientSummaryDto>>
{
    public async Task<List<ClientSummaryDto>> Handle(GetMyClientsQuery q, CancellationToken ct)
    {
        var profile = await profiles.GetByUserIdAsync(q.TrainerUserId);
        if (profile == null || profile.ClientIds.Count == 0) return [];

        var clients = new List<ClientSummaryDto>();
        foreach (var id in profile.ClientIds)
        {
            var u = await users.GetByIdAsync(id);
            if (u != null) clients.Add(new ClientSummaryDto(u.Id, u.Name, u.Email));
        }
        return clients;
    }
}
