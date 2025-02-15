using Microsoft.AspNetCore.Mvc.RazorPages;
using Mshop.Core.Common;
using Mshop.Core.Paginated;
using Mshop.Domain.Entity;
using Mshop.Infra.Cache.CircuitBreakerPolicy;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using StackExchange.Redis;
using System.Text;

namespace Mshop.Infra.Cache.Respository
{

    public class ProductCacheRepository : IProductCacheRepository
    {
        private readonly IDatabase _database;
        private readonly SearchCommands _search;
        private readonly string _indexName = "ProductIndex";
        private readonly string _keyPrefix = "Product:";

        private readonly ICircuitBreaker _circuitBreaker;

        public ProductCacheRepository(IConnectionMultiplexer database, ICircuitBreaker circuitBreaker)
        {
            _database = database.GetDatabase();
            _search = _database.FT();

            _indexName = $"{IndexName.Product}Index";
            _keyPrefix = $"{IndexName.Product}:";

            _circuitBreaker = circuitBreaker;

            circuitBreaker.Start(
            ex =>
            {
                return ex is RedisConnectionException || ex is TimeoutException || ex is Exception;
            },
            1,
            TimeSpan.FromMinutes(1));

        }
        public async Task<bool> Create(Product entity, DateTime? ExpirationDate, CancellationToken cancellationToken)
        {
            var key = $"{_keyPrefix}{entity.Id}";

            var hash = new HashEntry[]
            {
                new("Id", entity.Id.ToString()),
                new("Name", entity.Name),
                new("Description", entity.Description),
                new("Price", entity.Price.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"))),
                new("Stock", entity.Stock.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"))),
                new("IsActive", entity.IsActive),
                new("IsSale", entity.IsSale),
                new("CategoryId", Helpers.ClearString(entity.CategoryId.ToString())),
                new("Category", entity.Category.Name),
                new("Thumb", entity.Thumb?.Path)
            };

            await _database.HashSetAsync(key, hash);

            if (ExpirationDate.HasValue)
            {
                await _database.KeyExpireAsync(key, ExpirationDate.Value);
            }

            return true;
        }
        public async Task<bool> DeleteById(Product entity, CancellationToken cancellationToken)
        {
            var key = $"{_keyPrefix}{entity.Id.ToString()}";
            return await _database.KeyDeleteAsync(key);
        }
        public Task<PaginatedOutPut<Product>>? FilterPaginated(PaginatedInPut input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task<PaginatedOutPut<Product>>? FilterPaginatedQuery(PaginatedInPut input, Guid categoryId, bool onlyPromotion, CancellationToken cancellationToken)
        {
            try
            {
                return await _circuitBreaker.ExecuteActionAsync(async () => await SearchProducts(input, categoryId, onlyPromotion));
            }
            catch (RedisConnectionException erro)
            {
                Console.WriteLine($"CircuitBreaker ativado devido a: {erro.Message}");
                return null;
            }
            catch (Exception erro)
            {
                Console.WriteLine($"CircuitBreaker ativado devido a: {erro.Message}");
                return null;
            }
        }

        private async Task<PaginatedOutPut<Product>>? SearchProducts(PaginatedInPut input, Guid categoryId, bool onlyPromotion)
        {
            //FT.SEARCH "productsIndex" "@Name: @IsSale:{0} @CategoryId:{117e165769044bd0add3fffd8466cd03}"

            var offset = (input.Page - 1) * input.PerPage;

            var Id = Helpers.ClearString(categoryId.ToString());

            var queryBuilder = new StringBuilder();

            if (string.IsNullOrEmpty(input.Search))
                queryBuilder.Append($"@Name: ");
            else
                queryBuilder.Append($"@Name:{input.Search}*");

            if (onlyPromotion)
            {
                if (queryBuilder.Length > 0) queryBuilder.Append(" ");
                queryBuilder.Append($"@IsSale:{{1}}");
            }

            if (categoryId != Guid.Empty)
            {
                if (queryBuilder.Length > 0) queryBuilder.Append(" ");
                queryBuilder.Append($"@CategoryId:{{{Id}}}");
            }

            if (!onlyPromotion && categoryId == Guid.Empty && string.IsNullOrEmpty(input.Search))
            {
                queryBuilder.Clear();
                queryBuilder.Append("*");
            }

            /*var query = string.IsNullOrEmpty(input.Search)
                ? new Query("*")
                : new Query($"@Name:{input.Search}*");*/

            string querystring = queryBuilder.ToString();
            var query = new Query(querystring);

            // Obter o total de resultados sem paginação
            var totalResult = await _search.SearchAsync(_indexName, query);
            var totalItems = (int)totalResult.TotalResults;

            var result = await _search.SearchAsync(_indexName, query.Limit(offset, input.PerPage));


            if (result.Documents.Count == 0)
                return null;

            var products = result.Documents.Select(doc => RedisToProduct(doc)).ToList();

            return new PaginatedOutPut<Product>(
                input.Page,
                input.PerPage,
                totalItems,
                products);
        }


        public async Task<PaginatedOutPut<Product>>? FilterPaginatedPromotion(PaginatedInPut input, CancellationToken cancellationToken)
        {
            var offset = (input.Page - 1) * input.PerPage;

            var query = $"@Name:{input.Search}* @IsSale:{{{1}}}";

            if (string.IsNullOrWhiteSpace(input.Search))
                query = $"@IsSale:{{{1}}}";

            // Obter o total de resultados sem paginação
            var totalResult = await _search.SearchAsync(_indexName, new Query(query));
            var totalItems = (int)totalResult.TotalResults;

            var result = await _search.SearchAsync(_indexName, new Query(query).Limit(offset, input.PerPage));

            if (result.Documents.Count == 0)
                return null;

            var products = result.Documents.Select(doc => RedisToProduct(doc)).ToList();

            return new PaginatedOutPut<Product>(
                input.Page,
                input.PerPage,
                totalItems,
                products);
        }
        public async Task<PaginatedOutPut<Product>>? FilterPaginatedByCategory(PaginatedInPut input, Guid categoryId, CancellationToken cancellationToken)
        {
            var offset = (input.Page - 1) * input.PerPage;

            var Id = Helpers.ClearString(categoryId.ToString());

            var query = $"@Name:{input.Search}* CategoryId:{{{Id}}}";

            if (string.IsNullOrWhiteSpace(input.Search))
                query = $"@CategoryId:{{{Id}}}";

            // Obter o total de resultados sem paginação
            var totalResult = await _search.SearchAsync(_indexName, new Query(query));
            var totalItems = (int)totalResult.TotalResults;

            var result = await _search.SearchAsync(_indexName, new Query(query).Limit(offset, input.PerPage));

            if (result.Documents.Count == 0)
                return null;

            var products = result.Documents.Select(doc => RedisToProduct(doc)).ToList();

            return new PaginatedOutPut<Product>(
                input.Page,
                input.PerPage,
                totalItems,
                products);
        }
        public async Task<Product?> GetById(Guid id)
        {
            try
            {
                return await _circuitBreaker.ExecuteActionAsync(async () => await SearchById(id));
            }
            catch (RedisConnectionException erro)
            {
                Console.WriteLine($"CircuitBreaker ativado devido a: {erro.Message}");
                return null;
            }
            catch (Exception erro)
            {
                Console.WriteLine($"CircuitBreaker ativado devido a: {erro.Message}");
                return null;
            }

        }

        private async Task<Product?> SearchById(Guid id)
        {
            var key = $"{_keyPrefix}{id}";
            var hash = await _database.HashGetAllAsync(key);

            if (hash.Length == 0)
                return null;

            return RedisToProduct(hash);
        }

        public async Task<bool> Update(Product entity, DateTime? ExpirationDate, CancellationToken cancellationToken)
        {
            return await Create(entity, ExpirationDate, cancellationToken);
        }








        public Task DeleteKey(string key)
        {
            throw new NotImplementedException();
        }

        public Task<TResult?> GetKey<TResult>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<List<TResult?>?> GetKeyCollection<TResult>(string key)
        {
            throw new NotImplementedException();
        }

        public Task SetKey(string key, object value, TimeSpan TimeExpiration)
        {
            throw new NotImplementedException();
        }

        public Task SetKeyCollection(string key, object value, TimeSpan TimeExpiration)
        {
            throw new NotImplementedException();
        }







        private Product RedisToProduct(HashEntry[] hash )
        {
            bool isActive = hash.FirstOrDefault(x => x.Name == "IsActive").Value.ToString() == "1" ? true : false;
            bool isPromotion = hash.FirstOrDefault(x => x.Name == "IsSales").Value.ToString() == "1" ? true : false;

            var product = new Product(
                description: hash.FirstOrDefault(x => x.Name == "Description").Value.ToString() ?? string.Empty,
                name: hash.FirstOrDefault(x => x.Name == "Name").Value.ToString() ?? string.Empty,
                price: decimal.Parse(hash.FirstOrDefault(x => x.Name == "Price").Value.ToString(), System.Globalization.CultureInfo.InvariantCulture),
                categoryId: Guid.Parse(hash.FirstOrDefault(x => x.Name == "CategoryId").Value.ToString()),
                stock: decimal.Parse(hash.FirstOrDefault(x => x.Name == "Stock").Value.ToString(), System.Globalization.CultureInfo.InvariantCulture),
                isActive: isActive,
                id: Guid.Parse(hash.FirstOrDefault(x => x.Name == "Id").Value.ToString()),
                isSale: isPromotion
                );
            
            product.AddCategory(
                    new Category(
                        name:hash.FirstOrDefault(x => x.Name == "Category").Value.ToString(),
                        id: Guid.Parse(hash.FirstOrDefault(x => x.Name == "CategoryId").Value.ToString())
                        )
                    );
            product.UpdateThumb(
                    hash.FirstOrDefault(x => x.Name == "Thumb").Value.ToString()
                    );


            return product;
  
            
        }

        private Product RedisToProduct(Document doc)
        {
            bool isActive = doc["IsActive"].ToString() == "1" ? true : false;
            bool isPromotion = doc["IsSale"].ToString() == "1" ? true : false;

            var product = new Product(
                name: doc["Name"].ToString() ?? string.Empty,
                description: doc["Description"].ToString() ?? string.Empty,
                price: decimal.Parse(doc["Price"].ToString(), System.Globalization.CultureInfo.InvariantCulture),
                categoryId: Guid.Parse(doc["CategoryId"].ToString() ?? string.Empty),
                stock: decimal.Parse(doc["Stock"].ToString() ?? "0"),
                isActive: isActive,
                id: Guid.Parse(doc["Id"].ToString() ?? string.Empty),
                isSale:isPromotion
                );

            product.AddCategory(
                    new Category(
                        name: doc["Category"].ToString() ?? string.Empty,
                        id: Guid.Parse(doc["CategoryId"].ToString() ?? string.Empty)
                        )
                    );
            product.UpdateThumb(
                    doc["Thumb"].ToString() ?? string.Empty
                    );

            return product;


        }

        
    }
}
