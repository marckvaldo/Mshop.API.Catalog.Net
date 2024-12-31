using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using ApplicationUseCase = Mshop.Application.UseCases.Product.UpdateProduct;

namespace Mshop.IntegrationTests.Application.UserCases.Product.UpdateProduct
{
    [Collection("Update Products Collection")]
    [CollectionDefinition("Update Products Collection", DisableParallelization = true)]
    public class UpdateProductTest : UpdateProdutTestFixture, IDisposable
    {

        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public UpdateProductTest()
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

        [Fact(DisplayName = nameof(UpdateProduct))]
        [Trait("Integration - Application.UseCase", "Product Use Case")]

        public async Task UpdateProduct()
        {
           
            //var notificacao = new Notifications();

            var category = FakerCategory();
            var product = FakerProduct(category);
            
            var request = RequestFake(product.Id,category.Id);

            await _productRepository.Create(product,CancellationToken.None);
            await _categoryRepository.Create(category,CancellationToken.None);  
            await _unitOfWork.CommitAsync();

            var useCase = new ApplicationUseCase.UpdateProduct(
                _productRepository, 
                _categoryRepository,
                _notification,
                _storageService,
                _unitOfWork);

            var outPut = await useCase.Handle(request, CancellationToken.None);

            var productDb = await _productRepository.GetById(product.Id);

            var result = outPut.Data;

            Assert.False(_notification.HasErrors());
            Assert.NotNull(outPut);
            Assert.NotNull(productDb);
            Assert.Equal(result.Name, productDb.Name);  
            Assert.Equal(result.Description, productDb.Description);  
            Assert.Equal(result.Price, productDb.Price);  
            Assert.Equal(result.CategoryId, productDb.CategoryId);
            Assert.NotEmpty(result.Name);
        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
