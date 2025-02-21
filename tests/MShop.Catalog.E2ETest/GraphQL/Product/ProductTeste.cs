using Bogus.DataSets;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Catalog.E2ETests.Base;
using Mshop.Catalog.E2ETests.GraphQL.Common.Product;
using Mshop.Domain.Entity;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Catalog.E2ETests.GraphQL.Product
{
    public class ProductTeste : ProductTesteFixture
    {
        private IProductCacheRepository _productCacheRepository;
        private readonly DateTime _expirationDate;
        private IConnectionMultiplexer _database;
        private readonly StartIndex _startIndex;
        public ProductTeste() 
        {
            _productCacheRepository = _serviceProvider.GetRequiredService<IProductCacheRepository>();
            _database = _serviceProvider.GetRequiredService<IConnectionMultiplexer>();
            _startIndex = _serviceProvider.GetRequiredService<StartIndex>();
            _expirationDate = DateTime.UtcNow.AddMinutes(1);

            DeleteIndexCache(_database.GetDatabase()).Wait();
            CreateIndexCahce(_startIndex).Wait();
        }



        [Fact(DisplayName = nameof(GetProductById))]
        [Trait("EndToEnd/API GraphQL", "Product - Endpoints")]
        public async Task GetProductById()
        {
            var category = FakerCategory();
            var products = FakerProducts(10, category);
            foreach (var item in products) 
            {
                await _productCacheRepository.Create(item, _expirationDate,CancellationToken.None);
            }

            var product = products.FirstOrDefault();
            var id = product.Id;

            string query = $@"
                        {{
                          productById(id: ""{id}""){{
                            name,
                            category{{
                                        name,
                                        id,
                                        isActive
                                    }},
                            categoryId,
                            stock,
                            price,
                            isPromotion,
                            thumb,
                            description,
                            id
                          }}
                        }}";

            var result = await _graphQLClient.SendQuery<ProductByIdResponse>(query);

            Assert.NotNull(result);
            Assert.Equal(result.Data.ProductById.Name, product?.Name);
            Assert.Equal(((decimal)result.Data.ProductById.Price), product?.Price);
            Assert.Equal(result.Data.ProductById.Category.Name, product?.Category.Name);       
            Assert.Equal(result.Data.ProductById.Category.Id, product?.CategoryId);
            Assert.Equal(result.Data.ProductById.Thumb, product?.Thumb?.Path);
            Assert.Equal(result.Data.ProductById.IsPromotion, product?.IsSale);
            Assert.Equal(result.Data.ProductById.Stock, product?.Stock);
            

        }


        [Theory(DisplayName = nameof(SearchProductAndReturnPaginateList))]
        [Trait("EndToEnd/API GraphQL", "Product - Endpoints")]
        [InlineData("Action", 5, 1, 1, 1)]
        [InlineData("Horror", 5, 1, 3, 3)]
        [InlineData("Horror", 5, 2, 0, 3)]
        [InlineData("Scifi", 5, 1, 4, 4)]
        [InlineData("Scifi", 2, 1, 2, 4)]
        [InlineData("Scifi", 3, 2, 1, 4)]
        [InlineData("Others", 5, 1, 0, 0)]
        [InlineData("Robots", 5, 1, 2, 2)]
        public async void SearchProductAndReturnPaginateList(
            string search,
            int perPage,
            int page,
            int expertedItemsCount,
            int expertedTotalCount)
        {
            var productNameList = new List<string>()
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

            var listProducts = GetListProducts(productNameList);

            foreach (var item in listProducts)
            {
                await _productCacheRepository.Create(item, _expirationDate, CancellationToken.None);
            }

            Thread.Sleep(1500);

            string query = $@"
                          {{
                              listProduct(search: ""{search}"", page: {page}, perPage: {perPage}, sort: ""name"", onlyPromotion: false, order: ASC, categoryId: """")
                                {{
                                total,
                                perPage,
                                currentPage,
                                data {{
                                    name,
                                    category{{
                                                name,
                                                id,
                                                isActive
                                            }},
                                    categoryId,
                                    stock,
                                    price,
                                    isPromotion,
                                    thumb,
                                    description,
                                    id
                                    }}
                                }}
                            }}";


            var result = await _graphQLClient.SendQuery<ProductPaginateResponse>(query);

            Assert.NotNull(result);
            Assert.True(result.Data.ListProduct.Total == expertedTotalCount);
            Assert.True(result.Data.ListProduct.Data.Count == expertedItemsCount);
            Assert.True(result.Data.ListProduct.CurrentPage == page);
            Assert.True(result.Data.ListProduct.PerPage == perPage);

            foreach (var item in result.Data.ListProduct.Data)
            {
                Assert.Equal(item.Name, listProducts.First(x => x.Id == item.Id).Name);
                Assert.Equal(item.Price, listProducts.First(x => x.Id == item.Id).Price);
                Assert.Equal(item.IsPromotion, listProducts.First(x => x.Id == item.Id).IsSale);
                Assert.Equal(item.Stock, listProducts.First(x => x.Id == item.Id).Stock);
                Assert.Equal(item.Price, listProducts.First(x => x.Id == item.Id).Price);
                Assert.Equal(item.Category.Name, listProducts.First(x => x.Id == item.Id).Category.Name);
                Assert.Equal(item.CategoryId, listProducts.First(x => x.Id == item.Id).CategoryId.ToString());
                Assert.Equal(item.Description, listProducts.First(x => x.Id == item.Id).Description);
                Assert.Equal(item.Thumb, listProducts.First(x => x.Id == item.Id).Thumb.Path ?? "");
            }
        }

    }
}
