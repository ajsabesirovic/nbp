using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Profile.Commands;

public record UpdateMyProfileCommand(string UserId, UpdateProfileRequest Request) : IRequest<MeResponse>;
