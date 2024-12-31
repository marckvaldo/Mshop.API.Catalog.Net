using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ApplicationUseCase = Mshop.Application.UseCases.Images.DeleteImage;

namespace Mshop.IntegrationTests.Application.UserCases.Images.DeleteImage
{
    [Collection("Delete Image Collection")]
    [CollectionDefinition("Delete Image Collection", DisableParallelization = true)]
    public class DeleteImageTest : IntegracaoBaseFixture, IDisposable
    {
        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public DeleteImageTest()
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

        [Fact(DisplayName = nameof(DeleteImage))]
        [Trait("Integration - Application.UseCase", "Image Use Case")]

        public async Task DeleteImage()
        {
            var image = FakerImage(Guid.NewGuid());

            await _imageRepository.Create(image,CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var useCase = new ApplicationUseCase.DeleteImage(
                _imageRepository,
                _storageService,
                _notification,
                _unitOfWork);

            await useCase.Handle(new ApplicationUseCase.DeleteImageInPut(image.Id), CancellationToken.None);

            var imageDb = await _imageRepository.GetById(image.Id);

            Assert.Null(imageDb);

        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
