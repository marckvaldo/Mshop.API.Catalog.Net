using Microsoft.AspNetCore.Mvc.RazorPages;
using Mshop.Core.Common;
using Mshop.Core.Paginated;
using Mshop.Domain.Entity;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using StackExchange.Redis;

namespace Mshop.Infra.Cache.Respository
{

    public class ProductCacheRepository : IProductCacheRepository
    {
        private readonly IDatabase _database;
        private readonly SearchCommands _search;
        private readonly string _indexName = "ProductIndex";
        private readonly string _keyPrefix = "Product:";

        public ProductCacheRepository(IConnectionMultiplexer database)
        {
            _database = database.GetDatabase();
            _search = _database.FT();

            _indexName = $"{IndexName.Product}Index";
            _keyPrefix = $"{IndexName.Product}:";
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
        public async Task<PaginatedOutPut<Product>>? FilterPaginated(PaginatedInPut input, CancellationToken cancellationToken)
        {
            var offset = (input.Page - 1) * input.PerPage;

            var query = string.IsNullOrEmpty(input.Search)
                ? new Query("*")
                : new Query($"@Name:{input.Search}*");

            var result = await _search.SearchAsync(_indexName, query.Limit(offset, input.PerPage));


            if (result.Documents.Count == 0)
                return null;

            var products = result.Documents.Select(doc => RedisToProduct(doc)).ToList();

            return new PaginatedOutPut<Product>(
                input.Page,
                input.PerPage,
                (int)result.TotalResults,
                products);
        }
        public async Task<PaginatedOutPut<Product>>? FilterPaginatedPromotion(PaginatedInPut input, CancellationToken cancellationToken)
        {
            var offset = (input.Page - 1) * input.PerPage;

            var query = $"@Name:{input.Search}* @IsSale:True";
            if (string.IsNullOrWhiteSpace(input.Search))
                query = $"@IsSale:True";

            

            var result = await _search.SearchAsync(_indexName, new Query(query).Limit(offset, input.PerPage));

            if (result is null)
                return null;

            var products = result.Documents.Select(doc => RedisToProduct(doc)).ToList();

            return new PaginatedOutPut<Product>(
                input.Page,
                input.PerPage,
                (int)result.TotalResults,
                products);
        }
        public async Task<PaginatedOutPut<Product>>? FilterPaginatedByCategory(PaginatedInPut input, Guid categoryId, CancellationToken cancellationToken)
        {
            var offset = (input.Page - 1) * input.PerPage;

            var Id = Helpers.ClearString(categoryId.ToString());

            var query = $"@Name:{input.Search}* CategoryId:{Id}";

            if (string.IsNullOrWhiteSpace(input.Search))
                query = $"CategoryId:{Id}";

           

            var result = await _search.SearchAsync(_indexName, new Query(query).Limit(offset, input.PerPage));

            if (result is null)
                return null;

            var products = result.Documents.Select(doc => RedisToProduct(doc)).ToList();

            return new PaginatedOutPut<Product>(
                input.Page,
                input.PerPage,
                (int)result.TotalResults,
                products);
        }
        public async Task<Product?> GetById(Guid id)
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
                stock: decimal.Parse(hash.FirstOrDefault(x => x.Name == "Stock").Value.ToString()),
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
