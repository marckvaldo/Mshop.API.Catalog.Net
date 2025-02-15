using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.UseCases.Cache.Products.ListProductCache;
using Mshop.Application.UseCases.Product.ListProducts;
using Mshop.Core.Data;
using Mshop.Core.Enum.Paginated;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;

namespace Mshop.IntegrationTests.Application.UserCases.Cache.Product.ListProduct
{
    [Collection("List Products Collection")]
    [CollectionDefinition("List Products Collection", DisableParallelization = true)]
    public class ListProductCacheTest : IntegracaoBaseFixture, IDisposable
    {

        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly IImagesCacheRepository _imagesCacheRepository;
        private readonly IProductCacheRepository _productsCacheRepository;  
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;
        private readonly StartIndex _startIndex;
        private readonly StackExchange.Redis.IDatabase _database;
        private readonly IBuildCacheImage _buildCacheImage;
        private readonly IBuildCacheProduct _buildCacheProduct;

        public ListProductCacheTest() : base()
        {
            var connection = _serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
            _database = connection.GetDatabase();

            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _imageRepository = _serviceProvider.GetRequiredService<IImageRepository>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>();
            _productsCacheRepository = _serviceProvider.GetRequiredService<IProductCacheRepository>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _storageService = _serviceProvider.GetRequiredService<IStorageService>();
            _notification = _serviceProvider.GetRequiredService<INotification>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();
            _buildCacheImage = _serviceProvider.GetRequiredService<IBuildCacheImage>();
            _buildCacheProduct = _serviceProvider.GetRequiredService<IBuildCacheProduct>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();

            DeleteCache(_database).Wait();
            _startIndex.CreateIndex().Wait();
        }

        [Fact(DisplayName = nameof(ListProductsCache))]
        [Trait("Integration - Application.UseCase.Cache", "Product Use Case")]

        public async Task ListProductsCache()
        {
            var category = FakerCategory();
            await _categoryRepository.Create(category,CancellationToken.None);

            var productsFake = FakerProducts(20,category);
            foreach (var product in productsFake)
            {
                await _productRepository.Create(product,CancellationToken.None);
            }

            await _unitOfWork.CommitAsync();

            var useCase = new ListProductCache(_productRepository, _productsCacheRepository, _buildCacheProduct, _notification);
            var request = new ListProductCacheInPut(
                            page: 1,
                            perPage:5,
                            search: "",
                            sort: "name",
                            dir: Mshop.Core.Enum.Paginated.SearchOrder.Asc,
                            false
                            );

            var outPut = await useCase.Handle(request, CancellationToken.None);
            Thread.Sleep(2000);
            var productsCache = await _productsCacheRepository.FilterPaginatedQuery(
                new Core.Paginated.PaginatedInPut(
                    page: 1, 
                    perPage: 20, 
                    search: "", 
                    orderBy: "", 
                    SearchOrder.Desc),Guid.Empty,false,
                CancellationToken.None
                );

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(productsFake.Count, result.Total);
            Assert.Equal(request.Page, result.Page);
            Assert.Equal(request.PerPage, result.PerPage);
            Assert.NotNull(result.Itens);
            Assert.True(result.Itens.Any());
            Assert.NotNull(productsCache);
            Assert.Equal(20,productsCache.Total);
        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
