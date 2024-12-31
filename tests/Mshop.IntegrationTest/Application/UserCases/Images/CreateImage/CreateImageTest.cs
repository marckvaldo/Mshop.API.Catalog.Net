using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Application.UserCases.Images.CreateImage;
using ApplicationUseCase = Mshop.Application.UseCases.Images.CreateImage;


namespace Mshop.IntegrationTests.Application.UserCases.Images.CreateImages
{
    [Collection("Create Image Collection")]
    [CollectionDefinition("Create Image Collection", DisableParallelization = true)]
    public class CreateImageTest : CreateImageTestFixture, IDisposable
    {
        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;


        public CreateImageTest() : base()
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

        [Fact(DisplayName = nameof(CreateImage))]
        [Trait("Integration - Application.UseCase", "Image Use Case")]
        public async Task CreateImage()
        {
            var category = FakerCategory();
            await _categoryRepository.Create(category,CancellationToken.None);
            
            var product = FakerProduct(category);
            await _productRepository.Create(product,CancellationToken.None);
            
            await _unitOfWork.CommitAsync(CancellationToken.None);

            var request = FakerCreateImageInPut(product.Id);

            var useCase = new ApplicationUseCase.CreateImage(
                _imageRepository, 
                _storageService, 
                _productRepository, 
                _notification,
                _unitOfWork);

            var outPut = await useCase.Handle(request,CancellationToken.None);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.True(result.Images.Count == 3);
            Assert.False(_notification.HasErrors());
        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
