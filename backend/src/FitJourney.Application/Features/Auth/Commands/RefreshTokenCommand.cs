using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string Token) : IRequest<AuthResponse>;
