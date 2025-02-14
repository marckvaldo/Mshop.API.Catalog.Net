using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.UseCases.Cache.Category.ListCategoriesCache;
using Mshop.Application.UseCases.Category.Common;
using Mshop.Catalog.E2ETest.Base;
using Mshop.Core.Data;
using Mshop.Core.Enum.Paginated;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.Catalog.E2ETests.API.Common;
using StackExchange.Redis;

namespace Mshop.Catalog.E2ETests.API.Cache.Category
{
    public class CacheAPITest : CacheAPITestFixture
    {
        private ICategoryRepository _categoryRepository;
        private ICategoryCacheRepository _categoryCacheRepository;
        private IConnectionMultiplexer _database;
        private IUnitOfWork _unitOfWork;
        private readonly RepositoryDbContext _DbContext;
        private readonly DateTime _expirationDate;
        private readonly StartIndex _startIndex;
        public CacheAPITest() : base()
        {
            _categoryCacheRepository = _serviceProvider.GetRequiredService<ICategoryCacheRepository>();
            _categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _DbContext = _serviceProvider.GetRequiredService<RepositoryDbContext>();
            _database = _serviceProvider.GetRequiredService<IConnectionMultiplexer>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();
            _expirationDate = DateTime.UtcNow.AddMinutes(1);


            DeleteDataBase(_DbContext,false).Wait();
            AddMigartion(_DbContext).Wait();
            
            DeleteIndexCache(_database.GetDatabase()).Wait();
            CreateIndexCahce(_startIndex).Wait();

        }

        [Fact(DisplayName = nameof(GetCategoryWithOutCacheById))]
        [Trait("EndToEnd/API", "Cache-Category - Endpoints")]
        public async Task GetCategoryWithOutCacheById()
        {
            var category = FakerCategory();
            await _categoryRepository.Create(category, CancellationToken.None);
            await _unitOfWork.CommitAsync();

            await _categoryCacheRepository.DeleteById(category,CancellationToken.None);
            var categoryNull = await _categoryCacheRepository.GetById(category.Id);

            var (response, outPut) = await _apiClient.Get<CustomResponse<CategoryModelOutPut>>($"{Configuration.URL_API_CACHE}category-cache/{category.Id}");
            Thread.Sleep(2000);
            var categoryDb = (await _categoryRepository.Filter(c => c.Id == outPut.Data.Id)).First();
            var categoryCache = await _categoryCacheRepository.GetById(category.Id);


            Assert.Null(categoryNull);
            Assert.NotNull(categoryCache);
            Assert.NotNull(categoryDb);
            Assert.NotNull(response);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(outPut.Data.Name, categoryDb.Name);
            Assert.Equal(outPut.Data.IsActive, categoryDb.IsActive);
            Assert.Equal(outPut.Data.Name, categoryCache.Name);
            Assert.Equal(outPut.Data.IsActive, categoryCache.IsActive);

        }


        [Fact(DisplayName = nameof(GetCategoryWithCacheById))]
        [Trait("EndToEnd/API", "Cache-Category - Endpoints")]
        public async Task GetCategoryWithCacheById()
        {
            var category = FakerCategory();
            await _categoryCacheRepository.Create(category, _expirationDate, CancellationToken.None);

            var categoryCache = await _categoryCacheRepository.GetById(category.Id);
            var categoryDb = await _categoryRepository.GetById(category.Id);

            var (response, outPut) = await _apiClient.Get<CustomResponse<CategoryModelOutPut>>($"{Configuration.URL_API_CACHE}category-cache/{category.Id}");
            Thread.Sleep(2000);
            categoryDb = await _categoryRepository.GetById(category.Id);


            Assert.Null(categoryDb);
            Assert.NotNull(categoryCache);
            Assert.NotNull(response);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(outPut.Data.Name, categoryCache.Name);
            Assert.Equal(outPut.Data.IsActive, categoryCache.IsActive);

        }



        [Fact(DisplayName = nameof(ListCategoryWithOutCache))]
        [Trait("EndToEnd/API", "Cache-Category - Endpoints")]
        public async Task ListCategoryWithOutCache()
        {
            var qtdCategory = 20;
            var perPager = 10;
            var page = 1;
            var categories = FakerCategories(qtdCategory);

            foreach (var category in categories)
            {
                await _categoryRepository.Create(category, CancellationToken.None);
            }
            await _unitOfWork.CommitAsync();

            var categoryNull = await _categoryCacheRepository.FilterPaginated(
                new Mshop.Core.Paginated.PaginatedInPut(
                    page,
                    perPager, 
                    "", 
                    "", 
                    SearchOrder.Asc),CancellationToken.None);

            var query = new ListCategoriesCacheInPut(page, perPager, "", "", SearchOrder.Desc);
            var (response, outPut) = await _apiClient.Get<CustomResponse<ListCategoriesCacheOutPut>>($"{Configuration.URL_API_CACHE}list-categories-cache", query);
            Thread.Sleep(2000);
            var categoriesDb = (await _categoryRepository.Filter(c => c.Name != null)).ToList();

            var categoriesCache = await _categoryCacheRepository.FilterPaginated(
               new Mshop.Core.Paginated.PaginatedInPut(
                   page,
                   perPager, 
                   "", 
                   "", 
                   SearchOrder.Asc),CancellationToken.None);

            Assert.Null(categoryNull);
            Assert.NotNull(categoriesCache);
            Assert.Equal(perPager, categoriesCache.Itens.Count);
            Assert.Equal(20, categoriesDb.Count);
            Assert.NotNull(categories);
            Assert.NotNull(response);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(outPut.Data.Itens);
            Assert.Equal(perPager, outPut.Data.PerPage);
            Assert.Equal(page, outPut.Data.Page);
            Assert.Equal(qtdCategory, outPut.Data.Total);

            foreach (var item in outPut.Data.Itens)
            {
                var categoryCache = await _categoryCacheRepository.GetById(item.Id);
                Assert.NotNull(categoryCache);
                Assert.Equal(categoryCache.Name, item.Name);
                Assert.Equal(categoryCache.IsActive, item.IsActive);

                var categoryDB = await _categoryCacheRepository.GetById(item.Id);
                Assert.NotNull(categoryDB);
                Assert.Equal(categoryDB.Name, item.Name);
                Assert.Equal(categoryDB.IsActive, item.IsActive);
            }

        }


        [Fact(DisplayName = nameof(ListCategoryWithCache))]
        [Trait("EndToEnd/API", "Cache-Category - Endpoints")]
        public async Task ListCategoryWithCache()
        {
            var qtdCategory = 20;
            var perPager = 10;
            var page = 1;
            var categories = FakerCategories(qtdCategory);

            foreach (var category in categories)
            {
                await _categoryCacheRepository.Create(category, _expirationDate, CancellationToken.None);
            }

            var categoriesCache = await _categoryCacheRepository.FilterPaginated(
                new Mshop.Core.Paginated.PaginatedInPut(
                    1,
                    20,
                    "",
                    "",
                    SearchOrder.Asc), CancellationToken.None);


            var query = new ListCategoriesCacheInPut(page, perPager, "", "", SearchOrder.Desc);
            var (response, outPut) = await _apiClient.Get<CustomResponse<ListCategoriesCacheOutPut>>($"{Configuration.URL_API_CACHE}list-categories-cache", query);
            Thread.Sleep(2000);
            var categoriesDb = (await _categoryRepository.Filter(c => c.Name != null)).ToList();


            Assert.NotNull(categoriesCache);
            Assert.Equal(20, categoriesCache.Itens.Count);
            Assert.Equal(0,categoriesDb.Count);
            Assert.NotNull(categories);
            Assert.NotNull(response);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(outPut.Data.Itens);
            Assert.Equal(perPager, outPut.Data.PerPage);
            Assert.Equal(page, outPut.Data.Page);
            Assert.Equal(qtdCategory, outPut.Data.Total);

            foreach (var item in outPut.Data.Itens)
            {
                var categoryCache = await _categoryCacheRepository.GetById(item.Id);
                Assert.NotNull(categoryCache);
                Assert.Equal(categoryCache.Name, item.Name);
                Assert.Equal(categoryCache.IsActive, item.IsActive);
            }

        }

    }
}
