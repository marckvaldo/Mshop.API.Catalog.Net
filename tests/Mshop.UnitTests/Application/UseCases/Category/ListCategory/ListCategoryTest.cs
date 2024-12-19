using Moq;
using useCase = Mshop.Application.UseCases.Category.ListCategorys;
using DomainEntity = Mshop.Domain.Entity;
using Mshop.Infra.Data.Interface;
using Mshop.Core.Message;
using Mshop.Core.Paginated;
using Mshop.Application.UseCases.Category.ListCategorys;
using Mshop.Application.UseCases.Category.Common;

namespace Mshop.Application.UseCases.Category.ListCategory
{
    public class ListCategoryTest : CategoryBaseFixtureTest
    {
        [Fact(DisplayName = nameof(ListCategory))]
        [Trait("Application-UseCase", "List Categogry")]

        public async void ListCategory()
        {
            var repository = new Mock<ICategoryRepository>();
            var notification = new Mock<INotification>();

            var categorys = FakerCategories(10);

            var result = new PaginatedOutPut<DomainEntity.Category>(1, 10, 10, categorys);

            var request = new ListCategoryInPut(1, 10, "", "Name", Core.Enum.Paginated.SearchOrder.Desc);

            repository.Setup(r => r.FilterPaginated(It.IsAny<PaginatedInPut>())).ReturnsAsync(result);

            var useCase = new useCase.ListCategory(repository.Object, notification.Object);
            var outPut = await useCase.Handle(request, CancellationToken.None);

            var resultOutPu = outPut.Data;

            Assert.NotNull(resultOutPu);
            Assert.NotNull(resultOutPu.Itens);
            Assert.Equal(resultOutPu.PerPage, request.PerPage);
            Assert.Equal(10, resultOutPu.Total);
            Assert.Equal(resultOutPu.Page, request.Page);
            Assert.Equal(10, resultOutPu.Itens.Count);


        }
    }
}
