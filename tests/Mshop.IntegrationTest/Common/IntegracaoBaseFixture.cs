using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Application;
using Mshop.Core.Test.Common;
using Mshop.Infra.Cache;
using Mshop.Infra.Data;
using Mshop.Infra.Data.Context;
using Mshop.IntegrationTests.Infra.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.IntegrationTests.Common
{
    public abstract class IntegracaoBaseFixture : BaseFixture
    {
        protected IConfiguration _configuration;

        protected IServiceProvider _serviceProvider;

        public IntegracaoBaseFixture() : base()
        {
            _serviceProvider = BuilderProvider();
        }
        protected IServiceProvider BuilderProvider()
        {
            var service = new ServiceCollection();

            var inMemorySettings = new Dictionary<string, string>()
            {
                {"ConnectionStrings:RepositoryMysql", ConfigurationTests.ConnectionStrings },
                {"Redis:Endpoint",ConfigurationTests.RedisEndpoint },
                {"Redis:User",ConfigurationTests.RedisUser },
                {"Redis:Password",ConfigurationTests.RedisPasseword }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            service.AddDataBaseAndRepository(configuration)
                .AddCache(configuration)
                .AddRepositoryCache()
                .AddUseCase();

            return service.BuildServiceProvider();
        }

        protected async Task AddMigartion(RepositoryDbContext dbContext)
        {
            if (dbContext != null)
                await Task.Run(() => dbContext.Database.Migrate());
        }

        protected async Task DeleteDataBase(RepositoryDbContext dbContext, bool disposeDBContext = true)
        {
            
                if (dbContext != null)
                    await dbContext.Database.EnsureDeletedAsync();

                if (disposeDBContext && dbContext != null)
                    await dbContext.DisposeAsync();
        }

        protected async Task DeleteCache(StackExchange.Redis.IDatabase database)
        {
            await database.ExecuteAsync("FLUSHALL");
        }

        protected (IServiceScope scope, T ServiceInstance) CreateScopedService<T>() where T : class
        {
            var scope = _serviceProvider.CreateScope();
            var serviceInstance = scope.ServiceProvider.GetRequiredService<T>();
            return (scope, serviceInstance);
        }
    }
}
