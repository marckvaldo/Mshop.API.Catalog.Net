
using Moq;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;
using useCases = Mshop.Application.UseCases.Cache.Category.ListCategoriesCache;
using DomainEntity = Mshop.Domain.Entity;
using Mshop.Core.Paginated;
using System.Linq.Expressions;
using Mshop.UnitTests.Application.UseCases.Cache.Category.GetCategoryCache;

namespace Mshop.UnitTests.Application.UseCases.Cache.Category.ListCategoriesCache
{
    public class ListCategoriesTest : GetCategoryCacheFixture
    {
        private Mock<ICategoryCacheRepository> _categoryCacheRepository;
        private Mock<ICategoryRepository> _categoryRepository;
        private Mock<IBuildCacheCategory> _buildCacheCategory;
        private Mock<INotification> _notification;
        public ListCategoriesTest() : base()
        {
            _categoryRepository = new Mock<ICategoryRepository>();
            _categoryCacheRepository = new Mock<ICategoryCacheRepository>();
            _buildCacheCategory = new Mock<IBuildCacheCategory>();
            _notification = new Mock<INotification>();
        }

        [Fact(DisplayName = nameof(ShouldReturListCategoryByCache))]
        [Trait("Application-UseCase", "List Category Cache")]

        public void ShouldReturListCategoryByCache()
        {
            var listCategory = FakerCategories(50);
            string searchName = listCategory.First().Name[0..3];

            var listOrdenada = listCategory.Where(x=>x.Name == searchName).OrderBy(x=>x.Name.OrderDescending()).ToList();
            var paginateOutPut = new PaginatedOutPut<DomainEntity.Category>(1, 10, 50, listOrdenada);

            _categoryCacheRepository.Setup(r => r.FilterPaginated(It.IsAny<PaginatedInPut>(), CancellationToken.None)).ReturnsAsync(paginateOutPut);

            var useCase = new useCases.ListCategoriesCache(_categoryCacheRepository.Object,_categoryRepository.Object,_buildCacheCategory.Object,_notification.Object);
            var outPut = useCase.Handle(
                    new useCases.ListCategoriesCacheInPut(
                            page:1,
                            perPage:10,
                            search: searchName,
                            sort:"Name", 
                            dir: Core.Enum.Paginated.SearchOrder.Desc
                            ), CancellationToken.None);

            _categoryCacheRepository.Verify(r=>r.FilterPaginated(It.IsAny<PaginatedInPut>(), CancellationToken.None), Times.Once);
            _buildCacheCategory.Verify(r => r.Handle(), Times.Never);
            _categoryRepository.Verify(r=>r.GetById(It.IsAny<Guid>()), Times.Never);
            _notification.Verify(r => r.AddNotifications(It.IsAny<string>()),Times.Never);

            Assert.NotNull(outPut.Result);
            Assert.NotNull(outPut.Result.Data);

            var listOutPut = outPut.Result.Data;

            for (int i = 0;i<listOutPut.Data.Count;i++)
            {
                if(listCategory[i].Equals(listOutPut.Data[i]))
                    Assert.False(true);
            }
        }


        [Fact(DisplayName = nameof(ShouldReturListCategoryNotCacheAndBuildChace))]
        [Trait("Application-UseCase", "List Category Cache")]

        public void ShouldReturListCategoryNotCacheAndBuildChace()
        {
            var listCategory = FakerCategories(50);
            string searchName = listCategory.First().Name[0..3];

            var listOrdenada = listCategory.Where(x => x.Name == searchName).OrderBy(x => x.Name.OrderDescending()).ToList();
            var paginateOutPut = new PaginatedOutPut<DomainEntity.Category>(1, 10, 50, listOrdenada);

            _categoryCacheRepository.Setup(r => r.FilterPaginated(It.IsAny<PaginatedInPut>(), CancellationToken.None)).ReturnsAsync((PaginatedOutPut<DomainEntity.Category>?)null);
            _categoryRepository.Setup(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Category, bool>>>())).ReturnsAsync(listOrdenada);
            _categoryRepository.Setup(r => r.FilterPaginated(It.IsAny<PaginatedInPut>(), CancellationToken.None)).ReturnsAsync(paginateOutPut);
            _categoryCacheRepository.Setup(r => r.Create(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);
            

            var useCase = new useCases.ListCategoriesCache(_categoryCacheRepository.Object, _categoryRepository.Object, _buildCacheCategory.Object, _notification.Object);
            var outPut = useCase.Handle(
                    new useCases.ListCategoriesCacheInPut(
                            page: 1,
                            perPage: 10,
                            search: searchName,
                            sort: "Name",
                            dir: Core.Enum.Paginated.SearchOrder.Desc
                            ), CancellationToken.None);

            _categoryCacheRepository.Verify(r => r.FilterPaginated(It.IsAny<PaginatedInPut>(), CancellationToken.None), Times.Once);
            _buildCacheCategory.Verify(r => r.Handle(), Times.Once);
            _categoryRepository.Verify(r => r.FilterPaginated(It.IsAny<PaginatedInPut>(), CancellationToken.None), Times.Once);
            //_productRepository.Verify(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Category, bool>>>()), Times.Once);
            _categoryCacheRepository.Verify(r => r.Create(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None),Times.AtMost(50));
            _notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Never);

            Assert.NotNull(outPut.Result);
            Assert.NotNull(outPut.Result.Data);

            var listOutPut = outPut.Result.Data;

            for (int i = 0; i < listOutPut.Data.Count; i++)
            {
                if (listCategory[i].Equals(listOutPut.Data[i]))
                    Assert.False(true);
            }
        }


        [Fact(DisplayName = nameof(ShouldReturnNullCategory))]
        [Trait("Application-UseCase", "List Category Cache")]

        public void ShouldReturnNullCategory()
        {
            var listCategory = FakerCategories(50);
            Guid searchName = listCategory.First().Id;

            var listOrdenada = listCategory.Where(x => x.Name == searchName.ToString()).OrderBy(x => x.Name.OrderDescending()).ToList();
            var paginateOutPut = new PaginatedOutPut<DomainEntity.Category>(1, 10, 50, listOrdenada);

            _categoryCacheRepository.Setup(r => r.FilterPaginated(It.IsAny<PaginatedInPut>(), CancellationToken.None)).ReturnsAsync((PaginatedOutPut<DomainEntity.Category>?)null);
            _categoryRepository.Setup(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Category, bool>>>())).ReturnsAsync(listOrdenada);
            _categoryRepository.Setup(r => r.FilterPaginated(It.IsAny<PaginatedInPut>(), CancellationToken.None)).ReturnsAsync((PaginatedOutPut<DomainEntity.Category>?)null);
            _categoryCacheRepository.Setup(r => r.Create(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);


            var useCase = new useCases.ListCategoriesCache(_categoryCacheRepository.Object, _categoryRepository.Object, _buildCacheCategory.Object, _notification.Object);
            var outPut = useCase.Handle(
                    new useCases.ListCategoriesCacheInPut(
                            page: 1,
                            perPage: 10,
                            search: searchName.ToString(),
                            sort: "Name",
                            dir: Core.Enum.Paginated.SearchOrder.Desc
                            ), CancellationToken.None);

            _categoryCacheRepository.Verify(r => r.FilterPaginated(It.IsAny<PaginatedInPut>(), CancellationToken.None), Times.Once);
            _buildCacheCategory.Verify(r => r.Handle(), Times.Once);
            _categoryRepository.Verify(r => r.FilterPaginated(It.IsAny<PaginatedInPut>(), CancellationToken.None), Times.Once);
            _categoryCacheRepository.Verify(r => r.Create(It.IsAny<DomainEntity.Category>(), It.IsAny<DateTime>(), CancellationToken.None), Times.AtMost(50));
            _notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Once);

            
            Assert.Null(outPut.Result.Data);
            Assert.False(outPut.Result.IsSuccess);

         }
    }
}

