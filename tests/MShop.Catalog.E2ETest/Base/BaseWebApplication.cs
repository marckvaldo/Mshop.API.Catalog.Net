using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Test.Common;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Catalog.E2ETest.Base;
using StackExchange.Redis;

namespace Mshop.Catalog.E2ETests.Base
{
    public class BaseWebApplication : BaseFixture
    {
        protected CustomerWebApplicationFactory<Mshop.API.Catalog.Program> _webApp;
        protected GrpcWebApplicationFactory<Mshop.gRPC.Catalog.Program> _webAppGrpc;
        protected IServiceProvider _serviceProvider;
        protected HttpClient _httpClient;
        protected APIClient _apiClient;
        protected GrpcClient _grpcClient;
        protected BaseWebApplication(TypeProjetct typeProjetct = TypeProjetct.Http) : base()
        {
            /*_webApp = new CustomerWebApplicationFactory<Program>();
            _serviceProvider = _webApp.Services.GetRequiredService<IServiceProvider>();
            _httpClient = _webApp.CreateClient();
            _apiClient = new APIClient(_httpClient);*/

            if(typeProjetct == TypeProjetct.Http)
                BuildWebAplicationAPI().Wait(); 

            if(typeProjetct == TypeProjetct.Grpc)
                BuildWebAplicationGrpc().Wait();
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

        protected async Task DeleteIndexCache(IDatabase database)
        {
            string indexNameCategory = $"{IndexName.Category}Index";
            await database.ExecuteAsync("FT.DROPINDEX", indexNameCategory, "DD");

            string indexNameProduct = $"{IndexName.Product}Index";
            await database.ExecuteAsync("FT.DROPINDEX", indexNameProduct, "DD");

            string indexNameImage = $"{IndexName.Image}Index";
            await database.ExecuteAsync("FT.DROPINDEX", indexNameImage, "DD");

        }

        protected async Task CreateIndexCahce(StartIndex startIndex)
        {
            await startIndex.CreateIndex();
        }

        protected async Task BuildWebAplicationAPI()
        {
            _webApp = new CustomerWebApplicationFactory<Mshop.API.Catalog.Program>();
            _serviceProvider = _webApp.Services.GetRequiredService<IServiceProvider>();
            _httpClient = _webApp.CreateClient();
            _apiClient = new APIClient(_httpClient);
        }

        protected async Task BuildWebAplicationGrpc()
        {
            _webAppGrpc = new GrpcWebApplicationFactory<Mshop.gRPC.Catalog.Program>();
            _serviceProvider = _webAppGrpc.Services.GetRequiredService<IServiceProvider>();
            _httpClient = _webAppGrpc.CreateClient();
            _grpcClient = new GrpcClient(_httpClient);
        }
    }

}
