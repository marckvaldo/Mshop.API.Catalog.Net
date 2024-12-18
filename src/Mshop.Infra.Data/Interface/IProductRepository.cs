using Mshop.Domain.Entity;
using Mshop.Core.Data;
using Mshop.Core.Paginated;

namespace Mshop.Infra.Data.Interface
{
    public interface IProductRepository : IRepository<Product>, IPaginated<Product>
    {
        Task<List<Product>> GetProductsPromotions();

        Task<List<Product>> GetProductsByCategoryId(Guid categoryId);

        Task<Product> GetProductWithCategory(Guid id);

        Task<List<Product>> GetProductAll(bool OnlyActive = false);

        Task<PaginatedOutPut<Product>> FilterPaginatedByCategoryId(PaginatedInPut input, Guid categoryId, CancellationToken cancellationToken);

        Task<PaginatedOutPut<Product>> FilterPaginatedPromotion(PaginatedInPut input, CancellationToken cancellationToken);
    }
}
