using Microsoft.Extensions.DependencyInjection;
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
        private Mock<IServiceProvider> _serviceProvider;
        private Mock<IServiceScope> _mockScope;
        private Mock<IServiceScopeFactory> _scopeFactory;

        public BuildCacheProductTest()
        {
            _productCacheRepository = new Mock<IProductCacheRepository>();
            _productRepository = new Mock<IProductRepository>();

            _serviceProvider = new Mock<IServiceProvider>();
            _mockScope = new Mock<IServiceScope>();
            _scopeFactory = new Mock<IServiceScopeFactory>();
        }

        [Fact(DisplayName = nameof(ShouldBuilderCacheProduct))]
        [Trait("Application-Service", "Builder Product Cache")]

        public async void ShouldBuilderCacheProduct()
        {
            var category = FakerCategory();
            var listProduct = FakerProducts(50, category);

       
            var _productRepository = new Mock<IProductRepository>();
            var _productCacheRepository = new Mock<IProductCacheRepository>();

            _productCacheRepository.Setup(c => c.Create(It.IsAny<DomainEntity.Product>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);
            _productRepository.Setup(c => c.GetProductAll(It.IsAny<bool>())).ReturnsAsync(listProduct);


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
                .Setup(sp => sp.GetService(typeof(IProductRepository)))
                .Returns(_productRepository.Object);

            _serviceProvider
                .Setup(sp => sp.GetService(typeof(IProductCacheRepository)))
                .Returns(_productCacheRepository.Object);


            var service = new ServiceBuilder.BuildCacheProduct(_serviceProvider.Object);
            await service.Handle();

            _productCacheRepository.Verify(c=>c.Create(It.IsAny<DomainEntity.Product>(), It.IsAny<DateTime>(), CancellationToken.None), Times.AtMost(50));
            _productRepository.Verify(c => c.GetProductAll(It.IsAny<bool>()), Times.Once);
        }



    }
}
