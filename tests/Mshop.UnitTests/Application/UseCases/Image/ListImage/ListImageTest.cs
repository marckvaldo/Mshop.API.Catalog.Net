using Moq;
using Mshop.Application.UseCases.Image.Common;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;
using System.Linq.Expressions;
using useCase = Mshop.Application.UseCases.Images.ListImage;
using DomainEntity = Mshop.Domain.Entity;
using Mshop.Core.Exception;

namespace Mshop.Application.UseCases.Image.ListImage
{
    public class ListImageTest : ImageBaseFixtureTest
    {
        [Fact(DisplayName = nameof(ListImage))]
        [Trait("Application-UseCase", "List Image")]
        public async void ListImage()
        {
            var repository = new Mock<IImageRepository>();
            var notification = new Mock<INotification>();

            var productId = Guid.NewGuid();
            var imagens = FakerImages(productId, 3);

            var request = FakerImage(productId);
            repository.Setup(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>())).ReturnsAsync(imagens);

            var useCase = new useCase.ListImage(notification.Object, repository.Object);
            var outPut = await useCase.Handle( new useCase.ListImageInPut(productId), CancellationToken.None);

            var result = outPut.Data;

            repository.Verify(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>()), Times.Once);
            Assert.NotNull(result);

            foreach (var item in result.Images) 
            {
                var image = imagens.Where(x=>x.FileName == item.Image).FirstOrDefault();   

                Assert.NotNull(image);
                Assert.Equal(image?.FileName, item.Image);
            }
        }



        [Fact(DisplayName = nameof(ShoudReturNullWhenListImage))]
        [Trait("Application-UseCase", "List Image")]
        public void ShoudReturNullWhenListImage()
        {
            var notification = new Mock<INotification>();
            var repository = new Mock<IImageRepository>();

            //repository.Setup(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>())).ThrowsAsync(new NotFoundException(""));
            repository.Setup(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>())).ReturnsAsync((List<DomainEntity.Image>?)null);

            var useCase = new useCase.ListImage(notification.Object, repository.Object);
            var outPut = useCase.Handle(new useCase.ListImageInPut(Guid.NewGuid()), CancellationToken.None);

            //var outPut = async () => await useCase.Handle(new useCase.ListImageInPut(Guid.NewGuid()), CancellationToken.None);
            //var exception = Assert.ThrowsAsync<NotFoundException>(outPut);

            notification.Verify(r=>r.AddNotifications(It.IsAny<string>()),Times.Never);
            repository.Verify(r=>r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>()),Times.Once);

        }
    }
}
