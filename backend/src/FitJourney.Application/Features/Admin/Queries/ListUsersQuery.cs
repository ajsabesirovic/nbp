using MediatR;
using FitJourney.Application.Common;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Admin.Queries;

public record ListUsersQuery(string? Search, string? Role, int Page, int Limit) : IRequest<PagedResult<AdminUserDto>>;
