using System;
using System.Text;
using API.Configuration;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Attributes;
public class CacheAttribute : Attribute, IAsyncActionFilter
{
    private readonly int _timeToLiveSeconds;
    public CacheAttribute(int timeToLiveSecond)
    {
        _timeToLiveSeconds = timeToLiveSecond;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Xem cache có chưa
        var cacheConfiguration = context.HttpContext.RequestServices.GetRequiredService<RedisConfiguration>();
        var zzz = cacheConfiguration.Enabled;
        if (!cacheConfiguration.Enabled)
        {
            await next();
            return;
        }
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
        var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
        var cacheResponse  =  await cacheService.GetCacheResponseAsync(cacheKey);

        if(!string.IsNullOrEmpty(cacheResponse)){
            var contentResult = new ContentResult(){
                Content = cacheResponse,
                ContentType = "application/json",
                StatusCode = 200
            };
            context.Result =  contentResult;
            return;
        }
        var excutedContext = await next();

        if (excutedContext.Result is OkObjectResult objectResult){
            await cacheService.SetCacheResponseAsync(cacheKey, objectResult.Value, TimeSpan. FromSeconds(_timeToLiveSeconds));
        }
    }
    private static string GenerateCacheKeyFromRequest(HttpRequest request)
    {
        var keyBuilder = new StringBuilder();
        keyBuilder.Append($"{request.Path}");
        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        {
            keyBuilder.Append($"|{key}-{value}");
        }
        return keyBuilder.ToString();
    }

}