using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NasaSyncService.Application.Interfaces.Repositories;
using NasaSyncService.Domain.Models;
using NasaSyncService.Infrastructure.Data.Contexts;

namespace NasaSyncService.Infrastructure.Repositories
{
    public class RecclassRepository(
        NasaDbContext context,
        IMapper mapper) : IRecclassRepository
    {
        private readonly NasaDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<RecclassDomain>> GetRecclassesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Recclasses
                .AsNoTracking()
                .ProjectTo<RecclassDomain>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}
