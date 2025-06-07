using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using ApplicationUseCase = Mshop.Application.UseCases.Product.CreateProducts;

namespace Mshop.IntegrationTests.Application.UserCases.Product.CreateProduct
{
    [Collection("Create Products Collection")]
    [CollectionDefinition("Create Products Collection", DisableParallelization = true)]
    public class CreateProductTest : CreateProductTestFixture, IDisposable
    {

        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public CreateProductTest() : base()
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


        [Fact(DisplayName = nameof(CreateProduct))]
        [Trait("Integration - Application.UseCase", "Product Use Case")]
        public async Task CreateProduct()
        {
            var categoryFake = FakerCategory();
            var request = FakerCrateProductInPut(categoryFake.Id);
            
            await _categoryRepository.Create(categoryFake, CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var categoryDb = await _categoryRepository.GetById(categoryFake.Id);  
            Assert.NotNull(categoryDb);
            request.CategoryId = categoryDb.Id;

            var productUseCase = new ApplicationUseCase.CreateProduct(
                _productRepository, 
                _notification,
                _categoryRepository, 
                _storageService, 
                _unitOfWork);

            var outPut = await productUseCase.Handle(request, CancellationToken.None);

            var result = outPut.Data;
            var newProduct = await _productRepository.GetById(result.Id);
            
           

            Assert.False(_notification.HasErrors());
            Assert.True(outPut.IsSuccess);
            Assert.NotNull(outPut);
            Assert.NotNull(newProduct);
            Assert.Equal(result.Name, newProduct.Name);
            Assert.Equal(result.Description, newProduct.Description);
            Assert.Equal(result.Price, newProduct.Price);
            Assert.Equal(result.CategoryId, newProduct.CategoryId);
            Assert.Equal(result.Stock, newProduct.Stock);
            Assert.Equal(result.IsActive, newProduct.IsActive);

            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Price, result.Price);
            Assert.Equal(request.CategoryId, result.CategoryId);
            Assert.Equal(request.Stock, result.Stock);
            Assert.Equal(request.IsActive, result.IsActive);

        }


        [Fact(DisplayName = nameof(SholdReturnErrorWhenCreateProductWithOutCategory))]
        [Trait("Integration - Application.UseCase", "Product Use Case")]
        public async Task SholdReturnErrorWhenCreateProductWithOutCategory()
        {
            var request = FakerCrateProductInPut(Guid.NewGuid());
            var productUseCase = new ApplicationUseCase.CreateProduct(
                _productRepository, 
                _notification, 
                _categoryRepository, 
                _storageService, 
                _unitOfWork);

            //var outPut = async () => await productUseCase.BuildCache(request, CancellationToken.None);
            //var exception = await Assert.ThrowsAsync<ApplicationValidationException>(outPut);
            //Assert.Equal("Error", exception.Message);

            var outPut = await productUseCase.Handle(request, CancellationToken.None);
            Assert.True(_notification.HasErrors());
            Assert.False(outPut.IsSuccess);

        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
