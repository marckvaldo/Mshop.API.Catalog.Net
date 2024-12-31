using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ApplicationUseCase = Mshop.Application.UseCases.Product.DeleteProduct;

namespace Mshop.IntegrationTests.Application.UserCases.Product.DeleteProduct
{
    [Collection("Delete Products Collection")]
    [CollectionDefinition("Delete Products Collection", DisableParallelization = true)]
    public class DeleteProductTest : IntegracaoBaseFixture, IDisposable
    {
        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public DeleteProductTest() :base()
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

        [Fact(DisplayName = nameof(DeleteProduct))]
        [Trait("Integration - Application.UseCase", "Product Use Case")]

        public async Task DeleteProduct()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);

            
            await _productRepository.Create(product, CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var useCase = new ApplicationUseCase.DeleteProduct(
                _productRepository, 
                _imageRepository, 
                _notification, 
                _storageService,
                _unitOfWork);

            var OutPut = await useCase.Handle(new ApplicationUseCase.DeleteProductInPut(product.Id), CancellationToken.None);
            var productDbDelete = await _productRepository.GetById(product.Id);

            Assert.Null(productDbDelete);
            Assert.True(OutPut.IsSuccess);
            Assert.False(_notification.HasErrors());

        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
