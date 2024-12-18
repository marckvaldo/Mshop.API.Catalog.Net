using Mshop.API.Catalog.Filter;
using System.Text.Json.Serialization;

namespace Mshop.API.Catalog.Configuration
{
    public static class ControllerConfiguration
    {
        public static IServiceCollection AddConfigurationController(this IServiceCollection services)
        {
            services.AddControllers(
                options => options.Filters.Add(typeof(ApiGlobalExceptionFilter))
                )
                .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }

        public static WebApplication UseDocumentation(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            return app;
        }
    }
}
