using Mshop.Core.Cache;
using Mshop.Core.Paginated;
using Mshop.Domain.Entity;

namespace Mshop.Infra.Cache.Interface
{
    public interface ICategoryCacheRepository : ICacheRepository<Category>
    {
        Task<PaginatedOutPut<Category>> FilterPaginated(PaginatedInPut input, CancellationToken cancellationToken);

        Task<bool> AddCategory(Category entity, DateTime? ExpirationDate, CancellationToken cancellationToken);

        Task<bool> DeleteCategory(Category entity, CancellationToken cancellationToken);

        Task<Category> GetCategoryById(Guid id);

        Task<bool> UpadteProduct(Category entity, DateTime? ExpirationDate, CancellationToken cancellationToken);
    }
}
