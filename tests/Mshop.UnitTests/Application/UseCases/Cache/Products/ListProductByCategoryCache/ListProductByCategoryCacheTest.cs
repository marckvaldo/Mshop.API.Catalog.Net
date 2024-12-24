
using Moq;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;
using useCases = Mshop.Application.UseCases.Cache.Products.ListProductByCategoryCache;
using DomainEntity = Mshop.Domain.Entity;
using Mshop.Core.Paginated;
using System.Linq.Expressions;
using Mshop.UnitTests.Application.UseCases.Cache.Category.GetCategoryCache;

namespace Mshop.UnitTests.Application.UseCases.Cache.Products.ListProductByCategoryCache
{
    public class ListProductByCategoryCacheTest : GetCategoryCacheFixture
    {
        private Mock<IProductCacheRepository> _productCacheRepository;
        private Mock<IProductRepository> _productRepository;
        private Mock<IImageRepository> _imageRepository;
        private Mock<IImagesCacheRepository> _imageCacheRepository;
        private Mock<IBuildCacheProduct> _buildCacheProduct;
        private Mock<IBuildCacheImage> _buildCacheImage;
        private Mock<INotification> _notification;
        public ListProductByCategoryCacheTest() : base()
        {
            _productRepository = new Mock<IProductRepository>();
            _productCacheRepository = new Mock<IProductCacheRepository>();
            _imageRepository = new Mock<IImageRepository>();
            _imageCacheRepository = new Mock<IImagesCacheRepository>();
            _buildCacheProduct = new Mock<IBuildCacheProduct>();
            _buildCacheImage = new Mock<IBuildCacheImage>();
            _notification = new Mock<INotification>();
        }

        [Fact(DisplayName = nameof(ShouldReturListProductByCategoryCache))]
        [Trait("Application-UseCase", "List Product by Category Cache")]

        public void ShouldReturListProductByCategoryCache()
        {
            var category = FakerCategory();
            var listProducts = FakerProducts(50,category);

            string searchName = listProducts.First().Name[0..3];

            var listOrdenada = listProducts.Where(x=>x.Name == searchName).Where(c=>c.CategoryId == category.Id).OrderBy(x=>x.Name.OrderDescending()).ToList();
            var paginateOutPut = new PaginatedOutPut<DomainEntity.Product>(1, 10, 50, listOrdenada);

            _productCacheRepository.Setup(r => r.FilterPaginatedByCategory(It.IsAny<PaginatedInPut>(),It.IsAny<Guid>(),CancellationToken.None)).ReturnsAsync(paginateOutPut);

            var useCase = new useCases.ListProductByCategoryCache(_productRepository.Object, _productCacheRepository.Object, _buildCacheProduct.Object, _notification.Object);
            var outPut = useCase.Handle(
                    new useCases.ListProductByCategoryCacheInPut(
                            page:1,
                            perPage:10,
                            search: searchName,
                            sort:"Name", 
                            dir: Core.Enum.Paginated.SearchOrder.Desc,
                            category.Id
                            ), CancellationToken.None);

            _productCacheRepository.Verify(r=>r.FilterPaginatedByCategory(It.IsAny<PaginatedInPut>(), It.IsAny<Guid>(), CancellationToken.None), Times.Once);
            _buildCacheProduct.Verify(r => r.Handle(), Times.Never);
            _productRepository.Verify(r=>r.GetById(It.IsAny<Guid>()), Times.Never);
            _notification.Verify(r => r.AddNotifications(It.IsAny<string>()),Times.Never);

            Assert.NotNull(outPut.Result);
            Assert.NotNull(outPut.Result.Data);

            var listOutPut = outPut.Result.Data;

            for (int i = 0;i<listOutPut.Itens.Count;i++)
            {
                if(listOrdenada[i].Equals(listOutPut.Itens[i]))
                    Assert.False(true);
            }
        }


        [Fact(DisplayName = nameof(ShouldReturListProductByCategoryNotCacheAndBuildChace))]
        [Trait("Application-UseCase", "List Product by Category Cache")]

        public void ShouldReturListProductByCategoryNotCacheAndBuildChace()
        {
            var category = FakerCategory();
            var listProducts = FakerProducts(50, category);

            string searchName = listProducts.First().Name[0..3];

            var listOrdenada = listProducts.Where(x => x.Name == searchName).OrderBy(x => x.Name.OrderDescending()).ToList();
            var paginateOutPut = new PaginatedOutPut<DomainEntity.Product>(1, 10, 50, listOrdenada);

            _productCacheRepository.Setup(r => r.FilterPaginatedByCategory(It.IsAny<PaginatedInPut>(),It.IsAny<Guid>() ,CancellationToken.None)).ReturnsAsync((PaginatedOutPut<DomainEntity.Product>?)null);
            _productRepository.Setup(r => r.FilterPaginatedByCategoryId(It.IsAny<PaginatedInPut>(), It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(paginateOutPut);
            _productRepository.Setup(r => r.GetProductAll(It.IsAny<bool>())).ReturnsAsync(listOrdenada);
            _productCacheRepository.Setup(r => r.Create(It.IsAny<DomainEntity.Product>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);



            var useCase = new useCases.ListProductByCategoryCache(_productRepository.Object, _productCacheRepository.Object, _buildCacheProduct.Object, _notification.Object);
            var outPut = useCase.Handle(
                    new useCases.ListProductByCategoryCacheInPut(
                            page: 1,
                            perPage: 10,
                            search: searchName,
                            sort: "Name",
                            dir: Core.Enum.Paginated.SearchOrder.Desc,
                            category.Id
                            ), CancellationToken.None);

            _productCacheRepository.Verify(r => r.FilterPaginatedByCategory(It.IsAny<PaginatedInPut>(), It.IsAny<Guid>(), CancellationToken.None), Times.Once);
            _buildCacheProduct.Verify(r => r.Handle(), Times.Once);
            _productRepository.Verify(r => r.FilterPaginatedByCategoryId(It.IsAny<PaginatedInPut>(), It.IsAny<Guid>(), CancellationToken.None), Times.Once);
            _notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Never);

            Assert.NotNull(outPut.Result);
            Assert.NotNull(outPut.Result.Data);

            var listOutPut = outPut.Result.Data;

            for (int i = 0; i < listOutPut.Itens.Count; i++)
            {
                if (listOrdenada[i].Equals(listOutPut.Itens[i]))
                    Assert.False(true);
            }
        }


        [Fact(DisplayName = nameof(ShouldReturnNullProductByCategory))]
        [Trait("Application-UseCase", "List Product by Category Cache")]

        public void ShouldReturnNullProductByCategory()
        {

            var category = FakerCategory();
            var listProducts = FakerProducts(50, category);

            string searchName = listProducts.First().Name[0..3];

            var listOrdenada = listProducts.Where(x => x.Name == searchName).OrderBy(x => x.Name.OrderDescending()).ToList();
            var paginateOutPut = new PaginatedOutPut<DomainEntity.Product>(1, 10, 50, listOrdenada);

            _productCacheRepository.Setup(r => r.FilterPaginatedByCategory(It.IsAny<PaginatedInPut>(), It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync((PaginatedOutPut<DomainEntity.Product>?)null);
            _productRepository.Setup(r => r.FilterPaginatedByCategoryId(It.IsAny<PaginatedInPut>(), It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync((PaginatedOutPut<DomainEntity.Product>?)null);
            _productRepository.Setup(r => r.GetProductAll(It.IsAny<bool>())).ReturnsAsync((List<DomainEntity.Product>?)null);
            _productCacheRepository.Setup(r => r.Create(It.IsAny<DomainEntity.Product>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);

            var useCase = new useCases.ListProductByCategoryCache(_productRepository.Object, _productCacheRepository.Object, _buildCacheProduct.Object, _notification.Object);
            var outPut = useCase.Handle(
                    new useCases.ListProductByCategoryCacheInPut(
                            page: 1,
                            perPage: 10,
                            search: searchName,
                            sort: "Name",
                            dir: Core.Enum.Paginated.SearchOrder.Desc,
                            category.Id
                            ), CancellationToken.None);

            _productCacheRepository.Verify(r => r.FilterPaginatedByCategory(It.IsAny<PaginatedInPut>(), It.IsAny<Guid>(), CancellationToken.None), Times.Once);
            _buildCacheProduct.Verify(r => r.Handle(), Times.Once);
            _productRepository.Verify(r => r.GetProductAll(It.IsAny<bool>()), Times.Never);
            _notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Once);

            Assert.Null(outPut.Result.Data);
            Assert.False(outPut.Result.IsSuccess);

           

        }
    }
}

