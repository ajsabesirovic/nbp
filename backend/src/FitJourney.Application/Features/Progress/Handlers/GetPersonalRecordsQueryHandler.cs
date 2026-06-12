using AutoMapper;
using MediatR;
using FitJourney.Application.DTOs;
using FitJourney.Application.Features.Progress.Queries;
using FitJourney.Domain.Interfaces;
namespace FitJourney.Application.Features.Progress.Handlers;

public class GetPersonalRecordsQueryHandler(IPersonalRecordRepository prs, IMapper mapper)
    : IRequestHandler<GetPersonalRecordsQuery, List<PersonalRecordDto>>
{
    public async Task<List<PersonalRecordDto>> Handle(GetPersonalRecordsQuery q, CancellationToken ct)
    {
        var records = await prs.GetByUserAsync(q.UserId);
        return mapper.Map<List<PersonalRecordDto>>(records);
    }
}
