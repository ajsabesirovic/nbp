using MediatR;
using FitJourney.Application.DTOs;
namespace FitJourney.Application.Features.Progress.Queries;

public record GetPersonalRecordsQuery(string UserId) : IRequest<List<PersonalRecordDto>>;
