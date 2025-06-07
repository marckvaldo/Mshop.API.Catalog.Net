using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message.DomainEvent;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.Infra.Data.Repository;

namespace Mshop.Infra.Data
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddDataBaseAndRepository(this IServiceCollection services, IConfiguration configuration)
        {
            var ConnectionString = configuration.GetConnectionString("RepositoryMysql");
            services.AddDbContext<RepositoryDbContext>(options =>
                options.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString),
                b=>b.MigrationsAssembly("Mshop.Infra.Data"))
                //.EnableDetailedErrors()
                //.EnableSensitiveDataLogging(), 
                //ServiceLifetime.Scoped
                );

            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<ICategoryRepository,CategoryRepository>();
            services.AddScoped<IProductRepository,ProductRepository>();
            services.AddScoped<IImageRepository, ImagesRepository>();


            return services;
        }


        public static WebApplication AddMigrateDatabase(this WebApplication app)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == "EndToEndTest") return app;

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();
            dbContext.Database.Migrate();
            return app;
        }
    }
}
