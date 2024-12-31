using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Data;
using Mshop.Core.Enum.Paginated;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTests.Common;
using ServiceBuilder = Mshop.Application.Service;

namespace Mshop.IntegrationTests.Application.Service
{
    public class BuildCacheCategoryTest : IntegracaoBaseFixture
    {
        private ICategoryCacheRepository _categoryCacheRepository;
        private ICategoryRepository _categoryRepository;
        private IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;
        private readonly StartIndex _startIndex;
        private readonly StackExchange.Redis.IDatabase _database;

        public BuildCacheCategoryTest() : base()
        {
            var connection = _serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
            _database = connection.GetDatabase();

            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _categoryCacheRepository = _serviceProvider.GetRequiredService<ICategoryCacheRepository>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();

            DeleteDataBase(_DbContext, false).Wait();
            AddMigartion(_DbContext).Wait();

            DeleteCache(_database).Wait();
            _startIndex.CreateIndex().Wait();

        }

        [Fact(DisplayName = nameof(ShouldBuilderCacheCategory))]
        [Trait("Integration - Application.Service", "Builder Category Cache")]

        public async Task ShouldBuilderCacheCategory()
        {
            var listCategory = FakerCategories(50);

            foreach (var category in listCategory)
            {
                await _categoryRepository.Create(category, CancellationToken.None);
            }

            await _unitOfWork.CommitAsync();

            var service = new ServiceBuilder.BuildCacheCategory(_serviceProvider);
            await service.Handle();

            var categories = await _categoryCacheRepository.FilterPaginated(
                                        new Core.Paginated.PaginatedInPut(
                                            page: 1,
                                            perPage: 50,
                                            search: "",
                                            orderBy: "",
                                            order: SearchOrder.Desc),
                                        CancellationToken.None);

            Assert.NotNull(categories.Itens);
            Assert.Equal(50, categories.Itens.Count);
        }



    }
}
