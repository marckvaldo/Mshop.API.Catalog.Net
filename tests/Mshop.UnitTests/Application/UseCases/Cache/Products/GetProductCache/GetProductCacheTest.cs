using Moq;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;
using System.Linq.Expressions;
using DomainEntity = Mshop.Domain.Entity;
using useCases = Mshop.Application.UseCases.Cache.Products.GetProductCache;

namespace Mshop.UnitTests.Application.UseCases.Cache.Products.GetProductCache
{
    public class GetProductCacheTest : GetProductCacheFixture
    {
        private Mock<IProductCacheRepository> _productCacheRepository;
        private Mock<IProductRepository> _productRepository;
        private Mock<IImageRepository> _imageRepository;
        private Mock<IImagesCacheRepository> _imageCacheRepository;
        private Mock<IBuildCacheProduct> _buildCacheProduct;
        private Mock<IBuildCacheImage> _buildCacheImage;
        private Mock<INotification> _notification;
        public GetProductCacheTest() : base()
        {
            _productRepository = new Mock<IProductRepository>();
            _productCacheRepository = new Mock<IProductCacheRepository>();
            _imageRepository = new Mock<IImageRepository>();
            _imageCacheRepository = new Mock<IImagesCacheRepository>();
            _buildCacheProduct = new Mock<IBuildCacheProduct>();
            _buildCacheImage = new Mock<IBuildCacheImage>();
            _notification = new Mock<INotification>();
        }

        [Fact(DisplayName = nameof(ShouldReturnProductByCache))]
        [Trait("Application-UseCase", "Get Product Cache")]
        public async void ShouldReturnProductByCache()
        {
            
            var category = FakerCategory();
            var dadosResult = FakerProduct(category);
            var imagens = FakerImage(dadosResult.Id);

            _productCacheRepository.Setup(r => r.GetProductById(It.IsAny<Guid>())).ReturnsAsync(dadosResult);
            _imageCacheRepository.Setup(r => r.GetImageByProductId(It.IsAny<Guid>())).ReturnsAsync(imagens);


            var useCase = new useCases.GetProductCache(
                _productRepository.Object, 
                _productCacheRepository.Object, 
                _imageRepository.Object,
                _imageCacheRepository.Object,
                _buildCacheProduct.Object,
                _buildCacheImage.Object,
                _notification.Object);

            var outPut = useCase.Handle(new useCases.GetProductCacheInPut(dadosResult.Id), CancellationToken.None);

            _notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Never);
            _productCacheRepository.Verify(r => r.GetProductById(It.IsAny<Guid>()), Times.Once);
            _productRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);

            _imageCacheRepository.Verify(r=>r.GetImageByProductId(It.IsAny<Guid>()), Times.Once);
            _imageRepository.Verify(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>()), Times.Never);

            _buildCacheProduct.Verify(r => r.Handle(), Times.Never);
            _buildCacheImage.Verify(r => r.Handle(), Times.Never);
            

            Assert.NotNull(outPut.Result);
            Assert.NotNull(outPut.Result.Data);
            Assert.True(outPut.Result.IsSuccess);
            Assert.Equal(outPut.Result.Data.Name, dadosResult.Name);
            Assert.Equal(outPut.Result.Data.Id, dadosResult.Id);
            Assert.Equal(outPut.Result.Data.Category.Name, dadosResult.Category.Name);
            Assert.Equal(outPut.Result.Data.Category.IsActive, dadosResult.Category.IsActive);
            Assert.Equal(outPut.Result.Data.CategoryId, dadosResult.CategoryId);
            Assert.Equal(outPut.Result.Data.Price, dadosResult.Price);
            Assert.Equal(outPut.Result.Data.Description, dadosResult.Description);
            Assert.Equal(outPut.Result.Data.Id, dadosResult.Id);
            Assert.Equal(outPut.Result.Data.Stock, dadosResult.Stock);
            Assert.Equal(outPut.Result.Data.Images.Count(), imagens.Count());
        }


        [Fact(DisplayName = nameof(ShouldReturnProductNotCacheAndBuildChache))]
        [Trait("Application-UseCase", "Get Product Cache")]
        public async void ShouldReturnProductNotCacheAndBuildChache()
        {

            var category = FakerCategory();
            var listProducts = FakerProducts(10, category);
            var dadosResult = listProducts.First();

            var listImages = FakerImage(dadosResult.Id);

            _productCacheRepository.Setup(r => r.GetProductById(It.IsAny<Guid>())).ReturnsAsync((DomainEntity.Product?)null);
            _productRepository.Setup(r => r.GetProductWithCategory(It.IsAny<Guid>())).ReturnsAsync(dadosResult);
            _productRepository.Setup(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Product, bool>>>())).ReturnsAsync(listProducts);
            _productCacheRepository.Setup(r => r.AddProduct(It.IsAny<DomainEntity.Product>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);



            _imageCacheRepository.Setup(r => r.GetImageByProductId(It.IsAny<Guid>())).ReturnsAsync((List<DomainEntity.Image>?)null);
            _imageRepository.Setup(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>())).ReturnsAsync(listImages);
            _imageCacheRepository.Setup(r => r.AddImage(It.IsAny<DomainEntity.Image>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);


            var useCase = new useCases.GetProductCache(
                _productRepository.Object,
                _productCacheRepository.Object,
                _imageRepository.Object,
                _imageCacheRepository.Object,
                _buildCacheProduct.Object,
                _buildCacheImage.Object,
                _notification.Object);

            var outPut = useCase.Handle(new useCases.GetProductCacheInPut(dadosResult.Id), CancellationToken.None);


            _notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Never);
            _productCacheRepository.Verify(r => r.GetProductById(It.IsAny<Guid>()), Times.Once);
            _productRepository.Verify(r => r.GetProductWithCategory(It.IsAny<Guid>()), Times.Once);

            _imageCacheRepository.Verify(r => r.GetImageByProductId(It.IsAny<Guid>()), Times.Once);
            _imageRepository.Verify(r=>r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>()), Times.Once);   
            
            _buildCacheProduct.Verify(r => r.Handle(), Times.Once);
            _buildCacheImage.Verify(r => r.Handle(), Times.Once);
            

            Assert.NotNull(outPut.Result);
            Assert.NotNull(outPut.Result.Data);
            Assert.True(outPut.Result.IsSuccess);
            Assert.Equal(outPut.Result.Data.Name, dadosResult.Name);
            Assert.Equal(outPut.Result.Data.Id, dadosResult.Id);
            Assert.Equal(outPut.Result.Data.Category.Name, dadosResult.Category.Name);
            Assert.Equal(outPut.Result.Data.Category.IsActive, dadosResult.Category.IsActive);
            Assert.Equal(outPut.Result.Data.CategoryId, dadosResult.CategoryId);
            Assert.Equal(outPut.Result.Data.Price, dadosResult.Price);
            Assert.Equal(outPut.Result.Data.Description, dadosResult.Description);
            Assert.Equal(outPut.Result.Data.Id, dadosResult.Id);
            Assert.Equal(outPut.Result.Data.Stock, dadosResult.Stock);
            Assert.Equal(outPut.Result.Data.Images.Count(), listImages.Count());

        }


        [Fact(DisplayName = nameof(ShouldReturnErroCategoryByCache))]
        [Trait("Application-UseCase", "Get Category Cache")]
        public async void ShouldReturnErroCategoryByCache()
        {
           /*var listCategories = FakerCategories(10);
            var dadosResult = listCategories.First();

            _productCacheRepository.Setup(r => r.GetCategoryById(It.IsAny<Guid>())).ReturnsAsync((DomainEntity.Category?)null);
            _productRepository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((DomainEntity.Category?)null);
            _productRepository.Setup(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Category, bool>>>())).ReturnsAsync(listCategories);
            _productCacheRepository.Setup(r => r.AddCategory(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);

            var useCase = new useCase.GetCategoryCache(_notification.Object, _productCacheRepository.Object, _productRepository.Object, _buildCacheCategory.Object);
            var outPut = useCase.Handle(new useCase.GetCategoryCacheInPut(dadosResult.Id), CancellationToken.None);

            _notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Once);
            _productCacheRepository.Verify(r => r.GetCategoryById(It.IsAny<Guid>()), Times.Once);
            _productCacheRepository.Verify(r => r.AddCategory(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None), Times.AtMost(10));
            _buildCacheCategory.Verify(r => r.Handle(), Times.Once);
            _productRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);

            Assert.Null(outPut.Result.Data);
            Assert.False(outPut.Result.IsSuccess);*/
        }

    }
}
