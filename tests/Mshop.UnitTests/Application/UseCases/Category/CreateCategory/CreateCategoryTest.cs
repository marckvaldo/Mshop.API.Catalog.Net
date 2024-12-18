using Moq;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Infra.Data.Interface;
using Mshop.UnitTests.Application.UseCases.Category.CreateCategory;
using useCase = Mshop.Application.UseCases.Category.CreateCategory;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.Application.UseCases.Category.CreateCategory
{
    public class CreateCategoryTest: CreateCategoryTestFixture
{
        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("Application-UseCase","Create Category")]

        public async void CreateCategory()
        {
            var repository = new Mock<ICategoryRepository>();
            var notification = new Mock<INotification>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var request = FakerRequest();

            var useCase = new useCase.CreateCategory(notification.Object, repository.Object,unitOfWork.Object);
            var outPut = await useCase.Handle(request, CancellationToken.None);

            repository.Verify(r => r.Create(It.IsAny<DomainEntity.Category>(), CancellationToken.None), Times.Once);
            notification.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            unitOfWork.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.NotNull(outPut.Data);
            Assert.Equal(outPut?.Data?.Name, request.Name);
            Assert.Equal(outPut?.Data?.IsActive, request.IsActive);
            Assert.NotNull(outPut?.Data?.Id);
        }


        [Theory(DisplayName = nameof(CreateCategoryInvalid))]
        [Trait("Application-UseCase", "Create Category")]
        [MemberData(nameof(ListNamesCategoryInvalid))]

        public void CreateCategoryInvalid(string name)
        {
            var repository = new Mock<ICategoryRepository>();       
            var notification = new Mock<INotification>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var request = FakerRequest(name,true);

            notification.Setup(r=>r.HasErrors()).Returns(true);

            var useCase = new useCase.CreateCategory(notification.Object, repository.Object, unitOfWork.Object);
            //var action = async () => await useCase.Handle(request, CancellationToken.None);
            var outPut = useCase.Handle(request, CancellationToken.None);

            // var exception = Assert.ThrowsAsync<EntityValidationException>(action);

            repository.Verify(n => n.Create(It.IsAny<DomainEntity.Category>(), CancellationToken.None), Times.Never);
            unitOfWork.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            notification.Verify(r=>r.AddNotifications(It.IsAny<string>()),Times.AtMost(3));
        }

    }
}
