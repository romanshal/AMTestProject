using Microsoft.Extensions.Logging;

namespace NasaSyncService.Infrastructure.Data.Contexts
{
    public class DataSeedMaker
    {
        /// <summary>
        /// Methd for preconfigured data.
        /// </summary>
        /// <param name="context">Db context</param>
        /// <param name="logger">Logger</param>
        /// <returns></returns>
        public static async Task SeedAsync(NasaDbContext context, ILogger<DataSeedMaker> logger)
        {
        }
    }
}
