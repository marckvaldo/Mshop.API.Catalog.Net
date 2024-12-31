using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ServiceBuilder = Mshop.Application.Service;

namespace Mshop.IntegrationTests.Application.Service
{
    public class BuildCacheImageTest : IntegracaoBaseFixture
    {
        private IImagesCacheRepository _imagesCacheRepository;
        private IImageRepository _imagesRepository;
        private IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;
        private readonly StartIndex _startIndex;
        private readonly StackExchange.Redis.IDatabase _database;

        public BuildCacheImageTest() : base()
        {
            var connection = _serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
            _database = connection.GetDatabase();

            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _imagesCacheRepository = _serviceProvider.GetRequiredService<IImagesCacheRepository>();
            _imagesRepository = _serviceProvider.GetRequiredService<IImageRepository>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();

            Thread.Sleep(1000);
            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();

            DeleteCache(_database).Wait();
            _startIndex.CreateIndex().Wait();

        }

        [Fact(DisplayName = nameof(ShouldBuilderCacheImage))]
        [Trait("Integration - Application.Service", "Builder Image Cache")]

        public async Task ShouldBuilderCacheImage()
        {
            var productId = Guid.NewGuid();
            var listImages = FakerImages(productId, 10);

            foreach (var image in listImages)
            {
                await _imagesRepository.Create(image, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();

            var service = new ServiceBuilder.BuildCacheImage(_serviceProvider);
            await service.Handle();

            var Images = await _imagesCacheRepository.GetImageByProductId(productId);

            Assert.NotNull(Images);
            Assert.Equal(10, Images.Count());
        }



    }
}
