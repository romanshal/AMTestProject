using NasaSyncService.Domain.Models;

namespace NasaSyncService.Application.Interfaces.Repositories
{
    public interface IRecclassRepository
    {
        Task<IEnumerable<RecclassDomain>> GetRecclassesAsync(CancellationToken cancellationToken = default);
    }
}
