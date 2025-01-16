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
        return string.IsNullOrEmpty(cacheResponse) ?  null : cacheResponse;
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
