using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Mshop.Application.UseCases.Product.Common;
using Mshop.Application.UseCases.Product.ListProducts;
using Mshop.Core.Data;
using Mshop.Domain.Entity;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.Infra.Data.Repository;
using MShop.Catalog.E2ETest.Base;
using MShop.Catalog.E2ETests.API.Common;

namespace MShop.Catalog.E2ETests.API.Product
{
    [Collection("Crud Products Collection")]
    [CollectionDefinition("Crud Products Collection", DisableParallelization = true)]

    public class ProductAPITest : ProductAPITestFixture, IDisposable
    {
        private ICategoryRepository _categoryRepository;
        private ICategoryCacheRepository _categoryCacheRepository;
        private IProductRepository _productRepository;
        private IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;
        public ProductAPITest() : base()
        {
            _categoryCacheRepository = _serviceProvider.GetRequiredService<ICategoryCacheRepository>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>(); 
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();
        }

        [Fact(DisplayName = nameof(CreateProductAPI))]
        [Trait("EndToEnd/API","Product - Endpoints")]
        public async Task CreateProductAPI()
        {
            var request = await RequestCreate();
            var (response, outPut) = await _apiClient.Post<CustomResponse<ProductModelOutPut>>(Configuration.URL_API_PRODUCT, request);

            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode) ;
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(outPut.Data.Name, request.Name);
            Assert.Equal(outPut.Data.Description, request.Description);  
            Assert.Equal(outPut.Data.Price,request.Price);   
            Assert.Equal(outPut.Data.IsActive,request.IsActive);

            var dbProduct = (await _productRepository.Filter(p => p.Id == outPut.Data.Id)).FirstOrDefault();

            Assert.NotNull(dbProduct);
            Assert.Equal(dbProduct.Name, request.Name);            
            Assert.Equal(dbProduct.Description, request.Description);
            Assert.Equal(dbProduct.Price, request.Price);
            Assert.Equal(dbProduct.IsActive, request.IsActive);
        }


        [Fact(DisplayName = nameof(UpdateProduct))]
        [Trait("EndToEnd/API", "Product - Endpoints")]
        public async Task UpdateProduct()
        {

            var product = FakerProduct(FakerCategory());
            await _productRepository.Create(product,CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);

            var request = await RequestUpdate();
            request.Id = product.Id;
                                   

            var (response, output) = await _apiClient.Put<CustomResponse<ProductModelOutPut>>(
            $"{Configuration.URL_API_PRODUCT}{request.Id}",
            request);

            var dbProduct = (await _productRepository.Filter(p => p.Id == output.Data.Id)).FirstOrDefault();

            Assert.NotNull(response);
            Assert.NotNull(dbProduct);
            Assert.NotNull(output);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.True(output.Success);
            Assert.Equal(dbProduct.Name, output.Data.Name);
            Assert.Equal(dbProduct.Description, output.Data.Description);
            Assert.Equal(dbProduct.Id, output.Data.Id);
            Assert.Equal(dbProduct.Price, output.Data.Price);
        }



        [Fact(DisplayName = nameof(DeleteProduct))]
        [Trait("EndToEnd/API", "Product - Endpoints")]
        public async Task DeleteProduct()
        {
            var product = FakerProduct(FakerCategory());
            await _productRepository.Create(product, CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);

            var (response, output) = await _apiClient.Delete<CustomResponse<ProductModelOutPut>>($"{Configuration.URL_API_PRODUCT}{product.Id}");

            var dbProduct = (await _productRepository.Filter(p => p.Id == output.Data.Id)).FirstOrDefault();

            Assert.NotNull(response);
            Assert.NotNull(output);
            Assert.True(output.Success);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.Equal(product.Id, output.Data.Id);
            Assert.Equal(product.Name, output.Data.Name);   
            Assert.Equal(product.Price, output.Data.Price); 
            Assert.Equal(product.Description, output.Data.Description);
            Assert.Null(dbProduct);
        }


        [Fact(DisplayName = nameof(UpdateProductStock))]
        [Trait("EndToEnd/API", "Product - Endpoints")]
        public async Task UpdateProductStock()
        {
            var product = FakerProduct(FakerCategory());
            await _productRepository.Create(product, CancellationToken.None);
            await _unitOfWork.CommitAsync(CancellationToken.None);

            var productStock = FakerProduct(FakerCategory());
            var stock = productStock.Stock;

            var (response, outPut) = await _apiClient.Post<CustomResponse<ProductModelOutPut>>(
                $"{Configuration.URL_API_PRODUCT}update-stock/{product.Id}",new {product.Id, stock });

            var dbProduct = (await _productRepository.Filter(p => p.Id == outPut.Data.Id)).FirstOrDefault();

            Assert.NotNull(response);
            Assert.NotNull(outPut);
            Assert.NotNull(dbProduct);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(product.Name, outPut.Data.Name);
            Assert.Equal(product.Description, outPut.Data.Description);
            Assert.Equal(product.Price, outPut.Data.Price);
            Assert.Equal(dbProduct.Stock, stock);
        }


        [Theory(DisplayName = nameof(SholdReturnErrorWhenCantCreatePoduct))]
        [Trait("EndToEnd/API", "Product - Endpoints")]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task SholdReturnErrorWhenCantCreatePoduct(decimal price)
        {
            var request = await RequestCreate();
            request.Price = price;

            var (response, outPut) = await _apiClient.Post<CustomResponseErro>(Configuration.URL_API_PRODUCT, request);

            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Errors.Count > 0);
        }


        [Theory(DisplayName = nameof(SholdReturnErrorWhenCantUpdatePoduct))]
        [Trait("EndToEnd/API", "Product - Endpoints")]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task SholdReturnErrorWhenCantUpdatePoduct(decimal price)
        {
            var request =  await RequestUpdate();
            request.Price = price;

            var (response, outPut) = await _apiClient.Post<CustomResponseErro>(Configuration.URL_API_PRODUCT, request);

            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Errors.Count > 0);

        }


        [Fact(DisplayName = nameof(GetProductById))]
        [Trait("EndToEnd/API", "Product - Endpoints")]
        public async Task GetProductById()
        {
            var products = FakerProducts(20, FakerCategory());
            foreach (var item in products)
            {
                await _productRepository.Create(item, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();

            var product = products[3];
           
            var (response, outPut) = await _apiClient.Get<CustomResponse<ProductModelOutPut>>($"{Configuration.URL_API_PRODUCT}{product.Id}");

            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.True(outPut.Data.Id == product.Id);  
        }


        [Fact(DisplayName = nameof(ListProduct))]
        [Trait("EndToEnd/API", "Product - Endpoints")]
        public async Task ListProduct()
        {
            var products = FakerProducts(20, FakerCategory());
            foreach (var item in products)
            {
                await _productRepository.Create(item, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();

            var productDbBefore = (await _categoryRepository.Filter(p=>p.Name != null)).ToList(); 
            
            var (response, outPut) = await _apiClient.Get<CustomResponse<ListProductsOutPut>>($"{Configuration.URL_API_PRODUCT}list-products/");

            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.True(productDbBefore.Count() == outPut.Data.Total);
            Assert.True(outPut.Data.PerPage == 15);
            Assert.True(outPut.Data.Page == 1);

            foreach (var item in outPut.Data.Itens)
            {
                var expectItem = products.FirstOrDefault(x => x.Id == item.Id);

                Assert.NotNull(expectItem);
                Assert.Equal(expectItem.Name, item.Name);
                Assert.Equal(expectItem.Description, item.Description);
                Assert.Equal(expectItem.Price, item.Price);
            }

        }


        [Fact(DisplayName = nameof(ListProductWhenItemsEmptyDefault))]
        [Trait("EndToEnd/API", "Product - Endpoints")]
        public async Task ListProductWhenItemsEmptyDefault()
        {
            var (response, outPut) = await _apiClient.Get<CustomResponse<ListProductsOutPut>>($"{Configuration.URL_API_PRODUCT}list-products/");

            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.True(outPut.Data.PerPage == 15);
            Assert.True(outPut.Data.Page == 1);
            Assert.True(outPut.Data.Itens.Count() == 0);
        }


        [Theory(DisplayName = nameof(ListProductWithPaginated))]
        [Trait("EndToEnd/API", "Product - Endpoints")]
        [InlineData(10, 1, 10, 10)]
        [InlineData(17, 2, 10, 7)]
        [InlineData(17, 3, 10, 0)]
        public async Task ListProductWithPaginated(int quantityProduct, int page, int perPage, int expectedQuantityItems)
        {
            var products = FakerProducts(20, FakerCategory());
            foreach (var item in products)
            {
                await _productRepository.Create(item, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();

            var request = new ListProductInPut(page, perPage, "", "", Mshop.Core.Enum.Paginated.SearchOrder.Desc);
            var (response, outPut) = await _apiClient.Get<CustomResponse<ListProductsOutPut>>($"{Configuration.URL_API_PRODUCT}list-products/", request);

            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.True(outPut.Data.PerPage == perPage);
            Assert.True(outPut.Data.Page == page);
            Assert.True(outPut.Data.Itens.Count() == expectedQuantityItems);

            foreach (var item in outPut.Data.Itens)
            {
                var expectItem = products.FirstOrDefault(x => x.Id == item.Id);

                Assert.NotNull(expectItem);
                Assert.Equal(expectItem.Name, item.Name);
                Assert.Equal(expectItem.Description, item.Description);
                Assert.Equal(expectItem.Price, item.Price);
                //Assert.Equal(expectItem.Thumb.Path, item.Imagem);
            }
        }

        public void Dispose()
        {
            //TearDownRabbitMQ();
        }

        /*
        [Fact(DisplayName = nameof(ListProductPromotions))]
        [Trait("EndToEnd/API", "Product - Endpoints")]
        public async void ListProductPromotions()
        {
            var products = await GetProducts(5);
            await Persistence.CreateList(products);

            var (response, outPut) = await apiClient.Get<CustomResponse<List<ProductModelOutPut>>>($"{Configuration.URL_API_PRODUCT}list-products-promotions");

            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);

            foreach(var item in outPut.Data)
            {
                var expectItem = products.FirstOrDefault(p=>p.Id == item.Id);
                Assert.NotNull(expectItem);
                Assert.Equal(expectItem.Name, item.Name);   
                Assert.Equal(expectItem.Description,expectItem.Description);
                Assert.Equal(expectItem.Thumb, expectItem.Thumb);
                Assert.Equal(expectItem.Price, expectItem.Price);
                Assert.Equal(expectItem.Activate, expectItem.Activate); 
            }
        }


        [Fact(DisplayName = nameof(SholdReturnErrorWhenCantGetProductPromotion))]
        [Trait("EndToEnd/API", "Product - Endpoints")]
        public async void SholdReturnErrorWhenCantGetProductPromotion()
        {
            await ProductPersistenceCache.DeleteKey("promocao");
            var (response, outPut) = await apiClient.Get<CustomResponseErro>($"{Configuration.URL_API_PRODUCT}list-products-promotions");

            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.False(outPut.Success);
        }
        */
    }
}
