using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Profile.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Profile.Handlers;

public class GetMyProfileQueryHandler(IUserRepository users, IMapper mapper)
    : IRequestHandler<GetMyProfileQuery, MeResponse>
{
    public async Task<MeResponse> Handle(GetMyProfileQuery q, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(q.UserId) ?? throw new KeyNotFoundException("User not found");
        return new MeResponse(mapper.Map<UserDto>(user));
    }
}
