using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.IntegrationTests.Common;
using DomainEntity = Mshop.Domain.Entity;


namespace Mshop.IntegrationTests.Infra.Repository.Cache.ProductRepository
{
    [Collection("Repository Products Collection")]
    [CollectionDefinition("Repository Products Collection", DisableParallelization = true)]
    public class ProductRepositoryCacheTest: IntegracaoBaseFixture
    {
        private readonly IProductCacheRepository _productRepositoryCache;
        private readonly ICategoryCacheRepository _categoryRepositoryCache;
        private readonly StackExchange.Redis.IDatabase _database;
        private readonly DateTime _expirationDate;
        private readonly StartIndex _startIndex;

        public ProductRepositoryCacheTest()
        {
            var connection = _serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
            _database = connection.GetDatabase();
            _productRepositoryCache = _serviceProvider.GetRequiredService<IProductCacheRepository>();
            _categoryRepositoryCache = _serviceProvider.GetRequiredService<ICategoryCacheRepository>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();
            _expirationDate = DateTime.UtcNow.AddHours(1);

            DeleteCache(_database).Wait();
            _startIndex.CreateIndex().Wait();
        }

        [Fact(DisplayName = nameof(CreateProduct))]
        [Trait("Integration - Infra.Cache", "Product Repositorio")]

        public async Task CreateProduct()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            await _productRepositoryCache.Create(product, _expirationDate,CancellationToken.None);
            var newProduct = await _productRepositoryCache.GetById(product.Id);

            Assert.NotNull(newProduct);
            Assert.Equal(product.Id, newProduct.Id);
            Assert.Equal(product.Name, newProduct.Name);
            Assert.Equal(product.Thumb.Path, newProduct.Thumb.Path);
            Assert.Equal(product.Price, newProduct.Price);
            Assert.Equal(product.Stock, newProduct.Stock);
            Assert.Equal(product.CategoryId, newProduct.CategoryId);
        }

        [Fact(DisplayName = nameof(GetByIdProduct))]
        [Trait("Integration - Infra.Cache", "Product Repositorio")]

        public async Task GetByIdProduct()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            var productList = FakerProducts(20,category);
            
            productList.Add(product);
            foreach(var item in productList)
            {
               await _productRepositoryCache.Create(item, _expirationDate,CancellationToken.None);
            }

            var outPut = await _productRepositoryCache.GetById(product.Id);

            Assert.NotNull(outPut);
            Assert.Equal(product.Id, outPut.Id);
            Assert.Equal(product.Name, outPut.Name);
            Assert.Equal(product.Thumb.Path, outPut.Thumb.Path);
            Assert.Equal(product.Price, outPut.Price);
            Assert.Equal(product.Stock, outPut.Stock);
            Assert.Equal(product.CategoryId, outPut.CategoryId);
        }

        [Fact(DisplayName = nameof(UpdateProduct))]
        [Trait("Integration - Infra.Cache", "Product Repositorio")]

        public async Task UpdateProduct()
        {
            var category = FakerCategory();
            var productList = FakerProducts(20, category);
            Guid id = productList.First().Id;
            var imagem = FakerImage(id);
            var request = FakerProduct(category);

            foreach (var item in productList)
            {
                await _productRepositoryCache.Create(item, _expirationDate, CancellationToken.None);
            }
            var product = await _productRepositoryCache.GetById(id);

            Assert.NotNull(product);

            product.Update(request.Description, request.Name, request.Price, request.CategoryId);
            product.UpdateThumb(request.Thumb.Path);
            product.UpdateQuantityStock(request.Stock);

            await _productRepositoryCache.Update(product, _expirationDate, CancellationToken.None);

            var productUpdate = await _productRepositoryCache.GetById(id);

            Assert.NotNull(productUpdate);
            Assert.Equal(id, productUpdate.Id);
            Assert.Equal(request.Name, productUpdate.Name);
            Assert.Equal(request.Thumb.Path, productUpdate.Thumb.Path);
            Assert.Equal(request.Price, productUpdate.Price);
            Assert.Equal(request.Stock, productUpdate.Stock);
            Assert.Equal(request.CategoryId, productUpdate.CategoryId);
        }


        [Fact(DisplayName = nameof(DeleteProduct))]
        [Trait("Integration - Infra.Cache", "Product Repositorio")]

        public async Task DeleteProduct()
        {
            var category = FakerCategory();
            var productList = FakerProducts(20,category);

            foreach (var item in productList)
            {
                await _productRepositoryCache.Create(item, _expirationDate, CancellationToken.None);
            }

            var request = productList.First();
            await _productRepositoryCache.DeleteById(request, CancellationToken.None);
            var productUpdate = await _productRepositoryCache.GetById(request.Id);

            Assert.Null(productUpdate);
        }


        [Fact(DisplayName = nameof(FilterPaginated))]
        [Trait("Integration - Infra.Cache", "Product Repositorio")]
        public async Task FilterPaginated()
        {
            var category = FakerCategory();
            await _categoryRepositoryCache.Create(category, _expirationDate, CancellationToken.None);

            var productList = FakerProducts(30,category);
            foreach (var item in productList)
            {
                await _productRepositoryCache.Create(item, _expirationDate, CancellationToken.None);
            }

            var perPage = 10;
            var input = new Core.Paginated.PaginatedInPut(1, perPage, "","",Core.Enum.Paginated.SearchOrder.Asc);
            var outPut = await _productRepositoryCache.FilterPaginatedQuery(input, It.IsAny<Guid>(), It.IsAny<bool>(), CancellationToken.None);

            Assert.NotNull(outPut);
            Assert.NotNull(outPut.Data);
            Assert.True(outPut.Data.Count == perPage);
            Assert.Equal(input.PerPage, outPut.PerPage);

            foreach(DomainEntity.Product item in outPut.Data)
            {
                var product = productList.Find(x => x.Id == item.Id);
                Assert.NotNull(product);
                Assert.Equal(product.Name, item.Name);
                Assert.Equal(product.Description, item.Description);
                Assert.Equal(product.Price, item.Price);
                Assert.Equal(product.CategoryId, item.CategoryId);
            }

        }


        [Fact(DisplayName = nameof(SholdResultListEmptyFilterPaginated))]
        [Trait("Integration - Infra.Cache", "Product Repositorio")]

        public async Task SholdResultListEmptyFilterPaginated()
        {
            var perPage = 20;
            var input = new Core.Paginated.PaginatedInPut(1, perPage, "", "", Core.Enum.Paginated.SearchOrder.Asc);
            var outPut = await _productRepositoryCache.FilterPaginatedQuery(input, It.IsAny<Guid>(), It.IsAny<bool>(), CancellationToken.None);

            Assert.Null(outPut);
           
        }


        [Theory(DisplayName = nameof(SerachPaginated))]
        [Trait("Integration - Infra.Cache", "Product Repositorio")]
        [InlineData(10, 1, 10, 10)]
        [InlineData(17, 2, 10, 7)]
        [InlineData(21, 3, 10, 1)]

        public async Task SerachPaginated(int quantityProduct, int page, int perPage, int expectedQuantityItems)
        {

            var category = FakerCategory();
            await _categoryRepositoryCache.Create(category, _expirationDate, CancellationToken.None);

            var productList = FakerProducts(quantityProduct, category);
            foreach (var item in productList)
            {
                await _productRepositoryCache.Create(item, _expirationDate, CancellationToken.None);
            }

            var input = new Core.Paginated.PaginatedInPut(page, perPage, "", "", Core.Enum.Paginated.SearchOrder.Asc);
            var outPut = await _productRepositoryCache.FilterPaginatedQuery(input, Guid.Empty, false, CancellationToken.None);

            Assert.NotNull(outPut);
            Assert.NotNull(outPut.Data);
            Assert.True(outPut.Data.Count == expectedQuantityItems);
            Assert.Equal(outPut.PerPage, perPage);   
            Assert.True(outPut.Total == quantityProduct);
            Assert.Equal(input.PerPage, outPut.PerPage);

            foreach (DomainEntity.Product item in outPut.Data)
            {
                var product = productList.Find(x => x.Id == item.Id);
                Assert.NotNull(product);
                Assert.Equal(product.Name, item.Name);
                Assert.Equal(product.Description, item.Description);
                Assert.Equal(product.Price, item.Price);
                Assert.Equal(product.CategoryId, item.CategoryId);
            }

        }

    }
}
