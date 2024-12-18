using Mshop.Core.DomainObject;
using Mshop.Core.Paginated;

namespace Mshop.Core.Cache;

public interface ICacheRepository<TEntity>  where TEntity : Entity
{
    Task<TResult?> GetKey<TResult>(string key);
    Task SetKey(string key, object value, TimeSpan TimeExpiration);
    Task SetKeyCollection(string key, object value, TimeSpan TimeExpiration);
    Task DeleteKey(string key);
    Task<List<TResult?>?> GetKeyCollection<TResult>(string key);    
}
