using Moq;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceBuilder = Mshop.Application.Service;
using DomainEntity = Mshop.Domain.Entity;
using System.Linq.Expressions;
using Mshop.UnitTests.Application.UseCases.Common;
using Mshop.Infra.Data.Repository;

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
            var listImage = FakerImage(Guid.NewGuid());

            _imageCacheRepository.Setup(c => c.AddImage(It.IsAny<DomainEntity.Image>(), It.IsAny<DateTime>(), CancellationToken.None)).ReturnsAsync(true);
            _imageRepository.Setup(c => c.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>())).ReturnsAsync(listImage);

            var service = new ServiceBuilder.BuildCacheImage(_imageCacheRepository.Object, _imageRepository.Object);
            service.Handle();

            _imageCacheRepository.Verify(c=>c.AddImage(It.IsAny<DomainEntity.Image>(), It.IsAny<DateTime>(), CancellationToken.None), Times.AtMost(50));
            _imageRepository.Verify(c => c.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>()), Times.Once);
        }



    }
}
