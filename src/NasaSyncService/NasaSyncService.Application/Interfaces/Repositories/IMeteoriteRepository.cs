using NasaSyncService.Domain.Models;

namespace NasaSyncService.Application.Interfaces.Repositories
{
    public interface IMeteoriteRepository
    {

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
        Task<IEnumerable<GroupedMeteoriteDomain>> GetGroupedAsync(
            int? yearFrom,
            int? yearTo,
            Guid? recclassId,
            string? nameContains,
            string sortBy, // "year" | "count" | "mass"
            bool ascending = true,
            CancellationToken cancellationToken = default);
    }
}
