using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.UseCases.Images.ListImage;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ApplicationUseCase = Mshop.Application.UseCases.Images.ListImage;

namespace Mshop.IntegrationTests.Application.UserCases.Images.ListImage
{
    [Collection("List Image Collection")]
    [CollectionDefinition("List Image Collection", DisableParallelization = true)]
    public class ListImageTest : IntegracaoBaseFixture, IDisposable
    {
        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public ListImageTest()
        {
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _imageRepository = _serviceProvider.GetRequiredService<IImageRepository>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _storageService = _serviceProvider.GetRequiredService<IStorageService>();
            _notification = _serviceProvider.GetRequiredService<INotification>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();
        }

        [Fact(DisplayName = nameof(ListImage))]
        [Trait("Integration - Application.UseCase", "Image Use Case")]
        public async Task ListImage()
        {
            var quantity = 20;
            var product = FakerProduct(FakerCategory());
            var productId = product.Id;
            var imagesFake = FakerImages(productId, quantity);
            foreach(var item in imagesFake)
            {
                await _imageRepository.Create(item, CancellationToken.None);
            }
            await _productRepository.Create(product, CancellationToken.None);

            await _unitOfWork.CommitAsync(CancellationToken.None);
            
               
            var useCase = new ApplicationUseCase.ListImage(_notification, _imageRepository, _productRepository);
            var outPut = await useCase.Handle(new ListImageInPut(productId), CancellationToken.None);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(imagesFake.Count, quantity);
            Assert.Equal(result.Images.Count, quantity);
            foreach (var item in result.Images) 
            {
                var image = result.Images.Where(i => i.Image == item.Image);
                Assert.NotNull(image);
            }
        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
