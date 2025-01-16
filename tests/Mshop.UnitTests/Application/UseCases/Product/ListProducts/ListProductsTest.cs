using Moq;
using Mshop.Core.Message;
using Mshop.Core.Paginated;
using Mshop.Infra.Data.Interface;
using DomainEntity = Mshop.Domain.Entity;
using useCase = Mshop.Application.UseCases.Product.ListProducts;

namespace Mshop.Application.UseCases.Product.ListProducts
{
    public class ListProductsTest : ListProductTestFixture
    {
        [Fact(DisplayName = nameof(ListProducts))]
        [Trait("Application-UseCase", "List Products")]
        public async void ListProducts()
        {
            var repository = new Mock<IProductRepository>();
            var notification = new Mock<INotification>();

            var category = FakerCategory();
            var productsFake = FakerProducts(10, category);

            var useCase = new useCase.ListProducts(repository.Object, notification.Object);

            var random = new Random();

            var request = new ListProductInPut(
                            page: random.Next(1,10),
                            perPage: random.Next(10,20),
                            search: faker.Commerce.ProductName(),
                            sort:"name",
                            dir:Core.Enum.Paginated.SearchOrder.Asc
                            );

            var outPutRepository = new PaginatedOutPut<DomainEntity.Product>(
                                    currentPage: request.Page,
                                    perPage: request.PerPage,
                                    itens: productsFake,
                                    total: 10
                                    );

            repository.Setup(r => r.FilterPaginated(
                It.Is<PaginatedInPut>(
                    SearchInput => SearchInput.Page == request.Page
                    && SearchInput.PerPage == request.PerPage
                    && SearchInput.Search == request.Search
                    && SearchInput.OrderBy == request.Sort
                    && SearchInput.Order == request.Dir
                    ),
                It.IsAny<CancellationToken>()
                )).ReturnsAsync(outPutRepository);


            var outPut = await useCase.Handle(request, CancellationToken.None);

            var result = outPut.Data;

            Assert.NotNull(result);
            Assert.Equal(productsFake.Count, result.Total);
            Assert.Equal(request.Page, result.Page);
            Assert.Equal(request.PerPage, result.PerPage);
            Assert.NotNull(result.Itens);
            Assert.True(result.Itens.Any());
        }

    }
}
