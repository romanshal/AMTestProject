namespace NasaSyncService.Application.Dtos
{
    public sealed record GroupedMeteoritesDto(
        int? Year,
        int Count,
        decimal? TotalMass);
}
