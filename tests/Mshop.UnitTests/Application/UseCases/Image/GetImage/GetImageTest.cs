using Moq;
using Mshop.Application.UseCases.Image.Common;
using Mshop.Core.Message;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Data.Interface;
using useCase = Mshop.Application.UseCases.Images.GetImage;

namespace Mshop.Application.UseCases.Image.GetImage
{
    public class GetImageTest : ImageBaseFixtureTest
    {
        [Fact(DisplayName = nameof(GetImage))]
        [Trait("Application-UseCase", "Get Image")]
        public async void GetImage()
        {
            var repository = new Mock<IImageRepository>();
            var notification = new Mock<INotification>();
            var storageService = new Mock<IStorageService>();

            var request = FakerImage(Guid.NewGuid());
            repository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync(request);
            

            var useCase = new useCase.GetImage(notification.Object, repository.Object, storageService.Object);
            var outPut = await useCase.Handle(new useCase.GetImageInPut(Guid.NewGuid()), CancellationToken.None);

            Assert.NotNull(outPut);
            Assert.Equal(request.FileName, outPut.Data.Image.Image);
            Assert.Equal(request.ProductId, outPut.Data.ProductId);
            repository.Verify(r=>r.GetById(It.IsAny<Guid>()),Times.Once);
            notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Never);
        }



        [Fact(DisplayName = nameof(ShouldReturnErroWhenGetImage))]
        [Trait("Application-UseCase", "Get Image")]
        public void ShouldReturnErroWhenGetImage()
        {
            var repository = new Mock<IImageRepository>();
            var notification = new Mock<INotification>();
            var storageService = new Mock<IStorageService>();

            var request = FakerImage(Guid.NewGuid());

            var useCase = new useCase.GetImage(notification.Object, repository.Object, storageService.Object);
            var outPut = useCase.Handle(new useCase.GetImageInPut(Guid.NewGuid()), CancellationToken.None);


            //var action = async () => await useCase.BuildCache(new useCase.GetImageInPut(Guid.NewGuid()), CancellationToken.None);
            //var exception = Assert.ThrowsAsync<ApplicationValidationException>(action);

            repository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            notification.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Once);            
        }

    }
}
