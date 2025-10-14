using AutoMapper;
using MediatR;
using NasaSyncService.Application.Dtos;
using NasaSyncService.Application.Interfaces.Repositories;

namespace NasaSyncService.Application.Features.Meteorites.GetGroupedMeteorites
{
    public class GetGroupedMeteoritesHandler(
        IMeteoriteRepository meteoriteRepository,
        IMapper mapper) : IRequestHandler<GetGroupedMeteoritesQuery, IReadOnlyList<GroupedMeteoritesDto>>
    {
        private readonly IMeteoriteRepository _meteoriteRepository = meteoriteRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IReadOnlyList<GroupedMeteoritesDto>> Handle(GetGroupedMeteoritesQuery request, CancellationToken cancellationToken)
        {
            var group = await _meteoriteRepository.GetGroupedAsync(
                request.YearFrom,
                request.YearTo,
                request.Recclass,
                request.NameContains,
                request.SortBy,
                request.SortOrder,
                cancellationToken);

            var groupDto = _mapper.Map<IReadOnlyList<GroupedMeteoritesDto>>(group);

            return groupDto;
        }
    }
}
