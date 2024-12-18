using Moq;
using useCase = Mshop.Application.UseCases.Category.UpdateCategory;
using DomainEntity = Mshop.Domain.Entity;
using Mshop.Core.Message;
using Mshop.Core.Data;
using Mshop.Infra.Data.Interface;
using Mshop.Core.Exception;

namespace Mshop.UnitTests.Application.UseCases.Category.UpdateCategory
{
    public class UpdateCategoryTest : UpdateCategoryTestFituxre
    {
        private readonly Mock<INotification> _notifications;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ICategoryRepository> _repositoryCategory;

        public UpdateCategoryTest()
        {
            _notifications = new Mock<INotification>();
            _unitOfWork = new Mock<IUnitOfWork>();           
            _repositoryCategory = new Mock<ICategoryRepository>();
        }

        [Fact(DisplayName = nameof(UpdateCategory))]
        [Trait("Application-UseCase", "Update Category")]
        public async void UpdateCategory()
        {  
            var request = FakerRequest();
            var productFake = FakerCategory();

            _repositoryCategory.Setup(r => r.GetById(It.IsAny<Guid>()))
                .ReturnsAsync(productFake);

            var useCase = new useCase.UpdateCategory(
                _repositoryCategory.Object,
                _notifications.Object,
                _unitOfWork.Object);

            var outPut = await useCase.Handle(request, CancellationToken.None);


            _repositoryCategory.Verify(r => r.Update(It.IsAny<DomainEntity.Category>(), CancellationToken.None), Times.Once);
            _notifications.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()),Times.Once);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(result.Name, request.Name);
            Assert.Equal(result.IsActive, request.IsActive);

        }

        [Fact(DisplayName = nameof(SholdReturnErrorWhenUpdateCategory))]
        [Trait("Application-UseCase", "Update Category")]
        public void SholdReturnErrorWhenUpdateCategory()
        {
            var request = FakerRequest();
            var productFake = FakerCategory();

            //_repositoryCategory.Setup(r=>r.GetById(It.IsAny<Guid>())).ThrowsAsync(new NotFoundException("")); 
            _repositoryCategory.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((DomainEntity.Category?)null);

            var useCase = new useCase.UpdateCategory(
                _repositoryCategory.Object,
                _notifications.Object,
                _unitOfWork.Object);

            //var action = async () => await useCase.Handle(request, CancellationToken.None);
            //var exception = Assert.ThrowsAsync<NotFoundException>(action);

            var outPut = useCase.Handle(request, CancellationToken.None);

            _repositoryCategory.Verify(r => r.Update(It.IsAny<DomainEntity.Category>(), CancellationToken.None), Times.Never);
            _notifications.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Once);
            _unitOfWork.Verify(u=>u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}
