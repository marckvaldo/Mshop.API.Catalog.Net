using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.UseCases.Images.GetImage;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ApplicationUseCase = Mshop.Application.UseCases.Images.GetImage;

namespace Mshop.IntegrationTests.Application.UserCases.Images.GetImage
{
    [Collection("GetImage Collection")]
    [CollectionDefinition("GetImage Collection", DisableParallelization = true)]
    public class GetImageTest : IntegracaoBaseFixture, IDisposable
    {
        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public GetImageTest()
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

        [Fact(DisplayName = nameof(GetImage))]
        [Trait("Integration - Application.UseCase", "Image Use Case")]
        public async Task GetImage()
        {
            var imageFake = FakerImage(Guid.NewGuid());
            await _imageRepository.Create(imageFake,CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var guid = imageFake.Id;
            var useCase = new ApplicationUseCase.GetImage(_notification,_imageRepository,_storageService);
            var outPut = await useCase.Handle(new GetImageInPut(guid) ,CancellationToken.None);

            var result = outPut.Data;

            Assert.False(_notification.HasErrors());
            Assert.NotNull(result);
            Assert.Equal(result.ProductId, imageFake.ProductId);
            Assert.NotNull(result.Image);

        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
