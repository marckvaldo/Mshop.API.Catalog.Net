using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.UseCases.Category.ListCategories;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ApplicationUseCase = Mshop.Application.UseCases.Category.ListCategories;

namespace Mshop.IntegrationTests.Application.UserCases.Category.ListCategory
{
    [Collection("List Category Collection")]
    [CollectionDefinition("List Category Collection", DisableParallelization = true)]
    public class ListCategoryTest : IntegracaoBaseFixture, IDisposable
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotification _notification;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;

        public ListCategoryTest()
        {
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _notification = _serviceProvider.GetRequiredService<INotification>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();
        }


        [Fact(DisplayName = nameof(ListCategories))]
        [Trait("Integration - Application.UseCase", "Category Use Case")]

        public async Task ListCategories()
        {

            var categoryFake = FakerCategories(20);
            foreach (var item in categoryFake)
            {
                await _categoryRepository.Create(item, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();

            var useCase = new ApplicationUseCase.ListCategory(_categoryRepository, _notification);
            var request = new ListCategoryInPut(
                            page: 1,
                            perPage: 5,
                            search: "",
                            sort: "name",
                            dir: Mshop.Core.Enum.Paginated.SearchOrder.Asc
                            );

            var outPut = await useCase.Handle(request, CancellationToken.None);

            var result = outPut.Data;

            Assert.False(_notification.HasErrors());
            Assert.NotNull(result);
            Assert.Equal(categoryFake.Count, result.Total);
            Assert.Equal(request.Page, result.Page);
            Assert.Equal(request.PerPage, result.PerPage);
            Assert.NotNull(result.Itens);
            Assert.True(result.Itens.Any());

        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }

    }
}
