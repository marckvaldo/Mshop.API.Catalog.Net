using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.Respository;
using StackExchange.Redis;

namespace Mshop.Infra.Cache
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisPassword = configuration["Redis:Password"];
            var redisEndPoint = configuration["Redis:Endpoint"];
            var redisUser = configuration["Redis:User"];

            var conf = new ConfigurationOptions
            {
                EndPoints = { redisEndPoint },
                User = redisUser,
                Password = redisPassword
            };

            var redis = ConnectionMultiplexer.Connect(conf);
            services.AddSingleton<IConnectionMultiplexer>(redis);
            services.AddSingleton<StartIndex.StartIndex>();
            return services;
        }

        public static IServiceCollection AddRepositoryCache(this IServiceCollection services)
        {
            services.AddScoped<IProductCacheRepository, ProductCacheRepository>();
            services.AddScoped<ICategoryCacheRepository, CategoryCacheRepository>();    
            services.AddScoped<IImagesCacheRepository, ImagesCacheRepository>();    

            return services;
        }

        public static WebApplication CrateIndexRedis(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var redisService = scope.ServiceProvider.GetRequiredService<StartIndex.StartIndex>();
            redisService.CreateIndex().Wait();
            return app;
        }
    }
}
