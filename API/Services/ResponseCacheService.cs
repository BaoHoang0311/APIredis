using System;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace API.Services;

public class ResponseCacheService : IResponseCacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    public ResponseCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
    {
        _distributedCache = distributedCache;
        _connectionMultiplexer = connectionMultiplexer;
    }
    public async Task<string> GetCacheResponseAsync(string cacheKey)
    {
        var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);
        return string.IsNullOrEmpty(cacheResponse) ? null : cacheResponse;
    }

    public async Task RemoveCacheResponseAsync(string partern)
    {
        if (string.IsNullOrWhiteSpace(partern))
            throw new ArgumentException("Value cannot be null or whitespace");

        await foreach (var key in GetkeyAsync(partern+"*"))
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
    private async IAsyncEnumerable<string> GetkeyAsync(string pattern)
    {
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException("Value cannot be null or whitespace");
            foreach (var endPoint in _connectionMultiplexer.GetEndPoints())
            {
                var servers = _connectionMultiplexer.GetServer(endPoint);
                foreach (var key in servers.Keys(pattern:pattern))
                {
                    yield return key.ToString();
                }
            }
        }
    }
    public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut)
    {
        if (response == null) return;
        var serializeResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        });
        await _distributedCache.SetStringAsync(cacheKey, serializeResponse, new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = timeOut,
        });
    }
}
