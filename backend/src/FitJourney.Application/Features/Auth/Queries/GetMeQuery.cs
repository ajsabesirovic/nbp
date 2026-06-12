using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Auth.Queries;

public record GetMeQuery(string UserId) : IRequest<UserDto>;
