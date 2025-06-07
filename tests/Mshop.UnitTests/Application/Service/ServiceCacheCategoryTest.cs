using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mshop.Core.Test.UseCase;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.Respository;
using Mshop.Infra.Data.Interface;
using Mshop.Infra.Data.Repository;
using System.Linq.Expressions;
using DomainEntity = Mshop.Domain.Entity;
using ServiceBuilder = Mshop.Application.Service;

namespace Mshop.UnitTests.Application.Service
{
    [Collection("Service Category Cache")]
    public class ServiceCacheCategoryTest : UseCaseBaseFixture
    {
        private Mock<ICategoryCacheRepository> _categoryCacheRepository;
        private Mock<ICategoryRepository> _categoryRepository;
        private Mock<IConfigCacheRespository> _configCacheRespository;

        private Mock<IServiceProvider> _serviceProvider;
        private Mock<IServiceScope> _mockScope;
        private Mock<IServiceScopeFactory> _scopeFactory;

        public ServiceCacheCategoryTest()
        {
            _categoryCacheRepository = new Mock<ICategoryCacheRepository>();
            _categoryRepository = new Mock<ICategoryRepository>();
            _configCacheRespository = new Mock<IConfigCacheRespository>();

            _serviceProvider = new Mock<IServiceProvider>();
            _mockScope = new Mock<IServiceScope>();
            _scopeFactory = new Mock<IServiceScopeFactory>();
        }

        [Fact(DisplayName = nameof(ShouldBuilderCacheCategory))]
        [Trait("Application-Service-Category", "Builder Category Cache")]

        public async void ShouldBuilderCacheCategory()
        {
            var listCategory = FakerCategories(50);

            _categoryCacheRepository.Setup(c => c.Create(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);
            _categoryRepository.Setup(c => c.Filter(It.IsAny<Expression<Func<DomainEntity.Category, bool>>>())).ReturnsAsync(listCategory);
            _configCacheRespository.Setup(c => c.GetExpirationDate()).ReturnsAsync(DateTime.UtcNow.AddMinutes(1));

            _scopeFactory
               .Setup(sf => sf.CreateScope())
               .Returns(_mockScope.Object);

            _mockScope
                .Setup(s => s.ServiceProvider)
                .Returns(_serviceProvider.Object);

            _serviceProvider
                .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
                .Returns(_scopeFactory.Object);

            _serviceProvider
                .Setup(sp => sp.GetService(typeof(ICategoryRepository)))
                .Returns(_categoryRepository.Object);

            _serviceProvider
                .Setup(sp => sp.GetService(typeof(ICategoryCacheRepository)))
                .Returns(_categoryCacheRepository.Object);

            _serviceProvider
                .Setup(sp => sp.GetService(typeof(IConfigCacheRespository)))
                .Returns(_configCacheRespository.Object);

            var service = new ServiceBuilder.ServiceCacheCategory(_serviceProvider.Object);
            await service.BuildCache();

            _categoryCacheRepository.Verify(c=>c.Create(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None), Times.AtMost(50));
            _categoryRepository.Verify(c => c.Filter(It.IsAny<Expression<Func<DomainEntity.Category, bool>>>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ShouldAddCategoryToCache))]
        [Trait("Application-Service-Category", "Add Category Cache")]
        public async void ShouldAddCategoryToCache()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = FakerCategory();

            var expirationDate = DateTime.UtcNow.AddMinutes(1);

            _categoryCacheRepository
                .Setup(c => c.GetById(It.Is<Guid>(id => id == categoryId)))
                .ReturnsAsync((DomainEntity.Category)null);

            _categoryRepository
                .Setup(c => c.GetById(It.Is<Guid>(id => id == categoryId)))
                .ReturnsAsync(category);

            _categoryCacheRepository
                .Setup(c => c.Create(category, expirationDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);


            SetupScopeFactory();

            var service = new ServiceBuilder.ServiceCacheCategory(_serviceProvider.Object);

            // Act
            await service.AddCategory(categoryId, CancellationToken.None);

            // Assert
            _categoryCacheRepository.Verify(c => c.GetById(It.Is<Guid>(id => id == categoryId)), Times.Once);
            _categoryCacheRepository.Verify(c => c.Create(category, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
            _categoryRepository.Verify(c => c.GetById(It.Is<Guid>(id => id == categoryId)), Times.Once);
        }


        [Fact(DisplayName = nameof(ShouldRemoveCategoryFromCache))]
        [Trait("Application-Service-Category", "Remove Category Cache")]
        public async void ShouldRemoveCategoryFromCache()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = FakerCategory();

            _categoryCacheRepository
                .Setup(c => c.GetById(It.Is<Guid>(id => id == categoryId)))
                .ReturnsAsync(category);

            _categoryCacheRepository
                .Setup(c => c.DeleteById(category, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            SetupScopeFactory();

            var service = new ServiceBuilder.ServiceCacheCategory(_serviceProvider.Object);

            // Act
            await service.RemoveCategory(categoryId, CancellationToken.None);

            // Assert
            _categoryCacheRepository.Verify(c => c.GetById(It.Is<Guid>(id => id == categoryId)), Times.Once);
            _categoryCacheRepository.Verify(c => c.DeleteById(category, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact(DisplayName = nameof(ShouldUpdateCategoryInCache))]
        [Trait("Application-Service-Category", "Update Category Cache")]
        public async void ShouldUpdateCategoryInCache()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = FakerCategory();
            var expirationDate = DateTime.UtcNow.AddMinutes(1);

            _categoryCacheRepository
                .Setup(c => c.GetById(It.Is<Guid>(id => id == categoryId)))
                .ReturnsAsync(category);

            _categoryCacheRepository
                .Setup(c => c.DeleteById(category, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _categoryRepository
                .Setup(c => c.GetById(It.Is<Guid>(id => id == categoryId)))
                .ReturnsAsync(category);

            _categoryCacheRepository
                .Setup(c => c.Create(category, expirationDate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            SetupScopeFactory();

            var service = new ServiceBuilder.ServiceCacheCategory(_serviceProvider.Object);

            // Act
            await service.UpdateCategory(categoryId, CancellationToken.None);

            // Assert
            _categoryCacheRepository.Verify(c => c.GetById(It.Is<Guid>(id => id == categoryId)), Times.Once);
            _categoryCacheRepository.Verify(c => c.DeleteById(category, It.IsAny<CancellationToken>()), Times.Once);
            _categoryRepository.Verify(c => c.GetById(It.Is<Guid>(id => id == categoryId)), Times.Once);
            _categoryCacheRepository.Verify(c => c.Create(category, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
        }



        private void SetupScopeFactory()
        {
            _scopeFactory
                .Setup(sf => sf.CreateScope())
                .Returns(_mockScope.Object);

            _mockScope
                .Setup(s => s.ServiceProvider)
                .Returns(_serviceProvider.Object);

            _serviceProvider
                .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
                .Returns(_scopeFactory.Object);

            _serviceProvider
                .Setup(sp => sp.GetService(typeof(ICategoryRepository)))
                .Returns(_categoryRepository.Object);

            _serviceProvider
                .Setup(sp => sp.GetService(typeof(ICategoryCacheRepository)))
                .Returns(_categoryCacheRepository.Object);

            _serviceProvider
                .Setup(sp => sp.GetService(typeof(IConfigCacheRespository)))
                .Returns(_configCacheRespository.Object);
        }

    }
}
