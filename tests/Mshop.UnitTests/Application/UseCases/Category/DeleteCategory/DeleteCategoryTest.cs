using Moq;
using Mshop.Core.Data;
using CoreMessage = Mshop.Core.Message;
using Mshop.Infra.Data.Interface;
using DomainEntity = Mshop.Domain.Entity;
using useCase = Mshop.Application.UseCases.Category.DeleteCategory;
using Mshop.Application.UseCases.Category.Common;

namespace Mshop.Application.UseCases.Category.DeleteCategory
{
    public class DeleteCategoryTest : CategoryBaseFixtureTest
    {
        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("Application-UseCase", "Delete Category")]

        public async void DeleteCategory()
        {
            var repository = new Mock<ICategoryRepository>();
            var notification = new Mock<CoreMessage.INotification>();
            var repositoryProduct = new Mock<IProductRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var category = FakerCategory();

            //repositoryProduct.Setup(r => r.GetProductsByCategoryId(It.IsAny<Guid>())).ReturnsAsync(FakerProducts(6,FakerCategory()));
            repository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync(category);

            var useCase = new useCase.DeleteCategory(
                repository.Object, 
                repositoryProduct.Object, 
                notification.Object,
                unitOfWork.Object);

            var outPut = await useCase.Handle(new useCase.DeleteCategoryInPut(category.Id), CancellationToken.None);

            repository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            notification.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            unitOfWork.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(result.Name, category.Name);
            Assert.Equal(result.IsActive, category.IsActive);
            Assert.NotNull(result?.Id);
        }



        [Fact(DisplayName = nameof(ShouldReturnErroWhenDeleteCategoryNotFound))]
        [Trait("Application-UseCase", "Delete Category")]

        public void ShouldReturnErroWhenDeleteCategoryNotFound()
        {
            var repository = new Mock<ICategoryRepository>();
            var notification = new Mock<CoreMessage.INotification>();
            var repositoryProduct = new Mock<IProductRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var category = FakerCategory();

            var useCase = new useCase.DeleteCategory(
                repository.Object, 
                repositoryProduct.Object, 
                notification.Object,
                unitOfWork.Object);

            //var action = async () => await useCase.BuildCache(new useCase.DeleteCategoryInPut(category.Id), CancellationToken.None);
            //var exception = Assert.ThrowsAsync<ApplicationValidationException>(action);

            var outPut = useCase.Handle(new useCase.DeleteCategoryInPut(category.Id), CancellationToken.None);

            repository.Verify(n => n.DeleteById(It.IsAny<DomainEntity.Category>(), CancellationToken.None), Times.Never);          
            notification.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Once);
            repositoryProduct.Verify(n => n.GetProductsByCategoryId(It.IsAny<Guid>()), Times.Never);
            unitOfWork.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

        }



        [Fact(DisplayName = nameof(ShouldReturnErroWhenDeleteCategoryWhenThereAreSameProducts))]
        [Trait("Application-UseCase", "Delete Category")]

        public void ShouldReturnErroWhenDeleteCategoryWhenThereAreSameProducts()
        {
            var repository = new Mock<ICategoryRepository>();
            var repositoryProduct = new Mock<IProductRepository>();
            var notification = new Mock<CoreMessage.INotification>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var category = FakerCategory();
            var product = FakerProducts(3, category);

            repository.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync(category);
            repositoryProduct.Setup(r => r.GetProductsByCategoryId(It.IsAny<Guid>())).ReturnsAsync(product);
            

            var useCase = new useCase.DeleteCategory(
                repository.Object, 
                repositoryProduct.Object, 
                notification.Object, 
                unitOfWork.Object);

            //var action = async () => await useCase.BuildCache(new useCase.DeleteCategoryInPut(category.Id), CancellationToken.None);
            //var exception = Assert.ThrowsAsync<ApplicationValidationException>(action);

            var outPut = useCase.Handle(new useCase.DeleteCategoryInPut(category.Id), CancellationToken.None);

            repository.Verify(n => n.GetById(It.IsAny<Guid>()), Times.Once);
            repository.Verify(n => n.DeleteById(It.IsAny<DomainEntity.Category>(), CancellationToken.None), Times.Never);
            notification.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Once);
            unitOfWork.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}
