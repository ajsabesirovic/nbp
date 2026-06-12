using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Profile.Queries;

public record GetMyProfileQuery(string UserId) : IRequest<MeResponse>;
