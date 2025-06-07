using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.UseCases.Images.Common;
using Mshop.Catalog.E2ETest.Base;
using Mshop.Catalog.E2ETests.API.Common;
using Mshop.Core.Data;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;

namespace Mshop.Catalog.E2ETests.API.Images
{
    [Collection("Crud Image Collection")]
    [CollectionDefinition("Crud Image Collection", DisableParallelization = true)]
    public class ImageAPITest : ImageAPITestFixture
    {
        private ICategoryRepository _categoryRepository;
        private IProductRepository _productRepository;
        private IImageRepository _imageRepository;
        private IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public ImageAPITest()
        {
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _productRepository = _serviceProvider.GetRequiredService<IProductRepository>();
            _imageRepository = _serviceProvider.GetRequiredService<IImageRepository>();

            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();
        }

        [Fact(DisplayName = nameof(CreateImage))]
        [Trait("EndToEnd/API", "Image - Endpoints")]
        
        public async Task CreateImage()
        {
            var category = FakerCategory();
            var product = FakerProduct(category);

            await _categoryRepository.Create(category, CancellationToken.None);
            await _productRepository.Create(product, CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var request = await FakeRequest(product.Id);
            var (response, outPut) = await _apiClient.Post<CustomResponse<ListImageOutPut>>(Configuration.URL_API_IMAGE, request);

            Assert.NotNull(request);
            Assert.NotNull(request.Images);
            Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(outPut.Data.Images.Count(), request.Images.Count());
            Assert.Equal(outPut.Data.ProductId, request.ProductId);
            
        }



        [Fact(DisplayName = nameof(DeleteImage))]
        [Trait("EndToEnd/API", "Image - Endpoints")]

        public async Task DeleteImage()
        {

            var product = FakerProduct(FakerCategory());
            var images = FakerImages(product.Id);

            await _productRepository.Create(product, CancellationToken.None);

            foreach (var image in images)
            {
                await _imageRepository.Create(image, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();

            var request = images.First();

            var (response, outPut) = await _apiClient.Delete<CustomResponse<ImageOutPut>>($"{Configuration.URL_API_IMAGE}{request.Id}");

            Assert.NotNull(request);
            Assert.NotNull(images);
            Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(outPut.Data.Image.Image, request.FileName);
            Assert.Equal(outPut.Data.ProductId, request.ProductId);
        }


        [Fact(DisplayName = nameof(GetImage))]
        [Trait("EndToEnd/API", "Image - Endpoints")]

        public async void GetImage()
        {
            var product = FakerProduct(FakerCategory());
            var images = FakerImages(product.Id);

            await _productRepository.Create(product, CancellationToken.None);

            foreach (var image in images)
            {
                await _imageRepository.Create(image, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();

            var request = images.First();

            var (response, outPut) = await _apiClient.Get<CustomResponse<ImageOutPut>>($"{Configuration.URL_API_IMAGE}{request.Id}");

            Assert.NotNull(request);
            Assert.NotNull(images);
            Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(outPut.Data.Image.Image, request.FileName);
            Assert.Equal(outPut.Data.ProductId, request.ProductId);
        }


        [Fact(DisplayName = nameof(ListImageAPI))]
        [Trait("EndToEnd/API", "Image - Endpoints")]

        public async void ListImageAPI()
        {
            var product = FakerProduct(FakerCategory());
            var images = FakerImages(product.Id);

            await _productRepository.Create(product, CancellationToken.None);

            foreach (var image in images)
            {
                await _imageRepository.Create(image, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();    
            var request = images.First();

            var (response, outPut) = await _apiClient.Get<CustomResponse<ListImageOutPut>>($"{Configuration.URL_API_IMAGE}list-Images-by-productId/{request.ProductId}");

            Assert.NotNull(request);
            Assert.NotNull(images);
            Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(outPut.Data.Images.Count(), images.Count());
            Assert.Equal(outPut.Data.ProductId, request.ProductId);

        }
    }
}
