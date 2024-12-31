using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ApplicationUseCase = Mshop.Application.UseCases.Product.GetProduct;

namespace Mshop.IntegrationTests.Application.UserCases.Product.GetProduct
{

    [Collection("Get Products Collection")]
    [CollectionDefinition("Get Products Collection", DisableParallelization = true)]
    public class GetProductTest:IntegracaoBaseFixture, IDisposable
    {
        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public GetProductTest() : base()
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

        [Fact(DisplayName = nameof(GetProduct))]
        [Trait("Integration - Application.UseCase", "Product Use Case")]
        public async Task GetProduct()
        {

            var category = FakerCategory();
            await _categoryRepository.Create(category,CancellationToken.None);

            var productFake = FakerProduct(category);
            await _productRepository.Create(productFake,CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);
           
            var guid = productFake.Id;
            var useCase = new ApplicationUseCase.GetProduct(_productRepository,  _imageRepository ,_notification);
            var outPut = await useCase.Handle(new ApplicationUseCase.GetProductInPut(guid), CancellationToken.None);

            var result = outPut.Data;

            Assert.False(_notification.HasErrors());
            Assert.NotNull(result);
            Assert.Equal(result.Name, productFake.Name);
            Assert.Equal(result.Description, productFake.Description);
            Assert.Equal(result.Price, productFake.Price);
            Assert.Equal(result.CategoryId, productFake.CategoryId);
            Assert.Equal(result.Stock, productFake.Stock);
            Assert.Equal(result.IsActive, productFake.IsActive);

        }


        [Fact(DisplayName = nameof(SholdReturnErrorWhenCantGetProduct))]
        [Trait("Integration - Application.UseCase", "Product Use Case")]
        public async Task SholdReturnErrorWhenCantGetProduct()
        {
            var category = FakerCategory();
            await _categoryRepository.Create(category, CancellationToken.None);

            var productFake = FakerProduct(category);
            await _productRepository.Create(productFake, CancellationToken.None);
            await _unitOfWork.CommitAsync();
            //await _DbContext.Products.AddAsync(productFake);
            //await _DbContext.SaveChangesAsync();

            var useCase = new ApplicationUseCase.GetProduct(_productRepository, _imageRepository, _notification);
            //var outPut = async () => await useCase.Handle(new ApplicationUseCase.GetProductInPut(Guid.NewGuid()), CancellationToken.None);
            //var exception = await Assert.ThrowsAsync<ApplicationValidationException>(outPut);
            //Assert.Equal("Error", exception.Message);

            var outPut = await useCase.Handle(new ApplicationUseCase.GetProductInPut(Guid.NewGuid()), CancellationToken.None);

            Assert.True(_notification.HasErrors());
            Assert.False(outPut.IsSuccess);
        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
