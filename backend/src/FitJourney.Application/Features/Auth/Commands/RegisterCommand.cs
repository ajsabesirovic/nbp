using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Auth.Commands;

public record RegisterCommand(RegisterRequest Request) : IRequest<AuthResponse>;
