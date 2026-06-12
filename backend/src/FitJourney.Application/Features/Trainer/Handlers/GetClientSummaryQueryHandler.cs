using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Trainer.Queries;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Trainer.Handlers;

public class GetClientSummaryQueryHandler(
    ITrainerProfileRepository profiles,
    IUserRepository users,
    IWorkoutPlanRepository plans,
    IPlanAssignmentRepository assignments,
    IWorkoutSessionRepository sessions,
    IProgressPhotoRepository photos,
    IMapper mapper)
    : IRequestHandler<GetClientSummaryQuery, ClientDetailDto>
{
    public async Task<ClientDetailDto> Handle(GetClientSummaryQuery q, CancellationToken ct)
    {
        var profile = await profiles.GetByUserIdAsync(q.TrainerUserId)
            ?? throw new KeyNotFoundException("Trainer profile not found");
        if (!profile.ClientIds.Contains(q.ClientId))
            throw new ForbiddenException("Client is not assigned to this trainer");

        var client = await users.GetByIdAsync(q.ClientId)
            ?? throw new KeyNotFoundException($"Client {q.ClientId} not found");

        var planIds = await assignments.GetActivePlanIdsByTrainerForUserAsync(q.TrainerUserId, q.ClientId);
        var assignedPlans = new List<WorkoutPlan>();
        foreach (var planId in planIds)
        {
            var p = await plans.GetByIdAsync(planId);
            if (p != null) assignedPlans.Add(p);
        }

        var completion = new List<PlanCompletionDto>();
        foreach (var plan in assignedPlans)
        {
            var expected = Math.Max(1, plan.DurationWeeks * plan.DaysPerWeek);
            var logged = await sessions.CountByUserAndPlanAsync(q.ClientId, plan.Id);
            var rate = (double)logged / expected;
            completion.Add(new PlanCompletionDto(plan.Id, plan.Name, expected, logged, Math.Min(rate, 1.0)));
        }

        var recent = await sessions.GetRecentAsync(q.ClientId, 5);
        var clientPhotos = await photos.GetByUserAsync(q.ClientId, 12);
        var profileDto = client.Profile == null ? null : mapper.Map<UserProfileDto>(client.Profile);

        return new ClientDetailDto(
            new ClientSummaryDto(client.Id, client.Name, client.Email),
            profileDto,
            completion,
            mapper.Map<List<SessionDto>>(recent),
            mapper.Map<List<ProgressPhotoDto>>(clientPhotos));
    }
}
