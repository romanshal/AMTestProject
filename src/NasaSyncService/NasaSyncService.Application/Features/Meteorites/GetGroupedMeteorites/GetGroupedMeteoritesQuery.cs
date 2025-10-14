using MediatR;
using NasaSyncService.Application.Dtos;

namespace NasaSyncService.Application.Features.Meteorites.GetGroupedMeteorites
{
    public record GetGroupedMeteoritesQuery(
        int? YearFrom,
        int? YearTo,
        Guid? Recclass,
        string? NameContains,
        string SortBy, // "year" | "count" | "mass"
        bool SortOrder) : IRequest<IReadOnlyList<GroupedMeteoritesDto>>;
}
