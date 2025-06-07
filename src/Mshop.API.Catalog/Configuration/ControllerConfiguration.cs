using Asp.Versioning.ApiExplorer;
using Elastic.CommonSchema;
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
            /*services.AddSwaggerGen(x =>
            {
                x.OperationFilter<SwaggerDefaultValues>();
            });

            //colcoando versionamento na API
            //precisa instalar 2 pacotes
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                options.ReportApiVersions = true;

            }).AddMvc().AddApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });
            //fim do versionamento

            services.ConfigureOptions<SwaggerConfig>();*/

            return services;
        }

        /*public static WebApplication UseDocumentation(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var version = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                    foreach (var description in version.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }

                });
            }

            return app;
        }*/
    }
}
