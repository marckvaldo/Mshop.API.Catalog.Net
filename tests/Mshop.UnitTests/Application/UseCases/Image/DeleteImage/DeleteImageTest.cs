using Moq;
using useCase = Mshop.Application.UseCases.Images.DeleteImage;
using DomainEntity = Mshop.Domain.Entity;
using Mshop.Infra.Data.Interface;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Core.Data;


namespace Mshop.Application.UseCases.Image.DelteImage
{
    public class DeleteImageTest : DeleteImageTestFixute
    {
        [Fact(DisplayName = nameof(DeleteImage))]
        [Trait("Application-UseCase", "Delete Image")]

        public async void DeleteImage()
        {
            var repository = new Mock<IImageRepository>();
            var notification = new Mock<INotification>();
            var storageService = new Mock<IStorageService>();
            var unitOfWork = new Mock<IUnitOfWork>();


            var id = Guid.NewGuid();
            var images = FakerImage(id);
            var request = FakerRequest();

            repository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync(images);
            storageService.Setup(s => s.Delete(It.IsAny<string>())).ReturnsAsync(true);

            var useCase = new useCase.DeleteImage(
                repository.Object, 
                storageService.Object, 
                notification.Object,
                unitOfWork.Object);

            var outPut = await useCase.Handle(new useCase.DeleteImageInPut(request), CancellationToken.None);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(result.ProductId, id);
            Assert.NotNull(result.Image);
            unitOfWork.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            storageService.Verify(r => r.Delete(It.IsAny<string>()), Times.Once);
            notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Never);
            repository.Verify(r=>r.DeleteById(It.IsAny<DomainEntity.Image>(), It.IsAny<CancellationToken>()), Times.Once);
        }



        [Fact(DisplayName = nameof(ShouldReturnErrorWhenDeleteImage))]
        [Trait("Application-UseCase", "Delete Image")]

        public void ShouldReturnErrorWhenDeleteImage()
        {
            var repository = new Mock<IImageRepository>();
            var notification = new Mock<INotification>();
            var storageService = new Mock<IStorageService>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var id = Guid.NewGuid();
            var images = FakerImage(id);
            var request = FakerRequest();
         

            var useCase = new useCase.DeleteImage(
                repository.Object, 
                storageService.Object, 
                notification.Object,
                unitOfWork.Object);

            var action = async () => await useCase.Handle(new useCase.DeleteImageInPut(request), CancellationToken.None);

            var exception = Assert.ThrowsAsync<ApplicationException>(action);

            repository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            notification.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Once);
            storageService.Verify(s => s.Delete(It.IsAny<string>()), Times.Never);
            repository.Verify(r => r.DeleteById(It.IsAny<DomainEntity.Image>(),CancellationToken.None), Times.Never);
            unitOfWork.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);


        }
    }
}
