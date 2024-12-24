using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Enum.Paginated;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.Infra.Data.Repository;
using Mshop.IntegrationTests.Common;
using Mshop.IntegrationTests.Infra.Repository.Data.CategoryRepository;

namespace Mshop.IntegrationTests.Infra.Repository.Data.ImagesRepository
{
    [Collection("Repository Image Collection")]
    [CollectionDefinition("Repository Image Collection", DisableParallelization = true)]
    public class ImagesRepositoryTest : IntegracaoBaseFixture
    {
        private readonly IImageRepository _imagesRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly RepositoryDbContext _DbContext;
        private readonly ImagesRepositoryPertsistence _persistenceImage;

        public ImagesRepositoryTest()
        {
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _imagesRepository = _serviceProvider.GetRequiredService<IImageRepository>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

            _persistenceImage = new ImagesRepositoryPertsistence(_DbContext);

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();
        }

        [Fact(DisplayName = nameof(CreateImage))]
        [Trait("Integration - Infra.Data", "Image Repositorio")]

        public async Task CreateImage()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            var request = FakerImage(product.Id);

            await _imagesRepository.Create(request, CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var newImage = await _imagesRepository.GetById(request.Id);

            Assert.NotNull(newImage);
            Assert.Equal(newImage.Id, request.Id);
            Assert.Equal(newImage.ProductId, request.ProductId);
            Assert.Equal(newImage.FileName, request.FileName);
        }


        [Fact(DisplayName = nameof(GetImageByProductId))]
        [Trait("Integration - Infra.Data", "Image Repositorio")]

        public async void GetImageByProductId()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            var listImages = FakerImages(product.Id);
            var listImages2 = FakerImages(Guid.NewGuid());

            listImages = listImages.Union(listImages2).ToList();
                
            foreach(var image in listImages)
            {
                await _imagesRepository.Create(image, CancellationToken.None);
            }

            await _unitOfWork.CommitAsync();
            
            var images = await _imagesRepository.GetImagesByProductId(product.Id);

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
        [Trait("Integration - Infra.Data", "Image Repositorio")]

        public async Task DeleteImages()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            var listImages = FakerImages(product.Id);
            var listImages2 = FakerImages(Guid.NewGuid());

            listImages = listImages.Union(listImages2).ToList();

            foreach (var image in listImages)
            {
                await _imagesRepository.Create(image, CancellationToken.None);
            }
           
            await _unitOfWork.CommitAsync();
            
            var imageDelete = listImages.First();
            await _imagesRepository.DeleteById(imageDelete, CancellationToken.None);

            await _unitOfWork.CommitAsync();

            var images = await _imagesRepository.GetImagesByProductId(product.Id);
      
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


        [Fact(DisplayName = nameof(DeleteImagesByProductId))]
        [Trait("Integration - Infra.Data", "Image Repositorio")]

        public async Task DeleteImagesByProductId()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            var listImages = FakerImages(product.Id);
          
            foreach (var image in listImages)
            {
                await _imagesRepository.Create(image, CancellationToken.None);
            }

            await _unitOfWork.CommitAsync();

            await _imagesRepository.DeleteByIdProduct(product.Id);

            await _unitOfWork.CommitAsync();

            var images = await _imagesRepository.GetImagesByProductId(product.Id);

            Assert.Equal(0,images.Count());
        }


    }
}
