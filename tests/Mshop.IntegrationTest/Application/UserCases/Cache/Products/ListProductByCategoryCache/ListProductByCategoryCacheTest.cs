﻿using Microsoft.Extensions.DependencyInjection;
using ApplicationUseCase = Mshop.Application.UseCases.Cache.Products.ListProductByCategoryCache;
using Mshop.Core.Data;
using Mshop.Core.Enum.Paginated;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;

namespace Mshop.IntegrationTests.Application.UserCases.Cache.Product.ListProductByCategoryCache
{
    [Collection("List Products Collection")]
    [CollectionDefinition("List Products Collection", DisableParallelization = true)]
    public class ListProductByCategoryCacheTest : IntegracaoBaseFixture, IDisposable
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

        public ListProductByCategoryCacheTest() : base()
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

        [Fact(DisplayName = nameof(ListProductByCategoryCache))]
        [Trait("Integration - Application.UseCase.Cache", "Product Use Case")]

        public async Task ListProductByCategoryCache()
        {
            var category = FakerCategory();
            await _categoryRepository.Create(category,CancellationToken.None);

            var productsFake = FakerProducts(10,category);

            var category2 = FakerCategory();
            var productsFake2 = FakerProducts(10, category2);
            productsFake = productsFake.Union(productsFake2).ToList();

            foreach (var product in productsFake)
            {
                await _productRepository.Create(product, CancellationToken.None);
            }


            await _unitOfWork.CommitAsync();

            var useCase = new ApplicationUseCase.ListProductByCategoryCache(
                _productRepository, 
                _productsCacheRepository, 
                _buildCacheProduct,
                _notification);
            var request = new ApplicationUseCase.ListProductByCategoryCacheInPut(
                            page: 1,
                            perPage:5,
                            search: "",
                            sort: "name",
                            dir: Mshop.Core.Enum.Paginated.SearchOrder.Asc,
                            CategoryId: category.Id
                            );

            var outPut = await useCase.Handle(request, CancellationToken.None);
           /* var productsCache = await _productsCacheRepository.FilterPaginated(
                new Core.Paginated.PaginatedInPut(
                    currentPage: 1, 
                    perPage: 20, 
                    search: "", 
                    orderBy: "", 
                    SearchOrder.Desc),
                CancellationToken.None
                );*/

            Thread.Sleep(3000);

            var productsCache = await _productsCacheRepository.FilterPaginatedQuery(
                new Core.Paginated.PaginatedInPut(
                    currentPage: 1, 
                    perPage: 30,
                    search: "", 
                    orderBy: "", 
                    order: SearchOrder.Asc),
                category.Id,false,
                CancellationToken.None
                );

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(productsFake.Count, result.Total);
            Assert.Equal(request.CurrentPage, result.CurrentPage);
            Assert.Equal(request.PerPage, result.PerPage);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Any());
            Assert.NotNull(productsCache);
            Assert.Equal(10, productsCache.Data.Count());
        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
