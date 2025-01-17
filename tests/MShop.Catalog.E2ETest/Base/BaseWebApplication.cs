using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Test.Common;
using Mshop.Infra.Data.Context;
using MShop.Catalog.E2ETest.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MShop.Catalog.E2ETests.Base
{
    public class BaseWebApplication : BaseFixture
    {
        protected CustomerWebApplicationFactory<Program> _webApp;
        protected IServiceProvider _serviceProvider;
        protected HttpClient _httpClient;
        protected APIClient _apiClient;
        protected BaseWebApplication() : base()
        {
           _webApp = new CustomerWebApplicationFactory<Program>();
           _serviceProvider = _webApp.Services.GetRequiredService<IServiceProvider>();
           _httpClient = _webApp.CreateClient();
           _apiClient = new APIClient(_httpClient);
        }

        protected async Task DeleteDataBase(RepositoryDbContext dbContext, bool disposeDBContext = true)
        {

            if (dbContext != null)
                await dbContext.Database.EnsureDeletedAsync();

            if (disposeDBContext && dbContext != null)
                await dbContext.DisposeAsync();
        }

        protected async Task AddMigartion(RepositoryDbContext dbContext)
        {
            if (dbContext != null)
                await Task.Run(() => dbContext.Database.Migrate());
        }
    }

}
