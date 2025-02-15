using Microsoft.AspNetCore.Mvc.Filters;
using Mshop.Core.Paginated;
using Mshop.Domain.Entity;
using Mshop.Infra.Cache.CircuitBreakerPolicy;
using Mshop.Infra.Cache.Interface;
using Mshop.Infra.Cache.StartIndex;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using Polly;
using Polly.CircuitBreaker;
using StackExchange.Redis;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mshop.Infra.Cache.Respository
{
    public class CategoryCacheRepository: ICategoryCacheRepository
    {
        private readonly IDatabase _database;
        private readonly SearchCommands _search;
        private readonly string _indexName = "CategoryIndex";
        private readonly string _keyPrefix = "Category:";

        private readonly ICircuitBreaker _circuitBreaker;

        public CategoryCacheRepository(IConnectionMultiplexer database, ICircuitBreaker circuitBreaker)
        {
            _database = database.GetDatabase();
            _search = _database.FT();

            _indexName = $"{IndexName.Category}Index";
            _keyPrefix = $"{IndexName.Category}:";

            _circuitBreaker = circuitBreaker;

            circuitBreaker.Start(
            ex =>
            {
                return ex is RedisConnectionException || ex is TimeoutException || ex is Exception;
            }, 
            1, 
            TimeSpan.FromMinutes(1));
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
            try
            {
                return await _circuitBreaker.ExecuteActionAsync(async () => await SearchPaginate(input));              
            }
            catch(RedisConnectionException erro)
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

        private async Task<PaginatedOutPut<Category>>? SearchPaginate(PaginatedInPut input)
        {
            var offset = (input.Page - 1) * input.PerPage;

            var query = string.IsNullOrEmpty(input.Search)
                ? new Query("*")
                : new Query($"@Name:{input.Search}*");
            
            query = await Ordering(input, query);


            // Obter o total de resultados sem paginação
            var totalResult = await _search.SearchAsync(_indexName, query);
            var totalItems = (int)totalResult.TotalResults;



            var result = await _search.SearchAsync(_indexName, query.Limit(offset, input.PerPage));

            if (result.Documents.Count == 0 && totalItems == 0)
                return null;

            var cartegory = result.Documents.Select(doc => RedisToCategory(doc)).ToList();

            return new PaginatedOutPut<Category>(
                input.Page,
                input.PerPage,
                totalItems,
                cartegory);
        }

        private async Task<Query> Ordering(PaginatedInPut input, Query query)
        {
            switch (input.Order, input.OrderBy.ToLower())
            {
                case (Core.Enum.Paginated.SearchOrder.Asc, "name"):
                    query.SetSortBy("Name", true);
                    break;
                case (Core.Enum.Paginated.SearchOrder.Desc, "name"):
                    query.SetSortBy("Name", false);
                    break;
                case (Core.Enum.Paginated.SearchOrder.Asc, "id"):
                    query.SetSortBy("Id", true);
                    break;
                case (Core.Enum.Paginated.SearchOrder.Desc, "id"):
                    query.SetSortBy("Id", false);
                    break;
                default:
                    query.SetSortBy("Name", true);
                    break;
            }

            return query;
        }

        public async Task<Category?> GetById(Guid id)
        {

            try
            {
                return await _circuitBreaker.ExecuteActionAsync( async () => await FindById(id));
            }
            catch(RedisConnectionException erro)
            {
                Console.WriteLine($"CircuitBreaker ativado devido a: {erro.Message}");
                return null;
            }
            catch(Exception erro)
            {
                Console.WriteLine($"CircuitBreaker ativado devido a: {erro.Message}");
                return null;
            }
        }

        private async Task<Category?> FindById(Guid id)
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
