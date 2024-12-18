using Moq;
using Mshop.Core.Data;
using Mshop.Core.Exception;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Interface;
using DomainEntity = Mshop.Domain.Entity;
using useCase = Mshop.Application.UseCases.Product.DeleteProduct;


namespace Mshop.Application.UseCases.Product.DeleteProduct
{
    public class DeleteProductTest : DeleteProductTestFixture
    {
        [Fact(DisplayName = nameof(DeleteProduct))]
        [Trait("Application-UseCase", "Delete Products")]

        public async void DeleteProduct()
        {
            var repository = new Mock<IProductRepository>();
            var notification = new Mock<INotification>();
            var repositoryImage = new Mock<IImageRepository>();
            var storageService = new Mock<IStorageService>();
            var unitOfWork = new Mock<IUnitOfWork>();   

            repository.Setup(repository => repository.GetById(It.IsAny<Guid>())).ReturnsAsync(Faker());

            var guid = Faker().Id;

            var product = new useCase.DeleteProduct(
                repository.Object, 
                repositoryImage.Object, 
                notification.Object, 
                storageService.Object,
                unitOfWork.Object);

            var outPut = await product.Handle(new useCase.DeleteProductInPut(guid), CancellationToken.None);

            repository.Verify(r => r.DeleteById(It.IsAny<DomainEntity.Product>(), CancellationToken.None),Times.Once);
            repository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            notification.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            unitOfWork.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

            var result = outPut.Data;

            Assert.Equal(result.Id, guid);
            Assert.NotNull(result);            

        }

        [Fact(DisplayName = nameof(SholdReturnErrorWhenCantDeleteProduct))]
        [Trait("Application-UseCase", "Delete Products")]
        public void SholdReturnErrorWhenCantDeleteProduct()
        {
            var repository = new Mock<IProductRepository>();
            var notification = new Mock<INotification>();
            var repositoryImage = new Mock<IImageRepository>();
            var storageService = new Mock<IStorageService>();
            var unitOfWork = new Mock<IUnitOfWork>();

            //repository.Setup(r => r.GetById(It.IsAny<Guid>())).ThrowsAsync(new NotFoundException("your search returned null"));
            repository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((DomainEntity.Product?)null);

            var product = new useCase.DeleteProduct(
                repository.Object, 
                repositoryImage.Object, 
                notification.Object, 
                storageService.Object,
                unitOfWork.Object);

            var guid = Faker().Id;

            //var action = async () => await product.Handle(new useCase.DeleteProductInPut(guid), CancellationToken.None);
            //var exception = Assert.ThrowsAsync<NotFoundException>(action);

            var ouPut = product.Handle(new useCase.DeleteProductInPut(guid), CancellationToken.None);

            repository.Verify(r => r.DeleteById(It.IsAny<DomainEntity.Product>(), CancellationToken.None), Times.Never);
            repository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            notification.Verify(a => a.AddNotifications(It.IsAny<string>()), Times.Once);
            repositoryImage.Verify(a => a.DeleteByIdProduct(It.IsAny<Guid>()), Times.Never);
            unitOfWork.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
