using Moq;
using Mshop.Core.Test.UseCase;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;
using System.Linq.Expressions;
using DomainEntity = Mshop.Domain.Entity;
using ServiceBuilder = Mshop.Application.Service;

namespace Mshop.UnitTests.Application.Service
{
    public class BuildCacheImageTest : UseCaseBaseFixture
    {
        private Mock<IImagesCacheRepository> _imageCacheRepository;
        private Mock<IImageRepository> _imageRepository;

        public BuildCacheImageTest()
        {
            _imageCacheRepository = new Mock<IImagesCacheRepository>();
            _imageRepository = new Mock<IImageRepository>();
        }

        [Fact(DisplayName = nameof(ShouldBuilderCacheImage))]
        [Trait("Application-Service", "Builder Image Cache")]

        public void ShouldBuilderCacheImage()
        {
            var listImage = FakerImages(Guid.NewGuid());

            _imageCacheRepository.Setup(c => c.Create(It.IsAny<DomainEntity.Image>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);
            _imageRepository.Setup(c => c.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>())).ReturnsAsync(listImage);

            var service = new ServiceBuilder.BuildCacheImage(_imageCacheRepository.Object, _imageRepository.Object);
            service.Handle();

            _imageCacheRepository.Verify(c=>c.Create(It.IsAny<DomainEntity.Image>(), It.IsAny<DateTime>(), CancellationToken.None), Times.AtMost(50));
            _imageRepository.Verify(c => c.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>()), Times.Once);
        }



    }
}
