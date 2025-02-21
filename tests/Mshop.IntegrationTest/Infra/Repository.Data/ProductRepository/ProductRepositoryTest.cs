using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MessageCore = Mshop.Core.Message;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Repository;
using Mshop.Infra.Data.UnitOfWork;
using Mshop.IntegrationTests.Infra.Repository.Data.CategoryRepository;
using DataRepository = Mshop.Infra.Data.Interface;
using DomainEntity = Mshop.Domain.Entity;
using Mshop.Core.Message;
using Mshop.Core.Data;
using Mshop.IntegrationTests.Common;


namespace Mshop.IntegrationTests.Infra.Repository.Data.ProductRepository
{
    [Collection("Repository Products Collection")]
    [CollectionDefinition("Repository Products Collection", DisableParallelization = true)]
    public class ProductRepositoryTest: IntegracaoBaseFixture
    {
        private readonly RepositoryDbContext _DbContext;
        private readonly DataRepository.IProductRepository _repositoryProduct;
        private readonly IUnitOfWork _unitOfWork;

        protected readonly ProductRepositoryPersistence _persistenceProduct;
        protected readonly CategoryRepositoryPertsistence _persistenceCategory;
        

        public ProductRepositoryTest() : base()
        {
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _repositoryProduct = _serviceProvider.GetRequiredService<DataRepository.IProductRepository>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

            _persistenceProduct = new ProductRepositoryPersistence(_DbContext);
            _persistenceCategory = new CategoryRepositoryPertsistence(_DbContext);
            

            DeleteDataBase(_DbContext,false).Wait();
            AddMigartion(_DbContext).Wait();
        }

        [Fact(DisplayName = nameof(CreateProduct))]
        [Trait("Integration - Infra.Data", "Product Repositorio")]

        public async Task CreateProduct()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            await _repositoryProduct.Create(product, CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);
            var newProduct = await _persistenceProduct.GetById(product.Id);

            Assert.NotNull(newProduct);
            Assert.Equal(product.Id, newProduct.Id);
            Assert.Equal(product.Name, newProduct.Name);
            Assert.Equal(product.Thumb.Path, newProduct.Thumb.Path);
            Assert.Equal(product.Price, newProduct.Price);
            Assert.Equal(product.Stock, newProduct.Stock);
            Assert.Equal(product.CategoryId, newProduct.CategoryId);
        }

        [Fact(DisplayName = nameof(GetByIdProduct))]
        [Trait("Integration - Infra.Data", "Product Repositorio")]

        public async Task GetByIdProduct()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);
            var productList = FakerProducts(20,category);
            
            productList.Add(product);
            await _persistenceProduct.CreateList(productList);
            var outPut = await _repositoryProduct.GetById(product.Id);

            Assert.NotNull(outPut);
            Assert.Equal(product.Id, outPut.Id);
            Assert.Equal(product.Name, outPut.Name);
            Assert.Equal(product.Thumb, outPut.Thumb);
            Assert.Equal(product.Price, outPut.Price);
            Assert.Equal(product.Stock, outPut.Stock);
            Assert.Equal(product.CategoryId, outPut.CategoryId);
        }

        [Fact(DisplayName = nameof(UpdateProduct))]
        [Trait("Integration - Infra.Data", "Product Repositorio")]

        public async Task UpdateProduct()
        {
            var category = FakerCategory();
            var productList = FakerProducts(20, category);
            Guid id = productList.First().Id;
            var imagem = FakerImage(id);
            var request = FakerProduct(category);

            await _persistenceProduct.CreateList(productList);
            var product = await _repositoryProduct.GetById(id);

            Assert.NotNull(product);

            product.Update(request.Description, request.Name, request.Price, request.CategoryId);
            product.UpdateThumb(request.Thumb.Path);
            product.UpdateQuantityStock(request.Stock);

            await _repositoryProduct.Update(product, CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);
            var productUpdate = await _persistenceProduct.GetById(id);

            Assert.NotNull(productUpdate);
            Assert.Equal(id, productUpdate.Id);
            Assert.Equal(request.Name, productUpdate.Name);
            Assert.Equal(request.Thumb.Path, productUpdate.Thumb.Path);
            Assert.Equal(request.Price, productUpdate.Price);
            Assert.Equal(request.Stock, productUpdate.Stock);
            Assert.Equal(request.CategoryId, productUpdate.CategoryId);
        }


        [Fact(DisplayName = nameof(DeleteProduct))]
        [Trait("Integration - Infra.Data", "Product Repositorio")]

        public async Task DeleteProduct()
        {
            var category = FakerCategory();
            var productList = FakerProducts(20,category);
            await _persistenceProduct.CreateList(productList);   

            var request = productList.First();
            await _repositoryProduct.DeleteById(request, CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);
            var productUpdate = await _persistenceProduct.GetById(request.Id);

            Assert.Null(productUpdate);
        }


        [Fact(DisplayName = nameof(FilterPaginated))]
        [Trait("Integration - Infra.Data", "Product Repositorio")]
        public async Task FilterPaginated()
        {
            var category = FakerCategory();
            await _persistenceCategory.Create(category);

            var productList = FakerProducts(20,category);
            await _persistenceProduct.CreateList(productList);

            var perPage = 10;
            var input = new Core.Paginated.PaginatedInPut(1, perPage, "","",Core.Enum.Paginated.SearchOrder.Asc);
            var outPut = await _repositoryProduct.FilterPaginatedQuery(input, Guid.Empty, false, CancellationToken.None);

            Assert.NotNull(outPut);
            Assert.NotNull(outPut.Data);
            Assert.True(outPut.Data.Count == perPage);
            Assert.Equal(input.PerPage, outPut.PerPage);

            foreach(DomainEntity.Product item in outPut.Data)
            {
                var product = productList.Find(x => x.Id == item.Id);
                Assert.NotNull(product);
                Assert.Equal(product.Name, item.Name);
                Assert.Equal(product.Description, item.Description);
                Assert.Equal(product.Price, item.Price);
                Assert.Equal(product.CategoryId, item.CategoryId);
            }

        }


        [Fact(DisplayName = nameof(SholdResultListEmptyFilterPaginated))]
        [Trait("Integration - Infra.Data", "Product Repositorio")]

        public async Task SholdResultListEmptyFilterPaginated()
        {
            var perPage = 20;
            var input = new Core.Paginated.PaginatedInPut(1, perPage, "", "", Core.Enum.Paginated.SearchOrder.Asc);
            var outPut = await _repositoryProduct.FilterPaginatedQuery(input, Guid.Empty, false, CancellationToken.None);

            Assert.NotNull(outPut);
            Assert.True(outPut.Data.Count == 0);
            Assert.True(outPut.Total == 0);
            Assert.Equal(input.PerPage, outPut.PerPage);
        }


        [Theory(DisplayName = nameof(SerachRestusPaginated))]
        [Trait("Integration - Infra.Data", "Product Repositorio")]
        [InlineData(10,1,10,10)]
        [InlineData(17, 2, 10,7)]
        [InlineData(17, 3, 10, 0)]

        public async Task SerachRestusPaginated(int quantityProduct, int page, int perPage, int expectedQuantityItems)
        {

            var category = FakerCategory();
            await _persistenceCategory.Create(category);

            var productList = FakerProducts(quantityProduct,category);
            await _persistenceProduct.CreateList(productList);

            var input = new Core.Paginated.PaginatedInPut(page, perPage, "", "", Core.Enum.Paginated.SearchOrder.Asc);
            var outPut = await _repositoryProduct.FilterPaginatedQuery(input, Guid.Empty, false, CancellationToken.None);

            Assert.NotNull(outPut);
            Assert.NotNull(outPut.Data);
            Assert.True(outPut.Data.Count == expectedQuantityItems);
            Assert.Equal(outPut.PerPage, perPage);   
            Assert.True(outPut.Total == quantityProduct);
            Assert.Equal(input.PerPage, outPut.PerPage);

            foreach (DomainEntity.Product item in outPut.Data)
            {
                var product = productList.Find(x => x.Id == item.Id);
                Assert.NotNull(product);
                Assert.Equal(product.Name, item.Name);
                Assert.Equal(product.Description, item.Description);
                Assert.Equal(product.Price, item.Price);
                Assert.Equal(product.CategoryId, item.CategoryId);
            }

        }

    }
}
