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
    public class CategoryCacheRepository: ICategoryCacheRepository
    {
        private readonly IDatabase _database;
        private readonly SearchCommands _search;
        private readonly string _indexName = "CategoryIndex";
        private readonly string _keyPrefix = "Category:";

        public CategoryCacheRepository(IConnectionMultiplexer database)
        {
            _database = database.GetDatabase();
            _search = _database.FT();

            _indexName = $"{IndexName.Category}Index";
            _keyPrefix = $"{IndexName.Category}:";
        }
        public async Task<bool> Create(Category entity, DateTime? ExpirationDate, CancellationToken cancellationToken)
        {
            var key = $"{_keyPrefix}{entity.Id}";

            var hash = new HashEntry[]
            {
                new("Id", entity.Id.ToString()),
                new("Name", entity.Name),
                new("IsActive", entity.IsActive)
            };

            await _database.HashSetAsync(key, hash);

            if (ExpirationDate.HasValue)
            {
                await _database.KeyExpireAsync(key, ExpirationDate.Value);
            }

            return true;
        }
        public async Task<bool> DeleteById(Category entity, CancellationToken cancellationToken)
        {
            var key = $"{_keyPrefix}{entity.Id}";
            return await _database.KeyDeleteAsync(key);
        }
        public async Task<PaginatedOutPut<Category>>? FilterPaginated(PaginatedInPut input, CancellationToken cancellationToken)
        {
            var offset = (input.Page - 1) * input.PerPage;

            var query = string.IsNullOrEmpty(input.Search) 
                ? new Query("*") 
                : new Query($"@Name:{input.Search}*");

            var result = await _search.SearchAsync(_indexName, query.Limit(offset, input.PerPage));

            if (result.Documents.Count == 0)
                return null;

            var cartegory = result.Documents.Select(doc => RedisToCategory(doc)).ToList();

            return new PaginatedOutPut<Category>(
                input.Page,
                input.PerPage,
                (int)result.TotalResults,
                cartegory);
        }
        public async Task<Category?> GetById(Guid id)
        {
            var key = $"{_keyPrefix}{id}";
            var hash = await _database.HashGetAllAsync(key);

            if (hash.Length == 0)
                return null;

            return RedisToCategory(hash);
        }
        public async Task<bool> Update(Category entity, DateTime? ExpirationDate, CancellationToken cancellationToken)
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

        



        private Category RedisToCategory(HashEntry[] hash)
        {
            var category = new Category(
                    name:hash.FirstOrDefault(x => x.Name == "Name").Value.ToString() ?? string.Empty,
                    isActive:(hash.FirstOrDefault(x => x.Name == "IsActive").Value.ToString() == "1" ? true : false),
                    id: Guid.Parse(hash.FirstOrDefault(x => x.Name == "Id").Value.ToString())
                );

            return category;
        }

        private Category RedisToCategory(Document doc)
        { 

            bool IsActive = (doc["IsActive"].ToString() == "1" ? true : false);

            var category = new Category(
                    doc["Name"].ToString() ?? string.Empty,
                    Guid.Parse(doc["Id"].ToString()),
                    IsActive
                );
            
            return category;


        }
    }
}
