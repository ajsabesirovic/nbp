using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Profile.Commands;
using FitJourney.Domain.Entities;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Profile.Handlers;

public class UpdateMyProfileCommandHandler(IUserRepository users, IMapper mapper)
    : IRequestHandler<UpdateMyProfileCommand, MeResponse>
{
    public async Task<MeResponse> Handle(UpdateMyProfileCommand cmd, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(cmd.UserId) ?? throw new KeyNotFoundException("User not found");

        if (!string.IsNullOrWhiteSpace(cmd.Request.Name)) user.Name = cmd.Request.Name.Trim();

        if (cmd.Request.Profile != null)
        {
            user.Profile ??= new UserProfile();
            var p = cmd.Request.Profile;
            user.Profile.Gender = p.Gender;
            user.Profile.DateOfBirth = p.DateOfBirth;
            user.Profile.HeightCm = p.HeightCm;
            user.Profile.CurrentWeightKg = p.CurrentWeightKg;
            user.Profile.TargetWeightKg = p.TargetWeightKg;
            user.Profile.Experience = p.Experience;
            user.Profile.Goal = p.Goal;
        }

        user.UpdatedAt = DateTime.UtcNow;
        var saved = await users.UpdateAsync(user);
        return new MeResponse(mapper.Map<UserDto>(saved));
    }
}
