using System.Collections.Concurrent;
using TinyUrlService.Domain.Entities;

namespace TinyUrlService.Domain.Services
{
    public interface IUrlService
    {
        Task<string> CreateShortUrlAsync(string longUrl, string shortUrl = null, CancellationToken cancellationToken = default);
        Task<UrlMapping> GetLongUrlAsync(string shortUrl, CancellationToken cancellationToken = default);
        Task<bool> DeleteShortUrlAsync(string shortUrl, CancellationToken cancellationToken = default);
        Task<Dictionary<string, UrlMapping>> GetStatisticsAsync(CancellationToken cancellationToken = default);
    }

    public class UrlService : IUrlService
    {
        private readonly ConcurrentDictionary<string, UrlMapping> _urlMappings = new ConcurrentDictionary<string, UrlMapping>();

        public async Task<string> CreateShortUrlAsync(string longUrl, string shortUrl = null, CancellationToken cancellationToken = default)
        {
           _ = longUrl ?? throw new ArgumentNullException(nameof(longUrl));

            var setShortUrl = string.IsNullOrEmpty(shortUrl) ? Guid.NewGuid().ToString().Substring(0, 8) : shortUrl;

            if (_urlMappings.TryGetValue(setShortUrl, out var existingMapping))
            {
                if (existingMapping.LongUrl == longUrl)
                {
                    return await Task.FromResult(setShortUrl);
                }
                else
                {
                    throw new InvalidOperationException("Short URL already exists with a different long URL");
                }
            }

            _urlMappings.TryAdd(setShortUrl, new UrlMapping { ShortUrl = setShortUrl, LongUrl = longUrl, ClickCount = 0 });

            return await Task.FromResult(setShortUrl);
        }

        public async Task<UrlMapping> GetLongUrlAsync(string shortUrl, CancellationToken cancellationToken = default)
        {
            _ = shortUrl ?? throw new ArgumentNullException(nameof(shortUrl));

            if (_urlMappings.TryGetValue(shortUrl, out var mapping))
            {
                mapping.ClickCount++;
                return await Task.FromResult(mapping);
            }
            throw new KeyNotFoundException("Short URL not found");
        }

        public async Task<bool> DeleteShortUrlAsync(string shortUrl, CancellationToken cancellationToken = default)
        {
            _ = shortUrl ?? throw new ArgumentNullException(nameof(shortUrl));

            return await Task.FromResult(_urlMappings.TryRemove(shortUrl, out _));
        }

        public async Task<Dictionary<string, UrlMapping>> GetStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_urlMappings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }
    }
}
