using Microsoft.AspNetCore.Mvc.RazorPages;
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

    public class ImagesCacheRepository : IImagesCacheRepository
    {
        private readonly IDatabase _database;
        private readonly SearchCommands _search;
        private readonly string _indexName = "ImagesIndex";
        private readonly string _keyPrefix = "Images:";

        public ImagesCacheRepository(IConnectionMultiplexer database)
        {
            _database = database.GetDatabase();
            _search = _database.FT();

            _indexName = $"{IndexName.Product}Index";
            _keyPrefix = $"{IndexName.Product}:";
        }
        public async Task<bool> AddImage(Image entity, DateTime? ExpirationDate, CancellationToken cancellationToken)
        {
            var key = $"{_keyPrefix}:{entity.Id}";

            var hash = new HashEntry[]
            {   
                new("Id", entity.Id.ToString()),
                new("FileName", entity.FileName),
                new("ProductId", entity.ProductId.ToString())
            };

            await _database.HashSetAsync(key, hash);

            if (ExpirationDate.HasValue)
            {
                await _database.KeyExpireAsync(key, ExpirationDate.Value);
            }

            return true;
        }
        public async Task<bool> DeleteImage(Image entity, CancellationToken cancellationToken)
        {
            var key = $"{_keyPrefix}:{entity.Id.ToString()}";
            return await _database.KeyDeleteAsync(key);
        }        
        public async Task<Image?> GetImageById(Guid id)
        {
            var key = $"{_keyPrefix}{id}";
            var hash = await _database.HashGetAllAsync(key);

            if (hash.Length == 0) 
                return null;

            return RedisToImage(hash);
        }
        public async Task<IEnumerable<Image>?> GetImageByProductId(Guid productId)
        {
            var query = $"@ProductId:{productId.ToString()}";

            var result = await _search.SearchAsync(_indexName, new Query(query).Limit(0, 100));

            if (result is null)
                return null;

            var imagens = result.Documents.Select(doc => RedisToImage(doc)).ToList();

            return imagens;
       
        }
        public async Task<bool> UpadteImage(Image entity, DateTime? ExpirationDate, CancellationToken cancellationToken)
        {
            return await AddImage(entity, ExpirationDate, cancellationToken);
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







        private Image RedisToImage(HashEntry[] hash )
        {
            var image = new Image(
                hash.FirstOrDefault(x => x.Name == "FileName").Value.ToString() ?? string.Empty,
                Guid.Parse(hash.FirstOrDefault(x => x.Name == "ProductId").Value.ToString() ?? string.Empty)
                );

            return image;
  
        }

        private Image RedisToImage(Document doc)
        {
            var Image = new Image(
                doc["FileName"].ToString() ?? string.Empty,
                Guid.Parse(doc["ProductId"].ToString())             
                );

            return Image;
        }

        
    }
}
