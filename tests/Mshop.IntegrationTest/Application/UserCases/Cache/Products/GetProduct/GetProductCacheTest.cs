using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.Respository;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using System;
using ApplicationUseCase = Mshop.Application.UseCases.Cache.Products.GetProductCache;

namespace Mshop.IntegrationTests.Application.UserCases.Cache.Product.GetProduct
{

    [Collection("Get Products Collection")]
    [CollectionDefinition("Get Products Collection", DisableParallelization = true)]
    public class GetProductCacheTest:IntegracaoBaseFixture
    {
        private IServiceScope _scope;

        private IImageRepository _imageRepository;
        private IImagesCacheRepository _imagesCacheRepository;
        private IProductCacheRepository _productCacheRepository;
        private IProductRepository _productRepository;
        private ICategoryRepository _categoryRepository;
        private INotification _notification;
        private IStorageService _storageService;
        private IUnitOfWork _unitOfWork;
        private RepositoryDbContext _DbContext;
        private StartIndex _startIndex;
        private StackExchange.Redis.IDatabase _database;
        private IBuildCacheImage _buildCacheImage;
        private IBuildCacheProduct _buildCacheProduct;

        public GetProductCacheTest() : base()
        {
            //_scope = _serviceProvider.CreateScope();
            //var scopedProvider = _scope.ServiceProvider;

            var connection = _serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
            _database = connection.GetDatabase();

            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _imageRepository = _serviceProvider.GetRequiredService<IImageRepository>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _productCacheRepository = _serviceProvider.GetRequiredService<IProductCacheRepository>();
            _imagesCacheRepository = _serviceProvider.GetRequiredService<IImagesCacheRepository>();
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

        /*public async Task DisposeAsync()
        {
            if (_scope != null)
                _scope.Dispose();
        }
        public async Task InitializeAsync()
        {
            
        }*/

        [Fact(DisplayName = nameof(GetProductCache))]
        [Trait("Integration - Application.UseCase.Cache", "Product Use Case")]
        public async Task GetProductCache()
        {

            var category = FakerCategory();
            await _categoryRepository.Create(category, CancellationToken.None);

            var productFake = FakerProduct(category);
            await _productRepository.Create(productFake, CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);

            var guid = productFake.Id;
            var useCase = new ApplicationUseCase.GetProductCache(
                _productRepository,
                _productCacheRepository,
                _imageRepository,
                _imagesCacheRepository,
                _buildCacheProduct,
                _buildCacheImage,
                _notification);

            var outPut = await useCase.Handle(new ApplicationUseCase.GetProductCacheInPut(guid), CancellationToken.None);
            var result = outPut.Data;
            var productCache = await _productCacheRepository.GetById(guid);

            Assert.False(_notification.HasErrors());
            Assert.NotNull(result);
            Assert.Equal(result.Name, productFake.Name);
            Assert.Equal(result.Description, productFake.Description);
            Assert.Equal(result.Price, productFake.Price);
            Assert.Equal(result.CategoryId, productFake.CategoryId);
            Assert.Equal(result.Stock, productFake.Stock);
            Assert.Equal(result.IsActive, productFake.IsActive);

            Assert.NotNull(productCache);

        }


        [Fact(DisplayName = nameof(SholdReturnErrorWhenCantGetProductCache))]
        [Trait("Integration - Application.UseCase.Cache", "Product Use Case")]
        public async Task SholdReturnErrorWhenCantGetProductCache()
        {
            var category = FakerCategory();
            await _categoryRepository.Create(category, CancellationToken.None);

            var productFake = FakerProduct(category);
            await _productRepository.Create(productFake, CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var useCase = new ApplicationUseCase.GetProductCache(
                _productRepository,
                _productCacheRepository,
                _imageRepository,
                _imagesCacheRepository,
                _buildCacheProduct,
                _buildCacheImage,
                _notification);

            var outPut = await useCase.Handle(new ApplicationUseCase.GetProductCacheInPut(Guid.NewGuid()), CancellationToken.None);
            var productCache = await _productCacheRepository.GetById(Guid.NewGuid());

            Assert.Null(productCache);
            Assert.True(_notification.HasErrors());
            Assert.False(outPut.IsSuccess);
        }

    }
}
