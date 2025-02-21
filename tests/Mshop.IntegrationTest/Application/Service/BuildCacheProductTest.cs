using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Enum.Paginated;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ServiceBuilder = Mshop.Application.Service;

namespace Mshop.IntegrationTests.Application.Service
{
    public class BuildCacheProductTest : IntegracaoBaseFixture
    {
        private IProductCacheRepository _productCacheRepository;
        private IProductRepository _productRepository;
        private IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;
        private readonly StartIndex _startIndex;
        private readonly StackExchange.Redis.IDatabase _database;

        public BuildCacheProductTest() : base()
        {
            var connection = _serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
            _database = connection.GetDatabase();

            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _productCacheRepository = _serviceProvider.GetRequiredService<IProductCacheRepository>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();

            DeleteCache(_database).Wait();
            _startIndex.CreateIndex().Wait();

        }

        [Fact(DisplayName = nameof(ShouldBuilderCacheProduct))]
        [Trait("Integration - Application.Service", "Builder Product Cache")]

        public async Task ShouldBuilderCacheProduct()
        {
            var category = FakerCategory();
            var listCategories = FakerProducts(50, category);

            foreach (var product in listCategories)
            {
                await _productRepository.Create(product, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();

            var service = new ServiceBuilder.BuildCacheProduct(_serviceProvider);
            await service.Handle();

            var products = await _productCacheRepository.FilterPaginatedQuery(
                                        new Core.Paginated.PaginatedInPut(
                                            currentPage: 1,
                                            perPage: 50,
                                            search: "",
                                            orderBy: "",
                                            order: SearchOrder.Desc),Guid.Empty, false,
                                        CancellationToken.None);

            Assert.NotNull(products.Data);
            Assert.Equal(50, products.Data.Count());
        }



    }
}
