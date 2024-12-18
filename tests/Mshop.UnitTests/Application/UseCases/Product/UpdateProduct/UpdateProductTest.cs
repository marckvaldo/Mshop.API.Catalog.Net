using Moq;
using Mshop.Core.Data;
using Mshop.Core.Exception;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Interface;
using DomainEntity = Mshop.Domain.Entity;
using useCase = Mshop.Application.UseCases.Product.UpdateProduct;

namespace Mshop.Application.UseCases.Product.UpdateProduct
{
    public class UpdateProductTest : UpdateProductTestFixture
    {
        private readonly Mock<IProductRepository> _productRepository;
        private readonly Mock<INotification> _notifications;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IStorageService> _storageService;
        private readonly Mock<ICategoryRepository> _repositoryCategory;

        public UpdateProductTest()
        {
            _productRepository = new Mock<IProductRepository>();
            _notifications = new Mock<INotification>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _storageService = new Mock<IStorageService>();
            _repositoryCategory = new Mock<ICategoryRepository>();
        }

        [Fact(DisplayName = nameof(UpdateProduct))]
        [Trait("Application-UseCase", "Update Product")]
        public async void UpdateProduct()
        {
            var request = ProductInPut();
            var productRepository = ProductModelOutPut();

            var productFake = Faker();

            _productRepository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync(productFake);

            var categoryFake = FakerCategory();

            _repositoryCategory.Setup(c => c.GetById(It.IsAny<Guid>())).ReturnsAsync(categoryFake);

            _storageService.Setup(s => s.Upload(It.IsAny<string>(), It.IsAny<Stream>())).ReturnsAsync($"{productFake.Id}-thumb.jpg");

            var useCase = new useCase.UpdateProduct(
                _productRepository.Object,
                _repositoryCategory.Object,
                _notifications.Object,
                _storageService.Object,
                _unitOfWork.Object);

            var outPut = await useCase.Handle(request, CancellationToken.None);


            _productRepository.Verify(r => r.Update(It.IsAny<DomainEntity.Product>(), CancellationToken.None),Times.Once);
            _notifications.Verify(n=>n.AddNotifications(It.IsAny<string>()),Times.Never);
            _unitOfWork.Verify(n=>n.CommitAsync(It.IsAny<CancellationToken>()),Times.Once);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(result.Name, request.Name);
            Assert.Equal(result.Description, request.Description);
            Assert.Equal(result.Price, request.Price);
            Assert.Equal(result.CategoryId, request.CategoryId);

        }



        [Fact(DisplayName = nameof(ShoulReturnErroWhenNotFoundUpdateProduct))]
        [Trait("Application-UseCase", "Update Product")]
        public void ShoulReturnErroWhenNotFoundUpdateProduct()
        {
            var request = ProductInPut();
            var productRepository = ProductModelOutPut();

            //_productRepository.Setup(r => r.GetById(It.IsAny<Guid>())).ThrowsAsync(new NotFoundException(""));

            _productRepository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((DomainEntity.Product?)null);

            var useCase = new useCase.UpdateProduct(
                _productRepository.Object,
                _repositoryCategory.Object,
                _notifications.Object,
                _storageService.Object,
                _unitOfWork.Object);

            var outPut = async () => await useCase.Handle(request, CancellationToken.None);

            //var exception = Assert.ThrowsAsync<NotFoundException>(outPut);

            _productRepository.Verify(r => r.Update(It.IsAny<DomainEntity.Product>(), CancellationToken.None), Times.Never);
            _notifications.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            _unitOfWork.Verify(n => n.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }



        [Theory(DisplayName = nameof(ShoulReturnErroWhenUpdateProduct))]
        [Trait("Application-UseCase", "Update Product")]
        [MemberData(nameof(GetUpdateProductInPutInvalid))]
        public async void ShoulReturnErroWhenUpdateProduct(UpdateProductInPut request)
        {
            var categoryFake = FakerCategory();

            _repositoryCategory.Setup(c => c.GetById(It.IsAny<Guid>())).ReturnsAsync(categoryFake);

            var productRepository = ProductModelOutPut();

            var useCase = new useCase.UpdateProduct(
                _productRepository.Object,
                _repositoryCategory.Object,
                _notifications.Object,
                _storageService.Object,
                _unitOfWork.Object);

            //var outPut = async () => await useCase.Handle(request, CancellationToken.None);
            //var exception = Assert.ThrowsAsync<NotFoundException>(outPut);

            var outPut = await useCase.Handle(request, CancellationToken.None);

            _productRepository.Verify(r => r.Update(It.IsAny<DomainEntity.Product>(), CancellationToken.None), Times.Never);
            _notifications.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Once);
            _unitOfWork.Verify(n => n.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

        }

    }
}
