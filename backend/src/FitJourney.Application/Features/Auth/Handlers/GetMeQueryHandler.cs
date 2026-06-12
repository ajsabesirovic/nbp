using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Auth.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Auth.Handlers;

public class GetMeQueryHandler(IUserRepository users, IMapper mapper) : IRequestHandler<GetMeQuery, UserDto>
{
    public async Task<UserDto> Handle(GetMeQuery q, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(q.UserId) ?? throw new KeyNotFoundException("User not found");
        return mapper.Map<UserDto>(user);
    }
}
