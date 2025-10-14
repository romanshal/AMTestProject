using Microsoft.EntityFrameworkCore;
using NasaSyncService.Application.Interfaces.Repositories;
using NasaSyncService.Domain.Models;
using NasaSyncService.Infrastructure.Data.Contexts;

namespace NasaSyncService.Infrastructure.Repositories
{
    internal class MeteoriteRepository(
        NasaDbContext context) : IMeteoriteRepository
    {
        private readonly NasaDbContext _context = context;

        /// <summary>
        /// Get grouped meteorites by parameters.
        /// </summary>
        /// <param name="yearFrom">Year from</param>
        /// <param name="yearTo">Year to</param>
        /// <param name="recclassId">Recclass id</param>
        /// <param name="nameContains">Meteorite name</param>
        /// <param name="sortBy">Sort</param>
        /// <param name="ascending">Order</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of grouped meteorites.</returns>
        public async Task<IEnumerable<GroupedMeteoriteDomain>> GetGroupedAsync(
            int? yearFrom,
            int? yearTo,
            Guid? recclassId,
            string? nameContains,
            string sortBy, // "year" | "count" | "mass"
            bool ascending = true,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Meteorites.AsQueryable();

            if (yearFrom.HasValue)
                query = query.Where(m => m.YearUtc.HasValue && m.YearUtc.Value.Year >= yearFrom.Value);

            if (yearTo.HasValue)
                query = query.Where(m => m.YearUtc.HasValue && m.YearUtc.Value.Year <= yearTo.Value);

            if (recclassId.HasValue)
                query = query.Where(m => m.Recclass.ClassId == recclassId);

            if (!string.IsNullOrWhiteSpace(nameContains))
                query = query.Where(m => m.Name.Contains(nameContains));

            var groupedQuery = query
                .GroupBy(m => m.YearUtc.HasValue ? m.YearUtc.Value.Year : (int?)null)
                .Select(g => new GroupedMeteoriteDomain
                {
                    Year = g.Key,
                    Count = g.Count(),
                    TotalMass = g.Sum(x => x.MassGram.Value)
                });

            groupedQuery = sortBy.ToLower() switch
            {
                "count" => ascending
                    ? groupedQuery.OrderBy(g => g.Count)
                    : groupedQuery.OrderByDescending(g => g.Count),

                "mass" => ascending
                    ? groupedQuery.OrderBy(g => g.TotalMass)
                    : groupedQuery.OrderByDescending(g => g.TotalMass),

                _ => ascending
                    ? groupedQuery.OrderBy(g => g.Year)
                    : groupedQuery.OrderByDescending(g => g.Year)
            };

            return await groupedQuery.ToListAsync(cancellationToken);
        }
    }
}
