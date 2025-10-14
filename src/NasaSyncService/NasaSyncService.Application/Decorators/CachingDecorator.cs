using MediatR;
using NasaSyncService.Application.Interfaces.Cache;

namespace NasaSyncService.Application.Decorators
{
    public class CachingDecorator<TRequest, TResponse>(
        IRequestHandler<TRequest, TResponse> inner,
        ICacheService cache) : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner = inner;
        private readonly ICacheService _cache = cache;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var key = $"{typeof(TRequest).Name}:{request.GetHashCode()}";

            var cached = await _cache.GetAsync<TResponse>(key);
            if (cached is not null)
                return cached;

            var result = await _inner.Handle(request, cancellationToken);

            await _cache.SetAsync(key, result);

            return result;
        }
    }
}
