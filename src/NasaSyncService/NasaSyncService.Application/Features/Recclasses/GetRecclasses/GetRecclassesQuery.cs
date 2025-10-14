using MediatR;
using NasaSyncService.Application.Dtos;

namespace NasaSyncService.Application.Features.Recclasses.GetRecclasses
{
    public record GetRecclassesQuery : IRequest<IEnumerable<RecclassDto>>
    {
    }
}
