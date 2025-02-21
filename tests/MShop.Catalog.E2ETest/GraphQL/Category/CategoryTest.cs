using Microsoft.Extensions.DependencyInjection;
using Mshop.Catalog.E2ETests.Base;
using Mshop.Catalog.E2ETests.GraphQL.Common.Category;
using Mshop.Core.Data;
using Mshop.Core.Enum.Paginated;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using StackExchange.Redis;

namespace MShop.Catalog.E2ETest.GraphQL.Category
{
    public class CategoryTest : CategoryFixtureTest
    {
        private ICategoryCacheRepository _categoryCacheRepository;
        private readonly DateTime _expirationDate;
        private IConnectionMultiplexer _database;
        private readonly StartIndex _startIndex;

        public CategoryTest() : base()
        {
            _categoryCacheRepository = _serviceProvider.GetRequiredService<ICategoryCacheRepository>();
            _database = _serviceProvider.GetRequiredService<IConnectionMultiplexer>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();
            _expirationDate = DateTime.UtcNow.AddMinutes(1);

            DeleteIndexCache(_database.GetDatabase()).Wait();
            CreateIndexCahce(_startIndex).Wait();
        }

        [Fact(DisplayName = nameof(GetCategoryById))]
        [Trait("EndToEnd/API GraphQL", "Category - Endpoints")]
        public async void GetCategoryById()
        {
            var listCategories = FakerCategories(10);

            foreach(var item in listCategories)
            {
                await _categoryCacheRepository.Create(item, _expirationDate, CancellationToken.None);
            }
            
            var category = listCategories.FirstOrDefault();
            var id = category?.Id;


            string query = $@"
                        {{
                            categoryById(id:""{id}"")
                            {{
                                name
                            }}
                        }}";

 
            var result = await _graphQLClient.SendQuery<CategoryByIdResponse>(query);

            Assert.NotNull(result);
            Assert.Equal(result.Data.CategoryById.Name, category.Name);
        }


        [Theory(DisplayName = nameof(SearchCategoriesAndReturnPaginateList))]
        [Trait("EndToEnd/API GraphQL", "Category - Endpoints")]
        [InlineData("Action", 5, 1, 1, 1)]
        [InlineData("Horror", 5, 1, 3, 3)]
        [InlineData("Horror", 5, 2, 0, 3)]
        [InlineData("Scifi", 5, 1, 4, 4)]
        [InlineData("Scifi", 2, 1, 2, 4)]
        [InlineData("Scifi", 3, 2, 1, 4)]
        [InlineData("Others", 5, 1, 0, 0)]
        [InlineData("Robots", 5, 1, 2, 2)]
        public async void SearchCategoriesAndReturnPaginateList(
            string search,
            int perPage,
            int page,
            int expertedItemsCount,
            int expertedTotalCount)
        {
            var categoryNameList = new List<string>()
            {
                "Action",
                "Horror",
                "Horror - Robots",
                "Horror - Based on Real Facts",
                "Drama",
                "Scifi IA",
                "Scifi Space",
                "Scifi Robots",
                "Scifi Future",
            };

            var listCategories = GetListCategories(categoryNameList);

            foreach (var item in listCategories)
            {
                await _categoryCacheRepository.Create(item, _expirationDate, CancellationToken.None);
            }

            Thread.Sleep(1500);

            string query = $@"
                            {{
                                listCategories (page: {page}, perPage: {perPage}, search: ""{search}"", orderBy: ""name"" ,order: ASC) {{ 
                                    total,
                                    currentPage,
                                    perPage,
                                    data {{
                                        name,
                                        id
                                    }}
                                }}
                            }}";


            var result = await _graphQLClient.SendQuery<CategoryPaginateResponse>(query);

            Assert.NotNull(result);
            Assert.True(result.Data.ListCategories.Total == expertedTotalCount);
            Assert.True(result.Data.ListCategories.Data.Count == expertedItemsCount);
            Assert.True(result.Data.ListCategories.CurrentPage == page);
            Assert.True(result.Data.ListCategories.PerPage == perPage);

            foreach (var item in result.Data.ListCategories.Data)
            {
                Assert.Equal(item.Name, listCategories.First(x => x.Id == item.Id).Name);
            }
        }




        [Theory(DisplayName = nameof(SearchCategoriesAndReturnPaginateListSorting))]
        [Trait("EndToEnd/API GraphQL", "Category - Endpoints")]
        [InlineData("name", SearchOrder.Asc)]
        [InlineData("name", SearchOrder.Desc)]
        [InlineData("id",   SearchOrder.Asc)]
        [InlineData("id", SearchOrder.Desc)]
        [InlineData("", SearchOrder.Desc)]

        public async Task SearchCategoriesAndReturnPaginateListSorting(
         string orderBy,
         SearchOrder order,
         string search = "",
         int page = 1,
         int perPage = 10)
        {


            var listCategories = FakerCategories(page * perPage);
            var expectedList = CloneListCategorySort(listCategories, orderBy, order);

            foreach (var item in listCategories)
            {
                await _categoryCacheRepository.Create(item, _expirationDate, CancellationToken.None);
            }

            Thread.Sleep(1500);

            string query = $@"
                        {{
                            listCategories (page: {page}, perPage: {perPage}, search: ""{search}"", orderBy: ""{orderBy}"" ,order:{order.ToString().ToUpper()}) {{ 
                                total,
                                perPage,
                                currentPage,
                                data {{
                                    name,
                                    id
                                }}
                            }}
                        }}";

            var result = await _graphQLClient.SendQuery<CategoryPaginateResponse>(query);

            Assert.NotNull(result);
            Assert.NotNull(result.Data.ListCategories.Data);
            Assert.True(result.Data.ListCategories.CurrentPage == page);
            Assert.True(result.Data.ListCategories.PerPage == perPage);
            Assert.True(result.Data.ListCategories.Data.Count() == listCategories.Count);
            Assert.True(result.Data.ListCategories.Total == listCategories.Count);

            for (int i = 0; i < result.Data.ListCategories.Data.Count; i++)
            {
                var itemExpected = expectedList[i];
                var itemOutPut = result.Data.ListCategories.Data[i];
                Assert.NotNull(itemExpected);
                Assert.NotNull(itemOutPut);
                Assert.Equal(itemExpected.Name, itemOutPut.Name);
                //Assert.Equal(itemExpected.IsActive, itemOutPut.IsActive);
            }
        }

    }

}
