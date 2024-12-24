using Moq;
using Mshop.Core.Test.UseCase;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;
using DomainEntity = Mshop.Domain.Entity;
using ServiceBuilder = Mshop.Application.Service;

namespace Mshop.UnitTests.Application.Service
{
    public class BuildCacheProductTest : UseCaseBaseFixture
    {
        private Mock<IProductCacheRepository> _productCacheRepository;
        private Mock<IProductRepository> _productRepository;

        public BuildCacheProductTest()
        {
            _productCacheRepository = new Mock<IProductCacheRepository>();
            _productRepository = new Mock<IProductRepository>();
        }

        [Fact(DisplayName = nameof(ShouldBuilderCacheProduct))]
        [Trait("Application-Service", "Builder Product Cache")]

        public void ShouldBuilderCacheProduct()
        {
            var category = FakerCategory();
            var listProduct = FakerProducts(50, category);

            _productCacheRepository.Setup(c => c.Create(It.IsAny<DomainEntity.Product>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);
            _productRepository.Setup(c => c.GetProductAll(It.IsAny<bool>())).ReturnsAsync(listProduct);

            var service = new ServiceBuilder.BuildCacheProduct(_productCacheRepository.Object, _productRepository.Object);
            service.Handle();

            _productCacheRepository.Verify(c=>c.Create(It.IsAny<DomainEntity.Product>(), It.IsAny<DateTime>(), CancellationToken.None), Times.AtMost(50));
            _productRepository.Verify(c => c.GetProductAll(It.IsAny<bool>()), Times.Once);
        }



    }
}
