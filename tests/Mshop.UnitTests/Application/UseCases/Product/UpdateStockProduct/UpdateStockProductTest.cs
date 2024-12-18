using Moq;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;
using DomainEntity = Mshop.Domain.Entity;
using useCase = Mshop.Application.UseCases.Product.UpdateStockProduct;

namespace Mshop.Application.UseCases.Product.UpdateStockProduct
{
    public class UpdateStockProductTest :  UpdateStockProductTestFixture
    {
        private readonly Mock<IProductRepository> _productRepository;
        private readonly Mock<ICategoryRepository> _categoryRepository;
        private readonly Mock<INotification> _notifications;
        private readonly Mock<IUnitOfWork> _unitOfWork;

        public UpdateStockProductTest()
        {
            _productRepository = new Mock<IProductRepository>();
            _categoryRepository = new Mock<ICategoryRepository>();
            _notifications = new Mock<INotification>();
            _unitOfWork = new Mock<IUnitOfWork>();
        }

        [Fact(DisplayName = nameof(UpdateStockProduct))]
        [Trait("Application-UseCase", "Update Stock Product")]
        public async void UpdateStockProduct()
        {
            var request = UpdateStockProductInPut();

            _productRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .ReturnsAsync(Faker());
            
            var useCase = new useCase.UpdateStockProducts(
                _productRepository.Object,
                _notifications.Object,
                _unitOfWork.Object,
                _categoryRepository.Object);

            var outPut = await useCase.Handle(request, CancellationToken.None);

            _productRepository.Verify(r => r.Update(It.IsAny<DomainEntity.Product>(), CancellationToken.None), Times.Once);
            _notifications.Verify(n=>n.AddNotifications(It.IsAny<string>()),Times.Never);
            _unitOfWork.Verify(n=>n.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(request.Stock,result.Stock);
            
        }


        [Fact(DisplayName = nameof(SholdReturnErrorCantUpdateStockProduct))]
        [Trait("Application-UseCase", "Update Stock Product")]
        public void SholdReturnErrorCantUpdateStockProduct()
        {
            var request = UpdateStockProductInPut();
            request.Stock = request.Stock * -1;  

            _notifications.Setup(n=>n.HasErrors()).Returns(true);

            var useCase = new useCase.UpdateStockProducts(
                _productRepository.Object,
                _notifications.Object,
                _unitOfWork.Object,
                _categoryRepository.Object);

            //var outPut = async () => await useCase.Handle(request, CancellationToken.None);
            //var excption = Assert.ThrowsAsync<NotFoundException>(outPut);
            
            var outPut = useCase.Handle(request, CancellationToken.None);

            _productRepository.Verify(r => r.Update(It.IsAny<DomainEntity.Product>(), CancellationToken.None), Times.Never);
            _notifications.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Once);
            _unitOfWork.Verify(n => n.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

        }

    }
}
