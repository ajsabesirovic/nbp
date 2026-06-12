using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Trainer.Commands;
using FitJourney.Application.Features.Trainer.Queries;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Trainer.Handlers;

public class GetClientMeasurementsQueryHandler(
    ITrainerProfileRepository profiles,
    IBodyMeasurementRepository measurements,
    IMapper mapper)
    : IRequestHandler<GetClientMeasurementsQuery, List<BodyMeasurementDto>>
{
    public async Task<List<BodyMeasurementDto>> Handle(GetClientMeasurementsQuery q, CancellationToken ct)
    {
        await EnsureClientOf(profiles, q.TrainerUserId, q.ClientId);
        var items = await measurements.GetByUserAsync(q.ClientId, q.Limit);
        return mapper.Map<List<BodyMeasurementDto>>(items);
    }

    internal static async Task EnsureClientOf(ITrainerProfileRepository profiles, string trainerUserId, string clientId)
    {
        var profile = await profiles.GetByUserIdAsync(trainerUserId);
        if (profile == null || !profile.ClientIds.Contains(clientId))
            throw new ForbiddenException("This user is not your client.");
    }
}

public class CreateClientMeasurementCommandHandler(
    ITrainerProfileRepository profiles,
    IBodyMeasurementRepository measurements,
    IMapper mapper)
    : IRequestHandler<CreateClientMeasurementCommand, BodyMeasurementDto>
{
    public async Task<BodyMeasurementDto> Handle(CreateClientMeasurementCommand cmd, CancellationToken ct)
    {
        await GetClientMeasurementsQueryHandler.EnsureClientOf(profiles, cmd.TrainerUserId, cmd.ClientId);

        var now = DateTime.UtcNow;
        var r = cmd.Request;
        var m = new BodyMeasurement
        {
            UserId = cmd.ClientId,
            Date = r.RecordedAt ?? now,
            WeightKg = r.WeightKg,
            WaistCm = r.WaistCm,
            ChestCm = r.ChestCm,
            ArmCm = r.ArmCm,
            ThighCm = r.ThighCm,
            BodyFatPct = r.BodyFatPct,
            Note = r.Note,
            CreatedAt = now,
        };
        var saved = await measurements.CreateAsync(m);
        return mapper.Map<BodyMeasurementDto>(saved);
    }
}
