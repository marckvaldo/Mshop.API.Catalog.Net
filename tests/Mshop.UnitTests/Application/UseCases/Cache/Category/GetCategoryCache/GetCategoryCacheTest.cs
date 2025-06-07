using Moq;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;
using System.Linq.Expressions;
using DomainEntity = Mshop.Domain.Entity;
using useCase = Mshop.Application.UseCases.Cache.Category.GetCategory;

namespace Mshop.UnitTests.Application.UseCases.Cache.Category.GetCategoryCache
{
    public class GetCategoryCacheTest : GetCategoryCacheFixture
    {
        private Mock<ICategoryCacheRepository> _categoryCacheRepository;
        private Mock<ICategoryRepository> _categoryRepository;
        private Mock<IServiceCacheCategory> _buildCacheCategory;
        private Mock<INotification> _notification;
        public GetCategoryCacheTest() : base()
        {
            _categoryRepository = new Mock<ICategoryRepository>();
            _categoryCacheRepository = new Mock<ICategoryCacheRepository>();
            _buildCacheCategory = new Mock<IServiceCacheCategory>();
            _notification = new Mock<INotification>();
        }

        [Fact(DisplayName = nameof(ShouldReturnCategoryByCache))]
        [Trait("Application-UseCase", "Get Category Cache")]
        public async void ShouldReturnCategoryByCache()
        {
            //_productRepository = new Mock<ICategoryRepository>();
            //var repositoryCache = new Mock<ICategoryCacheRepository>();
            //var buildCache = new Mock<IServiceCacheCategory>();
            //var notification = new Mock<INotification>();

            var dadosResult = FakerCategory();
            _categoryCacheRepository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync(dadosResult);


            var useCase = new useCase.GetCategoryCache(_notification.Object, _categoryCacheRepository.Object, _categoryRepository.Object, _buildCacheCategory.Object);
            var outPut = useCase.Handle(new useCase.GetCategoryCacheInPut(dadosResult.Id), CancellationToken.None);

            _notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Never);
            _categoryCacheRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            _buildCacheCategory.Verify(r => r.BuildCache(), Times.Never);
            _categoryRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);

            Assert.NotNull(outPut.Result);
            Assert.NotNull(outPut.Result.Data);
            Assert.True(outPut.Result.IsSuccess);
            Assert.Equal(outPut.Result.Data.Name, dadosResult.Name);
            Assert.Equal(outPut.Result.Data.Name, dadosResult.Name);
        }


        [Fact(DisplayName = nameof(ShouldReturnCategoryNotCacheAndBuildChache))]
        [Trait("Application-UseCase", "Get Category Cache")]
        public async void ShouldReturnCategoryNotCacheAndBuildChache()
        {
            //_productRepository = new Mock<ICategoryRepository>();
            //var repositoryCache = new Mock<ICategoryCacheRepository>();
            //var buildCache = new Mock<IServiceCacheCategory>();
            //var notification = new Mock<INotification>();

            var listCategories = FakerCategories(10);
            var dadosResult = listCategories.First();

            _categoryCacheRepository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((DomainEntity.Category?)null);
            _categoryRepository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync(dadosResult);
            _categoryRepository.Setup(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Category, bool>>>())).ReturnsAsync(listCategories);
            _categoryCacheRepository.Setup(r => r.Create(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);


            var useCase = new useCase.GetCategoryCache(_notification.Object, _categoryCacheRepository.Object, _categoryRepository.Object, _buildCacheCategory.Object);
            var outPut = useCase.Handle(new useCase.GetCategoryCacheInPut(dadosResult.Id), CancellationToken.None);

            _notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Never);
            _categoryCacheRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            _categoryRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            _categoryCacheRepository.Verify(r => r.Create(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None), Times.AtMost(10));
            _buildCacheCategory.Verify(r => r.BuildCache(), Times.Once);


            Assert.NotNull(outPut.Result);
            Assert.NotNull(outPut.Result.Data);
            Assert.True(outPut.Result.IsSuccess);
            Assert.Equal(outPut.Result.Data.Name, dadosResult.Name);
            Assert.Equal(outPut.Result.Data.Name, dadosResult.Name);
        }


        [Fact(DisplayName = nameof(ShouldReturnErroCategoryByCache))]
        [Trait("Application-UseCase", "Get Category Cache")]
        public async void ShouldReturnErroCategoryByCache()
        {
            //_productRepository = new Mock<ICategoryRepository>();
            //var repositoryCache = new Mock<ICategoryCacheRepository>();
            //var buildCache = new Mock<IServiceCacheCategory>();
            //var notification = new Mock<INotification>();

            var listCategories = FakerCategories(10);
            var dadosResult = listCategories.First();

            _categoryCacheRepository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((DomainEntity.Category?)null);
            _categoryRepository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((DomainEntity.Category?)null);
            _categoryRepository.Setup(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Category, bool>>>())).ReturnsAsync(listCategories);
            _categoryCacheRepository.Setup(r => r.Create(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);

            var useCase = new useCase.GetCategoryCache(_notification.Object, _categoryCacheRepository.Object, _categoryRepository.Object, _buildCacheCategory.Object);
            var outPut = useCase.Handle(new useCase.GetCategoryCacheInPut(dadosResult.Id), CancellationToken.None);

            _notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Once);
            _categoryCacheRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            _categoryCacheRepository.Verify(r => r.Create(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None), Times.AtMost(10));
            _buildCacheCategory.Verify(r => r.BuildCache(), Times.Once);
            _categoryRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);

            Assert.Null(outPut.Result.Data);
            Assert.False(outPut.Result.IsSuccess);
        }

    }
}
