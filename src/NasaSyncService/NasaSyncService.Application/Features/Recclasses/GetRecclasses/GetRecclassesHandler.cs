using AutoMapper;
using MediatR;
using NasaSyncService.Application.Dtos;
using NasaSyncService.Application.Interfaces.Repositories;

namespace NasaSyncService.Application.Features.Recclasses.GetRecclasses
{
    internal class GetRecclassesHandler(
        IRecclassRepository recclassRepository,
        IMapper mapper) : IRequestHandler<GetRecclassesQuery, IEnumerable<RecclassDto>>
    {
        private readonly IRecclassRepository _recclassRepository = recclassRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<RecclassDto>> Handle(GetRecclassesQuery request, CancellationToken cancellationToken)
        {
            var reclasses = await _recclassRepository.GetRecclassesAsync(cancellationToken);

            var recclassesDto = _mapper.Map<IEnumerable<RecclassDto>>(reclasses);

            return recclassesDto;
        }
    }
}
