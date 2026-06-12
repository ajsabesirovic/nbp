using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Trainer.Commands;
using FitJourney.Application.Features.Trainer.Queries;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Trainer.Handlers;

public class GetTrainerProfileQueryHandler(ITrainerProfileRepository profiles, IMapper mapper)
    : IRequestHandler<GetTrainerProfileQuery, TrainerProfileDto>
{
    public async Task<TrainerProfileDto> Handle(GetTrainerProfileQuery q, CancellationToken ct)
    {
        var profile = await profiles.GetByUserIdAsync(q.TrainerUserId);
        if (profile != null) return mapper.Map<TrainerProfileDto>(profile);

        var now = DateTime.UtcNow;
        var created = await profiles.UpsertAsync(new TrainerProfile
        {
            UserId = q.TrainerUserId,
            CreatedAt = now,
            UpdatedAt = now,
        });
        return mapper.Map<TrainerProfileDto>(created);
    }
}

public class UpdateTrainerProfileCommandHandler(ITrainerProfileRepository profiles, IMapper mapper)
    : IRequestHandler<UpdateTrainerProfileCommand, TrainerProfileDto>
{
    public async Task<TrainerProfileDto> Handle(UpdateTrainerProfileCommand cmd, CancellationToken ct)
    {
        var profile = await profiles.GetByUserIdAsync(cmd.TrainerUserId) ?? new TrainerProfile
        {
            UserId = cmd.TrainerUserId,
            CreatedAt = DateTime.UtcNow,
        };

        if (cmd.Request.Certifications != null) profile.Certifications = cmd.Request.Certifications;
        if (cmd.Request.Specialization != null) profile.Specialization = cmd.Request.Specialization;
        if (cmd.Request.PricePerPlan.HasValue) profile.PricePerPlan = cmd.Request.PricePerPlan;
        if (cmd.Request.Bio != null) profile.Bio = cmd.Request.Bio;

        var saved = await profiles.UpsertAsync(profile);
        return mapper.Map<TrainerProfileDto>(saved);
    }
}
