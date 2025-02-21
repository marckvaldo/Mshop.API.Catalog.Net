using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.UseCases.Product.ListProducts;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;

namespace Mshop.IntegrationTests.Application.UserCases.Product.ListProduct
{
    [Collection("List Products Collection")]
    [CollectionDefinition("List Products Collection", DisableParallelization = true)]
    public class ListProductTest : IntegracaoBaseFixture, IDisposable
    {

        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public ListProductTest() : base()
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

        [Fact(DisplayName = nameof(ListProduct))]
        [Trait("Integration - Application.UseCase", "Product Use Case")]

        public async Task ListProducts()
        {
            var category = FakerCategory();
            await _categoryRepository.Create(category,CancellationToken.None);

            var productsFake = FakerProducts(20,category);
            foreach (var product in productsFake)
            {
                await _productRepository.Create(product,CancellationToken.None);
            }

            await _unitOfWork.CommitAsync();

            var useCase = new ListProducts(_productRepository, _notification);
            var request = new ListProductInPut(
                            page: 1,
                            perPage:5,
                            search: "",
                            sort: "name",
                            dir: Mshop.Core.Enum.Paginated.SearchOrder.Asc,false,Guid.Empty
                            );

            var outPut = await useCase.Handle(request, CancellationToken.None);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(productsFake.Count, result.Total);
            Assert.Equal(request.Page, result.CurrentPage);
            Assert.Equal(request.PerPage, result.PerPage);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Any());
        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }
    }
}
