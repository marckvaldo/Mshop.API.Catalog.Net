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
    public class BuildCacheImageTest : UseCaseBaseFixture
    {
        private Mock<IImagesCacheRepository> _imageCacheRepository;
        private Mock<IImageRepository> _imageRepository;

        private Mock<IServiceProvider> _serviceProvider;
        private Mock<IServiceScope> _mockScope;
        private Mock<IServiceScopeFactory> _scopeFactory;


        public BuildCacheImageTest()
        {
            _imageCacheRepository = new Mock<IImagesCacheRepository>();
            _imageRepository = new Mock<IImageRepository>();

            _serviceProvider = new Mock<IServiceProvider>();
            _mockScope = new Mock<IServiceScope>();
            _scopeFactory = new Mock<IServiceScopeFactory>();
        }

        [Fact(DisplayName = nameof(ShouldBuilderCacheImage))]
        [Trait("Application-Service", "Builder Image Cache")]

        public async void ShouldBuilderCacheImage()
        {
            var listImage = FakerImages(Guid.NewGuid());

            
            //var _mockScope = new Mock<IServiceScope>();
            //var _scopeFactory = new Mock<IServiceScopeFactory>();

            var _imageRepository = new Mock<IImageRepository>();
            var _imageCacheRepository = new Mock<IImagesCacheRepository>();

            _imageCacheRepository.Setup(c => c.Create(It.IsAny<DomainEntity.Image>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);
            _imageRepository.Setup(c => c.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>())).ReturnsAsync(listImage);


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
                .Setup(sp => sp.GetService(typeof(IImageRepository)))
                .Returns(_imageRepository.Object);

            _serviceProvider
                .Setup(sp => sp.GetService(typeof(IImagesCacheRepository)))
                .Returns(_imageCacheRepository.Object);


            var service = new ServiceBuilder.BuildCacheImage(_serviceProvider.Object);
            await service.Handle();

            _imageCacheRepository.Verify(c=>c.Create(It.IsAny<DomainEntity.Image>(), It.IsAny<DateTime>(), CancellationToken.None), Times.AtMost(50));
            _imageRepository.Verify(c => c.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>()), Times.Once);
        }



    }
}
