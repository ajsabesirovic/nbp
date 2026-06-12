using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Auth.Commands;

public record LoginCommand(LoginRequest Request) : IRequest<AuthResponse>;
