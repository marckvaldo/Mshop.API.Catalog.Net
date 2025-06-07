using Mshop.Domain.Entity;

namespace Mshop.Domain.Contract.Services
{
    public interface IServiceCacheCategory
    {
        Task BuildCache();

        Task AddCategory(Guid id, CancellationToken cancellationToken);

        Task RemoveCategory(Guid id, CancellationToken cancellationToken);

        Task UpdateCategory(Guid id, CancellationToken cancellationToken);
    }
}
