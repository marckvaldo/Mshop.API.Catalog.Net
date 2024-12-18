using Moq;
using Mshop.Infra.Data.Interface;
using useCase = Mshop.Application.UseCases.Images.CreateImage;
using DomainEntity = Mshop.Domain.Entity;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Core.Data;

namespace Mshop.Application.UseCases.Image.CreateImage
{
    public class CreateImageTest : CreateImageTestFixture
    {
        [Fact(DisplayName = nameof(CreateImage))]
        [Trait("Application-UseCase", "Create Image")]

        public async void CreateImage()
        {
            var repository = new Mock<IImageRepository>();
            var repositoryProduct = new Mock<IProductRepository>();
            var notification = new Mock<INotification>();
            var storageService = new Mock<IStorageService>();
            var unitOfWork = new Mock<IUnitOfWork>();

            repositoryProduct.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(FakerProduct(FakerCategory()));

            var id = Guid.NewGuid();
            var images = ImageFakers64(3);
            var request = FakerRequest(id, images);

            var useCase = new useCase.CreateImage(
                repository.Object,
                storageService.Object, 
                repositoryProduct.Object, 
                notification.Object,
                unitOfWork.Object);

            var outPut = await useCase.Handle(request, CancellationToken.None);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(result.ProductId, id);
            Assert.NotNull(result.Images);
            unitOfWork.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            notification.Verify(r=>r.AddNotifications(It.IsAny<string>()), Times.AtMost(8));
            storageService.Verify(r=>r.Upload(It.IsAny<string>(), It.IsAny<Stream>()), Times.AtMost(8));
            repositoryProduct.Verify(r=>r.GetById(It.IsAny<Guid>()), Times.Once);
            repository.Verify(r=>r.CreateRange(It.IsAny<List<DomainEntity.Image>>(), It.IsAny<CancellationToken>()), Times.Once);

        }


        [Fact(DisplayName = nameof(ShouldReturnErrorCreateImageWhenNotHaveImage))]
        [Trait("Application-UseCase", "Create Image")]

        public void ShouldReturnErrorCreateImageWhenNotHaveImage()
        {
            var repository = new Mock<IImageRepository>();
            var notification = new Mock<INotification>();
            var storageService = new Mock<IStorageService>();
            var repositoryProduct = new Mock<IProductRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var id = Guid.NewGuid();
            var images = ImageFakers64(3);
            var request = FakerRequest(id, images);

            request.Images = null;

            var useCase = new useCase.CreateImage(
                repository.Object, 
                storageService.Object, 
                repositoryProduct.Object, 
                notification.Object, 
                unitOfWork.Object);

            var action = async () => await useCase.Handle(request, CancellationToken.None);

            var exception = Assert.ThrowsAnyAsync<ApplicationException>(action);

            repository.Verify(r => r.CreateRange(It.IsAny<List<DomainEntity.Image>>(), CancellationToken.None), Times.Never);
            notification.Verify(n=>n.AddNotifications(It.IsAny<string>()), Times.Once);
            storageService.Verify(r => r.Upload(It.IsAny<string>(), It.IsAny<Stream>()), Times.Never);
            unitOfWork.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

    }
}
