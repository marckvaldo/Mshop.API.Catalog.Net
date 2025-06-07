using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.UseCases.Cache.Category.ListCategoriesCache;
using Mshop.Application.UseCases.Category.ListCategories;
using Mshop.Core.Data;
using Mshop.Core.Message;
using Mshop.Core.Paginated;
using Mshop.Domain.Contract.Services;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ApplicationUseCase = Mshop.Application.UseCases.Cache.Category.ListCategoriesCache;

namespace Mshop.IntegrationTests.Application.UserCases.Cache.Category.ListCategories
{
    [Collection("List Category Collection")]
    [CollectionDefinition("List Category Collection", DisableParallelization = true)]
    public class ListCategoryTest : IntegracaoBaseFixture, IDisposable
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryCacheRepository _categoryCacheRepository;
        private readonly IServiceCacheCategory _buildCacheCategory;
        private readonly INotification _notification;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;
        private readonly StackExchange.Redis.IDatabase _database;
        private readonly StartIndex _startIndex;

        public ListCategoryTest() : base()
        {
            var connection = _serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
            _database = connection.GetDatabase();

            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _categoryCacheRepository = _serviceProvider.GetRequiredService<ICategoryCacheRepository>();
            _buildCacheCategory = _serviceProvider.GetRequiredService<IServiceCacheCategory>();
            _notification = _serviceProvider.GetRequiredService<INotification>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();

            DeleteCache(_database).Wait();
            _startIndex.CreateIndex().Wait();
        }


        [Fact(DisplayName = nameof(ListCategoriesCache))]
        [Trait("Integration - Application.UseCase.Cache", "Category Use Case")]

        public async Task ListCategoriesCache()
        {

            var categoryFake = FakerCategories(20);
            foreach (var item in categoryFake)
            {
                await _categoryRepository.Create(item, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();

            var useCase = new ListCategoriesCache(_categoryCacheRepository, _categoryRepository,_buildCacheCategory, _notification);
            var request = new ListCategoriesCacheInPut(
                            page: 1,
                            perPage: 5,
                            search: "",
                            sort: "name",
                            dir: Mshop.Core.Enum.Paginated.SearchOrder.Asc
                            );

            var outPut = await useCase.Handle(request, CancellationToken.None);

            var paginate = new PaginatedInPut(currentPage:1, perPage: 20, search:"", orderBy:"",order: Core.Enum.Paginated.SearchOrder.Desc);
            Thread.Sleep(2000);
            var categoryCache = await _categoryCacheRepository.FilterPaginated(paginate,CancellationToken.None);

            var result = outPut.Data;

            Assert.False(_notification.HasErrors());
            Assert.NotNull(result);
            Assert.Equal(categoryFake.Count, result.Total);
            Assert.Equal(request.CurrentPage, result.CurrentPage);
            Assert.Equal(request.PerPage, result.PerPage);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Any());

            Assert.NotNull(categoryCache);
            Assert.Equal(20,categoryCache.Total);

        }

        public void Dispose()
        {
            //CleanInMemoryDatabase();
        }

    }
}
