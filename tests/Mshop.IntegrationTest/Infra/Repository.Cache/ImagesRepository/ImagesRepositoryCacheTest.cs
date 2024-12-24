using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Enum.Paginated;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.IntegrationTests.Common;

namespace Mshop.IntegrationTests.Infra.Repository.Cache.ImagesRepository
{
    [Collection("Repository Image Collection")]
    [CollectionDefinition("Repository Image Collection", DisableParallelization = true)]
    public class ImagesRepositoryCacheTest : IntegracaoBaseFixture
    {
        private readonly IImagesCacheRepository _imagesRepositoryCache;
        private readonly StackExchange.Redis.IDatabase _database;
        private readonly DateTime _expirationDate;
        private readonly StartIndex _startIndex;

        public ImagesRepositoryCacheTest()
        {
            var connection = _serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
            _database = connection.GetDatabase();
            _imagesRepositoryCache = _serviceProvider.GetRequiredService<IImagesCacheRepository>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();
            _expirationDate = DateTime.UtcNow.AddHours(1);

            
            DeleteCache(_database).Wait();
            _startIndex.CreateIndex().Wait();

        }

        [Fact(DisplayName = nameof(CreateImage))]
        [Trait("Integration - Infra.Cache", "Image Repositorio")]

        public async Task CreateImage()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            var request = FakerImage(product.Id);

            await _imagesRepositoryCache.Create(request, _expirationDate, CancellationToken.None);
            var newImage = await _imagesRepositoryCache.GetById(request.Id);

            Assert.NotNull(newImage);
            Assert.Equal(newImage.Id, request.Id);
            Assert.Equal(newImage.ProductId, request.ProductId);
            Assert.Equal(newImage.FileName, request.FileName);
        }


        [Fact(DisplayName = nameof(GetImageByProductId))]
        [Trait("Integration - Infra.Cache", "Image Repositorio")]

        public async void GetImageByProductId()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            var listImages = FakerImages(product.Id);
            var listImages2 = FakerImages(Guid.NewGuid());

            listImages = listImages.Union(listImages2).ToList();
                
            foreach(var image in listImages)
            {
                await _imagesRepositoryCache.Create(image, _expirationDate, CancellationToken.None);
            }
            
            var images = await _imagesRepositoryCache.GetImageByProductId(product.Id);

            Assert.NotNull(images);
            Assert.Equal(images.Count(),listImages.Where(p => p.ProductId == product.Id).Count());
            foreach (var image in images)
            {
                var newImage = images.Where(x=>x.Id == image.Id).Where(x=>x.ProductId == x.ProductId).FirstOrDefault();
                Assert.NotNull(newImage);
                Assert.Equal(newImage.Id, image.Id);
                Assert.Equal(newImage.ProductId, image.ProductId);
                Assert.Equal(newImage.FileName, image.FileName);
            }
               
        }


        [Fact(DisplayName = nameof(DeleteImages))]
        [Trait("Integration - Infra.Cache", "Image Repositorio")]

        public async Task DeleteImages()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            var listImages = FakerImages(product.Id);
            var listImages2 = FakerImages(Guid.NewGuid());

            listImages = listImages.Union(listImages2).ToList();

            foreach (var image in listImages)
            {
                await _imagesRepositoryCache.Create(image, _expirationDate, CancellationToken.None);
            }
           
            
            var imageDelete = listImages.First();
            await _imagesRepositoryCache.DeleteById(imageDelete, CancellationToken.None);
       
            var images = await _imagesRepositoryCache.GetImageByProductId(product.Id);
      
            Assert.NotNull(images);
            Assert.Equal(0, images.Where(x => x.Id == imageDelete.Id).Count());

            foreach (var image in images)
            {
                var newImage = images.Where(x => x.Id == image.Id).FirstOrDefault();
                
                Assert.NotNull(newImage);
                Assert.Equal(newImage.Id, image.Id);
                Assert.Equal(newImage.ProductId, image.ProductId);
                Assert.Equal(newImage.FileName, image.FileName);
            }            
        }
     

    }
}
