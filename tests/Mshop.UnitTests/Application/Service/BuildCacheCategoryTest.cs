using Moq;
using Mshop.Core.Test.UseCase;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;
using System.Linq.Expressions;
using DomainEntity = Mshop.Domain.Entity;
using ServiceBuilder = Mshop.Application.Service;

namespace Mshop.UnitTests.Application.Service
{
    public class BuildCacheCategoryTest : UseCaseBaseFixture
    {
        private Mock<ICategoryCacheRepository> _categoryCacheRepository;
        private Mock<ICategoryRepository> _categoryRepository;

        public BuildCacheCategoryTest()
        {
            _categoryCacheRepository = new Mock<ICategoryCacheRepository>();
            _categoryRepository = new Mock<ICategoryRepository>();
        }

        [Fact(DisplayName = nameof(ShouldBuilderCacheCategory))]
        [Trait("Application-Service", "Builder Category Cache")]

        public void ShouldBuilderCacheCategory()
        {
            var listCategory = FakerCategories(50);

            _categoryCacheRepository.Setup(c => c.AddCategory(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);
            _categoryRepository.Setup(c => c.Filter(It.IsAny<Expression<Func<DomainEntity.Category, bool>>>())).ReturnsAsync(listCategory);

            var service = new ServiceBuilder.BuildCacheCategory(_categoryCacheRepository.Object, _categoryRepository.Object);
            service.Handle();

            _categoryCacheRepository.Verify(c=>c.AddCategory(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None), Times.AtMost(50));
            _categoryRepository.Verify(c => c.Filter(It.IsAny<Expression<Func<DomainEntity.Category, bool>>>()), Times.Once);
        }



    }
}
