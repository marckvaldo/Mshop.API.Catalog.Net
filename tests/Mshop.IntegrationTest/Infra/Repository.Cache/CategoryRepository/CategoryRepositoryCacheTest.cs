using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Core.Enum.Paginated;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.IntegrationTests.Common;

namespace Mshop.IntegrationTests.Infra.Repository.Cache.CategoryRepository
{
    [Collection("Repository Category Collection")]
    [CollectionDefinition("Repository Category Collection", DisableParallelization = true)]
    public class CategoryRepositoryCacheTest : IntegracaoBaseFixture
    {
        private readonly ICategoryCacheRepository _categoryRepositoryCache;
        private readonly StackExchange.Redis.IDatabase _database;
        private readonly DateTime _expirationDate;
        private readonly StartIndex _startIndex;

        public CategoryRepositoryCacheTest() : base() 
        {
            var connection = _serviceProvider.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
            _database = connection.GetDatabase();
            _categoryRepositoryCache = _serviceProvider.GetRequiredService<ICategoryCacheRepository>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();
            _expirationDate = DateTime.UtcNow.AddHours(1);

            
            DeleteCache(_database).Wait();
            _startIndex.CreateIndex().Wait();

        }

        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("Integration - Infra.Cache", "Category Repositorio")]

        public async Task CreateCategory()
        {
            
            var request = FakerCategory();
            await _categoryRepositoryCache.Create(request, _expirationDate, CancellationToken.None);
            var newCategory = await _categoryRepositoryCache.GetById(request.Id);

            Assert.NotNull(newCategory);
            Assert.Equal(newCategory.Name, request.Name);
            Assert.Equal(newCategory.IsActive, request.IsActive);
            Assert.Equal(newCategory.Id, request.Id);
        }


        [Fact(DisplayName = nameof(GetByIdCategory))]
        [Trait("Integration - Infra.Cache", "Category Repositorio")]

        public async Task GetByIdCategory()
        {
            var categoryFaker = FakerCategory();

            await _categoryRepositoryCache.Create(categoryFaker, _expirationDate, CancellationToken.None);
            var category = await _categoryRepositoryCache.GetById(categoryFaker.Id);

            Assert.NotNull(category);
            Assert.Equal(category.Name, categoryFaker.Name);
            Assert.Equal(category.IsActive, categoryFaker.IsActive);    
        }


        [Fact(DisplayName = nameof(UpdateProduct))]
        [Trait("Integration - Infra.Cache", "Category Repositorio")]

        public async Task UpdateProduct()
        {
            var categoryFaker = FakerCategories(3);
            foreach (var item in categoryFaker)
            {
                await _categoryRepositoryCache.Create(item, _expirationDate, CancellationToken.None);
            }
            

            var category = categoryFaker.FirstOrDefault();
            Assert.NotNull(category);
            category.Update(faker.Commerce.Categories(1)[0]);
            category.Deactive();

            await _categoryRepositoryCache.Update(category,_expirationDate, CancellationToken.None);
            var categoryDb = await _categoryRepositoryCache.GetById(category.Id);

            Assert.NotNull(categoryDb);            
            Assert.Equal(categoryDb.Name, category.Name);
            Assert.Equal(categoryDb.IsActive, category.IsActive);

        }


        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("Integration - Infra.Cache", "Category Repositorio")]

        public async Task DeleteCategory()
        {
            var quantity = 3;
            var categoryFaker = FakerCategories(quantity);
            foreach (var item in categoryFaker)
            {
                await _categoryRepositoryCache.Create(item, _expirationDate, CancellationToken.None);
            }


            var categoryDelete = categoryFaker.FirstOrDefault();    
            Assert.NotNull(categoryDelete);
            await _categoryRepositoryCache.DeleteById(categoryDelete, CancellationToken.None);
            var outPut = await _categoryRepositoryCache.FilterPaginated(
                new Core.Paginated.PaginatedInPut(
                   currentPage:1,
                    perPage: 10,
                    search: "",
                    orderBy: "",
                    order: SearchOrder.Desc
                    ),
                CancellationToken.None);

            Assert.NotNull(outPut);
            Assert.Equal(quantity - 1, outPut.Data.Count());
            Assert.Equal(0, outPut.Data?.Where(x => x.Id == categoryDelete.Id).Count());
            
        }

        [Fact(DisplayName = nameof(FilterPaginated))]
        [Trait("Integration - Infra.Cache", "Category Repositorio")]

        public async Task FilterPaginated()
        {
            var quantity = 20;
            var perPage = 10;
            var categories = FakerCategories(quantity);

            foreach (var item in categories)
            {
                await _categoryRepositoryCache.Create(item, _expirationDate, CancellationToken.None);
            }

            var request = new Core.Paginated.PaginatedInPut(1, perPage, "", "", Core.Enum.Paginated.SearchOrder.Desc);

            var outPut = await _categoryRepositoryCache.FilterPaginated(request, CancellationToken.None);

            Assert.NotNull(outPut);
            Assert.NotNull(outPut?.Data);
            Assert.Equal(outPut.Total, quantity);
            Assert.Equal(outPut?.PerPage, perPage);
            Assert.Equal(outPut?.Data.Count(), perPage);
            Assert.Equal(outPut?.CurrentPage, 1);

            foreach(var item in outPut?.Data?.ToList())
            {
                var category = categories.Where(x => x.Id == item.Id).FirstOrDefault();
                Assert.NotNull(category);
                Assert.Equal(category.Name, item.Name);
                Assert.Equal(category.IsActive, item.IsActive);
            }

        }


        [Fact(DisplayName = nameof(SholdResultListEmptyFilterPaginated))]
        [Trait("Integration - Infra.Cache", "Category Repositorio")]
        public async Task SholdResultListEmptyFilterPaginated()
        {
            var perPage = 20;
            var input = new Core.Paginated.PaginatedInPut(1, perPage, "", "", Core.Enum.Paginated.SearchOrder.Asc);
            var outPut = await _categoryRepositoryCache.FilterPaginated(input, CancellationToken.None);

            Assert.Null(outPut);
        }


        [Theory(DisplayName = nameof(SerachRestusPaginated))]
        [Trait("Integration - Infra.Cache", "Category Repositorio")]
        [InlineData(10, 1, 10, 10)]
        [InlineData(17, 2, 10, 7)]
        [InlineData(21, 3, 10, 1)]

        public async Task SerachRestusPaginated(int quantityProduct, int page, int perPage, int expectedQuantityItems)
        {
            var categoryList = FakerCategories(quantityProduct);
            foreach (var item in categoryList)
            {
                await _categoryRepositoryCache.Create(item, _expirationDate, CancellationToken.None);
            }

            var input = new Core.Paginated.PaginatedInPut(page, perPage, "", "", Core.Enum.Paginated.SearchOrder.Asc);
            var outPut = await _categoryRepositoryCache.FilterPaginated(input, CancellationToken.None);

            Assert.NotNull(outPut);
            Assert.NotNull(outPut.Data);
            Assert.True(outPut.Data.Count == expectedQuantityItems);
            Assert.Equal(outPut.PerPage, perPage);
            Assert.True(outPut.Total == quantityProduct);
            Assert.Equal(input.PerPage, outPut.PerPage);

            foreach (var item in outPut.Data)
            {
                var category = categoryList.Where(x => x.Id == item.Id).FirstOrDefault();
                Assert.NotNull(category);
                Assert.Equal(category.Name, item.Name);
                Assert.Equal(category.IsActive, item.IsActive);
            }

        }


    }
}
