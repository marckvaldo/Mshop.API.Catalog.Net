using Moq;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;
using Mshop.UnitTests.Application.UseCases.Product.GetProduct;
using System.Linq.Expressions;
using DomainEntity = Mshop.Domain.Entity;
using useCase = Mshop.Application.UseCases.Product.GetProduct;

namespace Mshop.Application.UseCases.Product.GetProduts
{
    public class GetProdutctTest : GetProductTestFixture
    {
        [Fact(DisplayName = nameof(GetProduct))]
        [Trait("Application-UseCase", "Get Products")]
        public async void GetProduct()
        {
            var repository = new Mock<IProductRepository>();
            var notification = new Mock<INotification>();
            var repositoryImage = new Mock<IImageRepository>();

            
            var productFake = FakerProduct(FakerCategory());
            var guid = productFake.Id;
            var imagesFaker = FakerImages(guid,3);

            repository.Setup(r => r.GetProductWithCategory(It.IsAny<Guid>())).ReturnsAsync(productFake);
            repositoryImage.Setup(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>())).ReturnsAsync(imagesFaker);

            var useCase = new useCase.GetProduct(repository.Object, repositoryImage.Object, notification.Object) ;
            var outPut = await useCase.Handle(new useCase.GetProductInPut(guid), CancellationToken.None);

            repository.Verify(r => r.GetProductWithCategory(It.IsAny<Guid>()), Times.Once);
            notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Never);
            repositoryImage.Verify(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>()),Times.Once);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(result.Name, productFake.Name);
            Assert.Equal(result.Description, productFake.Description);
            Assert.Equal(result.Price, productFake.Price);
            Assert.Equal(result.Thumb, productFake.Thumb?.Path);
            Assert.Equal(result.CategoryId, productFake.CategoryId);
            Assert.Equal(result.Stock, productFake.Stock);
            Assert.Equal(result.IsActive, productFake.IsActive);
            Assert.NotNull(result.Images);
            
            foreach(var item in result.Images)
            {
                var image = imagesFaker.Where(i => i.FileName == item).FirstOrDefault();
                Assert.NotNull(image);
                Assert.Equal(image?.FileName,item);
            }

        }


        [Fact(DisplayName = nameof(GetProductWithOutImages))]
        [Trait("Application-UseCase", "Get Products")]
        public async void GetProductWithOutImages()
        {
            var repository = new Mock<IProductRepository>();
            var notification = new Mock<INotification>();
            var repositoryImage = new Mock<IImageRepository>();

            var productFake = FakerProduct(FakerCategory());
            var guid = productFake.Id;
            var imagesFaker = FakerImages(guid, 3);

            repository.Setup(r => r.GetProductWithCategory(It.IsAny<Guid>())).ReturnsAsync(productFake);
            repositoryImage.Setup(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>())).ReturnsAsync(new List<DomainEntity.Image>());

            var useCase = new useCase.GetProduct(repository.Object, repositoryImage.Object, notification.Object);
            var outPut = await useCase.Handle(new useCase.GetProductInPut(guid), CancellationToken.None);

            repository.Verify(r => r.GetProductWithCategory(It.IsAny<Guid>()), Times.Once);
            notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Never);
            repositoryImage.Verify(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>()), Times.Once);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(result.Name, productFake.Name);
            Assert.Equal(result.Description, productFake.Description);
            Assert.Equal(result.Price, productFake.Price);
            Assert.Equal(result.Thumb, productFake.Thumb?.Path);
            Assert.Equal(result.CategoryId, productFake.CategoryId);
            Assert.Equal(result.Stock, productFake.Stock);
            Assert.Equal(result.IsActive, productFake.IsActive);
            Assert.True(result.Images.Count == 0);
        }


        [Fact(DisplayName = nameof(SholdReturnErrorWhenCantGetProduct))]
        [Trait("Application-UseCase", "Get Products")]
        public void SholdReturnErrorWhenCantGetProduct()
        {
            var repository = new Mock<IProductRepository>();
            var notification = new Mock<INotification>();
            var repositoryImage = new Mock<IImageRepository>();

            repository.Setup(r => r.GetProductWithCategory(It.IsAny<Guid>()));//.ThrowsAsync(new NotFoundException(""));

            var caseUse = new useCase.GetProduct(repository.Object, repositoryImage.Object, notification.Object);

            //var outPut = async () => await caseUse.BuildCache(new useCase.GetProductInPut(Guid.NewGuid()), CancellationToken.None);
            //var exception = Assert.ThrowsAsync<ApplicationValidationException>(outPut);

            var outPut = caseUse.Handle(new useCase.GetProductInPut(Guid.NewGuid()), CancellationToken.None);

            repository.Verify(r => r.GetProductWithCategory(It.IsAny<Guid>()), Times.Once);
            notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Once);
            repositoryImage.Verify(r => r.Filter(It.IsAny<Expression<Func<DomainEntity.Image, bool>>>()), Times.Never);
        }
    }
}
