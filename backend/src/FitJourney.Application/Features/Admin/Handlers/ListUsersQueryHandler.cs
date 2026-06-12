using AutoMapper;
using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Admin.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Admin.Handlers;

public class ListUsersQueryHandler(IUserRepository users, IMapper mapper)
    : IRequestHandler<ListUsersQuery, PagedResult<AdminUserDto>>
{
    public async Task<PagedResult<AdminUserDto>> Handle(ListUsersQuery q, CancellationToken ct)
    {
        var (items, total) = await users.ListAsync(q.Search, q.Role, q.Page, q.Limit);
        return new PagedResult<AdminUserDto>(mapper.Map<List<AdminUserDto>>(items), total, q.Page, q.Limit);
    }
}
