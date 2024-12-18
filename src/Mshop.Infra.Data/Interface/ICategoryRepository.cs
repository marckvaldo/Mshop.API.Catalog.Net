using Mshop.Domain.Entity;
using Mshop.Core.Paginated;
using Mshop.Core.Data;

namespace Mshop.Infra.Data.Interface
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category> GetCategoryProducts(Guid id);

        Task<PaginatedOutPut<Category>> FilterPaginated(PaginatedInPut input);

        Task<Category> GetByName(string name);
    }
}
