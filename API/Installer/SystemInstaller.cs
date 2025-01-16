using API.Configuration;
using API.Installer;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using API.Services;

namespace API.InstallerExtension;

public static class SystemInstaller 
{
    public static void Installer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        services.AddSwaggerGen();

        var redisConfiguration =  new RedisConfiguration();
        configuration.GetSection("RedisConfiguration").Bind(redisConfiguration);
        services.AddSingleton(redisConfiguration);
        
        if(!redisConfiguration.Enabled) 
            return;
        
        services.AddSingleton<IConnectionMultiplexer>(_=>ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString));
        services.AddStackExchangeRedisCache(option => option.Configuration = redisConfiguration.ConnectionString);
        services.AddSingleton<IResponseCacheService,ResponseCacheService>();
        
    }
}
