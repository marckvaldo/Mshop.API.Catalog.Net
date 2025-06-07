using Moq;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;
using DomainEntity = Mshop.Domain.Entity;
using useCase = Mshop.Application.UseCases.Category.GetCategory;

namespace Mshop.Application.UseCases.Category.GetCategory
{
    public class GetCategoryTest : CategoryBaseFixtureTest
    {
        [Fact(DisplayName = nameof(GetCategory))]
        [Trait("Application-UseCase", "Get Category")]
        public async void GetCategory()
        {
            var repository = new Mock<ICategoryRepository>();
            var notification = new Mock<INotification>();

            var dadosResult = FakerCategory();

            repository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync(dadosResult);

            var useCase = new useCase.GetCategory(notification.Object,repository.Object);
            var outPut =  await useCase.Handle(new useCase.GetCategoryInPut(dadosResult.Id), CancellationToken.None);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(result.Name, dadosResult.Name);
            Assert.Equal(result.IsActive, dadosResult.IsActive);    

        }


        [Fact(DisplayName = nameof(SholdReturnErrorWhenCantGetCategory))]
        [Trait("Application-UseCase", "Get Category")]
        public void SholdReturnErrorWhenCantGetCategory()
        {
            var repository = new Mock<ICategoryRepository>();
            var notification = new Mock<INotification>();


            //repository.Setup(r => r.GetById(It.IsAny<Guid>())).ThrowsAsync(new NotFoundException(""));
            repository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((DomainEntity.Category?)null);

            var useCase = new useCase.GetCategory(notification.Object, repository.Object);
            //var outPut = async () => await useCase.BuildCache(new useCase.GetCategoryInPut(Guid.NewGuid()), CancellationToken.None);

            var outPut = useCase.Handle(new useCase.GetCategoryInPut(Guid.NewGuid()), CancellationToken.None);

            //var exception = Assert.ThrowsAsync<NotFoundException>(outPut);

            repository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            notification.Verify(r => r.AddNotifications(It.IsAny<string>()), Times.Once);
        }
    }
}
