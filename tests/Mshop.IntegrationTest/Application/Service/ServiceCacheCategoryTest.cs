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
    [Collection("Service Category Cache")]
    [CollectionDefinition("Service Category Cache Collection", DisableParallelization = true)]
    public class ServiceCacheCategoryTest : IntegracaoBaseFixture
    {
        private ICategoryCacheRepository _categoryCacheRepository;
        private ICategoryRepository _categoryRepository;
        private IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;
        private readonly StartIndex _startIndex;
        private readonly StackExchange.Redis.IDatabase _database;

        public ServiceCacheCategoryTest() : base()
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
        [Trait("Integration - Application.Service.Category", "Build Category Cache")]
        public async Task ShouldBuilderCacheCategory()
        {
            var listCategory = FakerCategories(50);

            foreach (var category in listCategory)
            {
                await _categoryRepository.Create(category, CancellationToken.None);
            }

            await _unitOfWork.CommitAsync();

            var service = new ServiceBuilder.ServiceCacheCategory(_serviceProvider);
            await service.BuildCache();

            var categories = await _categoryCacheRepository.FilterPaginated(
                                        new Core.Paginated.PaginatedInPut(
                                            currentPage: 1,
                                            perPage: 50,
                                            search: "",
                                            orderBy: "",
                                            order: SearchOrder.Desc),
                                        CancellationToken.None);

            Assert.NotNull(categories.Data);
            Assert.Equal(50, categories.Data.Count);
        }


        [Fact(DisplayName = nameof(ShouldAddCategoryToCache))]
        [Trait("Integration - Application.Service.Category", "Add Category Cache")]
        public async Task ShouldAddCategoryToCache()
        {
            // Arrange
            var category = FakerCategories(1).First();
            await _categoryRepository.Create(category, CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var service = new ServiceBuilder.ServiceCacheCategory(_serviceProvider);

            // Act
            await service.AddCategory(category.Id, CancellationToken.None);

            var cachedCategory = await _categoryCacheRepository.GetById(category.Id);

            // Assert
            Assert.NotNull(cachedCategory);
            Assert.Equal(category.Id, cachedCategory.Id);
            Assert.Equal(category.Name, cachedCategory.Name);
        }


        [Fact(DisplayName = nameof(ShouldRemoveCategoryFromCache))]
        [Trait("Integration - Application.Service.Category", "Remove Category Cache")]
        public async Task ShouldRemoveCategoryFromCache()
        {
            // Arrange
            var category = FakerCategories(1).First();
            await _categoryRepository.Create(category, CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var service = new ServiceBuilder.ServiceCacheCategory(_serviceProvider);
            await service.AddCategory(category.Id, CancellationToken.None);

            // Act
            await service.RemoveCategory(category.Id, CancellationToken.None);

            var cachedCategory = await _categoryCacheRepository.GetById(category.Id);

            // Assert
            Assert.Null(cachedCategory);
        }

        [Fact(DisplayName = nameof(ShouldUpdateCategoryInCache))]
        [Trait("Integration - Application.Service.Category", "Update Category Cache")]
        public async Task ShouldUpdateCategoryInCache()
        {
            // Arrange
            var category = FakerCategories(1).First();
            await _categoryRepository.Create(category, CancellationToken.None);
            await _unitOfWork.CommitAsync();

            var service = new ServiceBuilder.ServiceCacheCategory(_serviceProvider);
            await service.AddCategory(category.Id, CancellationToken.None);

            // Act
            category.Update("Updated Category Name");
            await _categoryRepository.Update(category, CancellationToken.None);
            await _unitOfWork.CommitAsync();

            await service.UpdateCategory(category.Id, CancellationToken.None);

            var cachedCategory = await _categoryCacheRepository.GetById(category.Id);

            // Assert
            Assert.NotNull(cachedCategory);
            Assert.Equal(category.Id, cachedCategory.Id);
            Assert.Equal("Updated Category Name", cachedCategory.Name);
        }

    }
}
