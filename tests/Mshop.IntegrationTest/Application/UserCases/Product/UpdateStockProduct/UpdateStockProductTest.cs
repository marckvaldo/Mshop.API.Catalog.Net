using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using ApplicationUseCase = Mshop.Application.UseCases.Product.UpdateStockProduct;

namespace Mshop.IntegrationTests.Application.UserCases.Product.UpdateStockProduct
{
    [Collection("Update stock Products Collection")]
    [CollectionDefinition("Update stock Products Collection", DisableParallelization = true)]
    public class UpdateStockProductTest : UpdateStockProductTestFixture, IDisposable
    {

        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public UpdateStockProductTest()
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

        [Fact(DisplayName = nameof(UpdateStockProduct))]
        [Trait("Integration - Application.UseCase", "Product Use Case")]

        public async Task UpdateStockProduct()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            var request = FakerUpdateStockProdjctInPut(product.Id);

            await _productRepository.Create(product,CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var useCase = new ApplicationUseCase.UpdateStockProducts(_productRepository, _notification, _unitOfWork, _categoryRepository);
            var outPut = await useCase.Handle(request, CancellationToken.None);
            var outPutDb = await _productRepository.GetById(product.Id);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(request.Stock, result.Stock);  
            Assert.Equal(request.Stock, outPutDb.Stock);

        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
