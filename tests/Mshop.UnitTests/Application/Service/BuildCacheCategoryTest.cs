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
    public class BuildCacheCategoryTest : UseCaseBaseFixture
    {
        private Mock<ICategoryCacheRepository> _categoryCacheRepository;
        private Mock<ICategoryRepository> _categoryRepository;

        private Mock<IServiceProvider> _serviceProvider;
        private Mock<IServiceScope> _mockScope;
        private Mock<IServiceScopeFactory> _scopeFactory;

        public BuildCacheCategoryTest()
        {
            _categoryCacheRepository = new Mock<ICategoryCacheRepository>();
            _categoryRepository = new Mock<ICategoryRepository>();

            _serviceProvider = new Mock<IServiceProvider>();
            _mockScope = new Mock<IServiceScope>();
            _scopeFactory = new Mock<IServiceScopeFactory>();
        }

        [Fact(DisplayName = nameof(ShouldBuilderCacheCategory))]
        [Trait("Application-Service", "Builder Category Cache")]

        public async void ShouldBuilderCacheCategory()
        {
            var listCategory = FakerCategories(50);

            _categoryCacheRepository.Setup(c => c.Create(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);
            _categoryRepository.Setup(c => c.Filter(It.IsAny<Expression<Func<DomainEntity.Category, bool>>>())).ReturnsAsync(listCategory);


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

            var service = new ServiceBuilder.BuildCacheCategory(_serviceProvider.Object);
            await service.Handle();

            _categoryCacheRepository.Verify(c=>c.Create(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None), Times.AtMost(50));
            _categoryRepository.Verify(c => c.Filter(It.IsAny<Expression<Func<DomainEntity.Category, bool>>>()), Times.Once);
        }



    }
}
