using Microsoft.AspNetCore.Mvc;

namespace Mshop.API.Catalog.Configuration
{
    public static class ConfigurationModelState
    {
        public static IServiceCollection AddConfigurationModelState(this IServiceCollection services)
        {
            //desativa o modelStateInvalid na controller automatizado
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            return services;
        }
    }
}
