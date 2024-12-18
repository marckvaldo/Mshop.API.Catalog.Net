using Mshop.Core.Cache;
using Mshop.Core.Paginated;
using Mshop.Domain.Entity;

namespace Mshop.Infra.Cache.Interface
{
    public interface IProductCacheRepository : ICacheRepository<Product>
    {
        Task<PaginatedOutPut<Product>> FilterPaginated(PaginatedInPut input, CancellationToken cancellationToken);

        Task<PaginatedOutPut<Product>>? FilterPaginatedPromotion(PaginatedInPut input, CancellationToken cancellationToken);

        Task<PaginatedOutPut<Product>>? FilterPaginatedByCategory(PaginatedInPut input, Guid categoryId, CancellationToken cancellationToken);

        Task<bool> AddProduct(Product entity, DateTime? ExpirationDate, CancellationToken cancellationToken);

        Task<bool> DeleteProduct(Product entity, CancellationToken cancellationToken);

        Task<Product> GetProductById(Guid id);

        Task<bool> UpadteProduct(Product entity, DateTime? ExpirationDate, CancellationToken cancellationToken);

    }

}
