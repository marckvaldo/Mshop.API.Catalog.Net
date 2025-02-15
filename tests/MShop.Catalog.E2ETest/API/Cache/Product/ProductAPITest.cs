using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.UseCases.Cache.Products.GetProductCache;
using Mshop.Application.UseCases.Cache.Products.ListProductByCategoryCache;
using Mshop.Application.UseCases.Cache.Products.ListProductCache;
using Mshop.Application.UseCases.Cache.Products.ListProductsPromotionCache;
using Mshop.Catalog.E2ETest.Base;
using Mshop.Catalog.E2ETests.API.Common;
using Mshop.Core.Data;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using StackExchange.Redis;

namespace Mshop.Catalog.E2ETests.API.Cache.Product
{
    [Collection("Crud Products Collection")]
    [CollectionDefinition("Crud Products Collection", DisableParallelization = true)]

    public class ProductAPITest : ProductAPITestFixture, IDisposable
    {
        private ICategoryRepository _categoryRepository;
        private ICategoryCacheRepository _categoryCacheRepository;
        private IProductRepository _productRepository;
        private IProductCacheRepository _productCacheRepository;
        private IConnectionMultiplexer _database;
        private IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;
        private readonly DateTime _expirationDate;
        private readonly StartIndex _startIndex;

        public ProductAPITest() : base()
        {
            _categoryCacheRepository = _serviceProvider.GetRequiredService<ICategoryCacheRepository>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _productCacheRepository = _serviceProvider.GetRequiredService<IProductCacheRepository>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>(); 
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _database = _serviceProvider.GetRequiredService<IConnectionMultiplexer>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();
            _expirationDate = DateTime.UtcNow.AddMinutes(1);

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();


            DeleteIndexCache(_database.GetDatabase()).Wait();
            CreateIndexCahce(_startIndex).Wait();
        }

       
        [Fact(DisplayName = nameof(GetProductWithCacheById))]
        [Trait("EndToEnd/API", "Cache-Product - Endpoints")]
        public async Task GetProductWithCacheById()
        {
            var product = FakerProduct(FakerCategory());
            await _productRepository.Create(product, CancellationToken.None);
            await _unitOfWork.CommitAsync();    

            var productCacheNull = await _productCacheRepository.GetById(product.Id);

            var (response, outPut) = await _apiClient.Get<CustomResponse<GetProductCacheOutPut>>($"{Configuration.URL_API_CACHE}product-cache/{product.Id}");
            Thread.Sleep(2000);
            var productCacheNotNull = await _productCacheRepository.GetById(product.Id);

            Assert.Null(productCacheNull);
            Assert.NotNull(productCacheNotNull);
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(product.Id, outPut.Data.Id);
            Assert.Equal(product.Name, outPut.Data.Name);
            Assert.Equal(product.Description, outPut.Data.Description);
            Assert.Equal(product.Price, outPut.Data.Price);
            //Assert.Equal(product.Thumb.Path, outPut.Data.Imagem);
            Assert.Equal(product.CategoryId, outPut.Data.CategoryId);
            Assert.Equal(product.Stock, outPut.Data.Stock);
            Assert.Equal(product.IsActive, outPut.Data.IsActive);
            Assert.Equal(product.CategoryId, outPut.Data.CategoryId);
    
        }


        [Fact(DisplayName = nameof(GetProductWithOutCacheById))]
        [Trait("EndToEnd/API", "Cache-Product - Endpoints")]
        public async Task GetProductWithOutCacheById()
        {
            var product = FakerProduct(FakerCategory());
            await _productCacheRepository.Create(product, _expirationDate, CancellationToken.None);

            var productCacheNotNull = await _productCacheRepository.GetById(product.Id);

            var (response, outPut) = await _apiClient.Get<CustomResponse<GetProductCacheOutPut>>($"{Configuration.URL_API_CACHE}product-cache/{product.Id}");
            Thread.Sleep(2000);
            productCacheNotNull = await _productCacheRepository.GetById(product.Id);

            Assert.NotNull(productCacheNotNull);
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(product.Id, outPut.Data.Id);
            Assert.Equal(product.Name, outPut.Data.Name);
            Assert.Equal(product.Description, outPut.Data.Description);
            Assert.Equal(product.Price, outPut.Data.Price);
            //Assert.Equal(product.Thumb.Path, outPut.Data.Imagem);
            Assert.Equal(product.CategoryId, outPut.Data.CategoryId);
            Assert.Equal(product.Stock, outPut.Data.Stock);
            Assert.Equal(product.IsActive, outPut.Data.IsActive);
            Assert.Equal(product.CategoryId, outPut.Data.CategoryId);

        }


        [Theory(DisplayName = nameof(ListProductWithPaginatedAndCache))]
        [Trait("EndToEnd/API", "Cache-Product - Endpoints")]
        [InlineData(10, 1, 10, 10)]
        [InlineData(17, 2, 10, 7)]
        [InlineData(17, 3, 10, 0)]
        public async Task ListProductWithPaginatedAndCache(int quantityProduct, int page, int perPage, int expectedQuantityItems)
        {
            var products = FakerProducts(quantityProduct, FakerCategory());
            foreach (var item in products)
            {
                await _productRepository.Create(item, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();


            var productCacheNull = await _productCacheRepository.FilterPaginatedQuery( 
                new Mshop.Core.Paginated.PaginatedInPut(
                    page, 
                    perPage, 
                    "", 
                    "", 
                    Mshop.Core.Enum.Paginated.SearchOrder.Desc),Guid.Empty, false, CancellationToken.None);


            var request = new ListProductCacheInPut(page, perPage, "", "", Mshop.Core.Enum.Paginated.SearchOrder.Desc, false);
            var (response, outPut) = await _apiClient.Get<CustomResponse<ListProductCacheOutPut>>($"{Configuration.URL_API_CACHE}list-products-cache/", request);

           Thread.Sleep(2000);

            var productCacheNotNull = await _productCacheRepository.FilterPaginatedQuery(
                new Mshop.Core.Paginated.PaginatedInPut(
                    page,
                    perPage,
                    "",
                    "",
                    Mshop.Core.Enum.Paginated.SearchOrder.Desc), Guid.Empty, false, CancellationToken.None);


            if(expectedQuantityItems == 0)
            {
                Assert.Null(productCacheNotNull);
            }
            else
            {
                Assert.NotNull(productCacheNotNull);
                Assert.True(productCacheNotNull!.Itens.Count() == expectedQuantityItems);
            }

            Assert.Null(productCacheNull);
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.True(outPut.Data.PerPage == perPage);
            Assert.True(outPut.Data.Page == page);
            Assert.True(outPut.Data.Itens.Count() == expectedQuantityItems);

            foreach (var item in outPut.Data.Itens)
            {
                var expectItem = products.FirstOrDefault(x => x.Id == item.Id);

                Assert.NotNull(expectItem);
                Assert.Equal(expectItem.Name, item.Name);
                Assert.Equal(expectItem.Description, item.Description);
                Assert.Equal(expectItem.Price, item.Price);
                //Assert.Equal(expectItem.Thumb.Path, item.Imagem);
            }
        }


        [Theory(DisplayName = nameof(ListProductByCategoryCache))]
        [Trait("EndToEnd/API", "Cache-Product - Endpoints")]
        [InlineData(10, 1, 10, 10)]
        [InlineData(17, 2, 10, 7)]
        [InlineData(17, 3, 10, 0)]
        public async Task ListProductByCategoryCache(int quantityProduct, int page, int perPage, int expectedQuantityItems)
        {
            var category = FakerCategory();
            var products = FakerProducts(quantityProduct, FakerCategory());
            foreach (var item in products)
            {
                await _productRepository.Create(item, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();


            var productCacheNull = await _productCacheRepository.FilterPaginatedQuery(
                new Mshop.Core.Paginated.PaginatedInPut(
                    page,
                    perPage,
                    "",
                    "",
                    Mshop.Core.Enum.Paginated.SearchOrder.Desc), Guid.Empty, false, CancellationToken.None);


            var request = new ListProductByCategoryCacheInPut(page, perPage, "", "", Mshop.Core.Enum.Paginated.SearchOrder.Desc, category.Id);
            var (response, outPut) = await _apiClient.Get<CustomResponse<ListProductByCategoryCacheOutPut>>($"{Configuration.URL_API_CACHE}list-products-by-category-cache/", request);

            Thread.Sleep(2000);

            var productCacheNotNull = await _productCacheRepository.FilterPaginatedQuery(
                new Mshop.Core.Paginated.PaginatedInPut(
                    page,
                    perPage,
                    "",
                    "",
                    Mshop.Core.Enum.Paginated.SearchOrder.Desc), Guid.Empty, false, CancellationToken.None);


            if (expectedQuantityItems == 0)
            {
                Assert.Null(productCacheNotNull);
            }
            else
            {
                Assert.NotNull(productCacheNotNull);
                Assert.True(productCacheNotNull!.Itens.Count() == expectedQuantityItems);
            }

            Assert.Null(productCacheNull);
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.True(outPut.Data.PerPage == perPage);
            Assert.True(outPut.Data.Page == page);
            Assert.True(outPut.Data.Itens.Count() == expectedQuantityItems);

            foreach (var item in outPut.Data.Itens)
            {
                var expectItem = products.FirstOrDefault(x => x.Id == item.Id);

                Assert.NotNull(expectItem);
                Assert.Equal(expectItem.Name, item.Name);
                Assert.Equal(expectItem.Description, item.Description);
                Assert.Equal(expectItem.Price, item.Price);
                //Assert.Equal(expectItem.Thumb.Path, item.Imagem);
            }
        }


        [Theory(DisplayName = nameof(ListProductPromotionCache))]
        [Trait("EndToEnd/API", "Cache-Product - Endpoints")]
        [InlineData(10, 1, 10, 10)]
        [InlineData(17, 2, 10, 7)]
        [InlineData(17, 3, 10, 0)]
        public async Task ListProductPromotionCache(int quantityProduct, int page, int perPage, int expectedQuantityItems)
        {
            var category = FakerCategory();
            var products = FakerProducts(quantityProduct, FakerCategory());
            foreach (var item in products)
            {
                await _productRepository.Create(item, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();


            var productCacheNull = await _productCacheRepository.FilterPaginatedQuery(
                new Mshop.Core.Paginated.PaginatedInPut(
                    page,
                    perPage,
                    "",
                    "",
                    Mshop.Core.Enum.Paginated.SearchOrder.Desc),Guid.Empty, false, CancellationToken.None);


            var request = new ListProductPromotionCacheInPut(page, perPage, "", "", Mshop.Core.Enum.Paginated.SearchOrder.Desc);
            var (response, outPut) = await _apiClient.Get<CustomResponse<ListProductPromotionCacheOutPut>>($"{Configuration.URL_API_CACHE}list-products-onlyPromotion-cache/", request);

            Thread.Sleep(2000);

            var productCacheNotNull = await _productCacheRepository.FilterPaginatedQuery(
                new Mshop.Core.Paginated.PaginatedInPut(
                    page,
                    perPage,
                    "",
                    "",
                    Mshop.Core.Enum.Paginated.SearchOrder.Desc), Guid.Empty, false, CancellationToken.None);


            if (expectedQuantityItems == 0)
            {
                Assert.Null(productCacheNotNull);
            }
            else
            {
                Assert.NotNull(productCacheNotNull);
                Assert.True(productCacheNotNull!.Itens.Count() == expectedQuantityItems);
            }

            Assert.Null(productCacheNull);
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.True(outPut.Data.PerPage == perPage);
            Assert.True(outPut.Data.Page == page);
            Assert.True(outPut.Data.Itens.Count() == expectedQuantityItems);

            foreach (var item in outPut.Data.Itens)
            {
                var expectItem = products.FirstOrDefault(x => x.Id == item.Id);

                Assert.NotNull(expectItem);
                Assert.Equal(expectItem.Name, item.Name);
                Assert.Equal(expectItem.Description, item.Description);
                Assert.Equal(expectItem.Price, item.Price);
                //Assert.Equal(expectItem.Thumb.Path, item.Imagem);
            }
        }


        public void Dispose()
        {

        }
    }
}
