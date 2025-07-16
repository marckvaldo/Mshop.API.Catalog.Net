using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Mshop.API.Catalog.Filter;
using HealthChecks.UI.Client;
using Mshop.API.Catalog.HealChecks;

namespace Mshop.API.Catalog.Configuration
{
    public static class ConfigurationHeathCheck
    {
        public static IServiceCollection AddConfigurationHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<HealthDataBase>("DataBase")
                .AddCheck<HealthCache>("Cache");

            return services;
        }

        public static WebApplication AddMapHealthCheck(this WebApplication app)
        {
            app.MapHealthChecks("/_metrics", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

            });
            return app;
        }
    }
}
