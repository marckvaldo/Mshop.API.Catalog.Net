using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.Service;
using Mshop.Domain.Contract.Services;
using Serilog;
using CoreMessage = Mshop.Core.Message;

namespace Mshop.Application
{
    public static class ServiceResgistrationExtensions
    {
        public static IServiceCollection AddUseCase(this IServiceCollection services)
        {
            services.AddScoped<IBuildCacheCategory, BuildCacheCategory>();
            services.AddScoped<IBuildCacheImage, BuildCacheImage>();
            services.AddScoped<IBuildCacheProduct, BuildCacheProduct>();

            services.AddScoped<CoreMessage.INotification, CoreMessage.Notifications>();
            services.AddScoped<IStorageService, StorageService>();

            services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(ServiceResgistrationExtensions).Assembly));

            return services;
        }

        public static IServiceCollection AddConfigurationSeriLog(this IServiceCollection services, IConfiguration configuration)
        {
            var elasticUri = configuration["Elasticsearch:Uri"];
            var elasticUsername = configuration["Elasticsearch:Username"];
            var elasticPassword = configuration["Elasticsearch:Password"];

            /*var ElasticsearchConfiguration = new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri($"http://{elasticUsername}:{elasticPassword}@{elasticUri}"))
            {
                AutoRegisterTemplate = true,
                IndexFormat = "logsaplication-{0:yyyy.MM.dd}",
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
            };

            Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithProperty("Aplication","Catalog")
                .WriteTo.Elasticsearch(ElasticsearchConfiguration)
                .CreateBootstrapLogger();*/

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()                
                .WriteTo.Console()
                .WriteTo.Elasticsearch([new Uri($"http://{elasticUri}")], Opt =>
                {
                    Opt.BootstrapMethod = Elastic.Ingest.Elasticsearch.BootstrapMethod.Failure;
                    Opt.DataStream = new Elastic.Ingest.Elasticsearch.DataStreams.DataStreamName("logs", "aplication");
                    Opt.ConfigureChannel = channelOpts => {
                        channelOpts.BufferOptions = new Elastic.Channels.BufferOptions { ExportMaxConcurrency = 2 };
                    };

                }, transport =>
                {
                    transport.Authentication(new BasicAuthentication(elasticUsername, elasticPassword));
                })
                //aqui eu estou falando para ignorar as querys nos meus logs
                .Filter.ByExcluding(logEvent =>
                    // Filtra logs relacionados ao Entity Framework Core (Database.Command)
                    logEvent.Properties.ContainsKey("SourceContext") &&
                    logEvent.Properties["SourceContext"].ToString().Contains("Microsoft.EntityFrameworkCore.Database.Command") ||

                    // Filtra logs relacionados ao CacheController
                    logEvent.Properties.ContainsKey("SourceContext") &&
                    logEvent.Properties["SourceContext"].ToString().Contains("Mshop.API.Catalog.Controllers.CacheController") ||

                    // Filtra logs de execução de OkObjectResult (executando ação do controller)
                    logEvent.MessageTemplate.Text.Contains("Executing OkObjectResult") ||
                    logEvent.MessageTemplate.Text.Contains("Executing OkObjectResult, writing value of type ") ||
                    // Filtra logs relacionados a mensagens de "writing value of type"
                    logEvent.MessageTemplate.Text.Contains("writing value of type") ||

                    // Filtra logs com a rota e execução de controller
                    logEvent.MessageTemplate.Text.Contains("Route matched with") ||
                    logEvent.MessageTemplate.Text.Contains("Executing controller action")
                    )
                .CreateLogger();

            return services;
        }

        public static WebApplication AddLayoutSerilog(this WebApplication app)
        {
            app.UseSerilogRequestLogging(opt =>
            {
                opt.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                opt.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHeaders", httpContext.Request.Headers);
                };
            });

            return app;
        }
    }
}
