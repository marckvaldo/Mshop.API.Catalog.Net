using Mshop.Core.Cache;
using Mshop.Core.Paginated;
using Mshop.Domain.Entity;

namespace Mshop.Infra.Cache.Interface
{
    public interface IImagesCacheRepository : ICacheRepository<Product>
    {
        Task<bool> AddImage(Image entity, DateTime? ExpirationDate, CancellationToken cancellationToken);

        Task<bool> DeleteImage(Image entity, CancellationToken cancellationToken);

        Task<Image> GetImageById(Guid id);

        Task<IEnumerable<Image>> GetImageByProductId(Guid productId);

        Task<bool> UpadteImage(Image entity, DateTime? ExpirationDate, CancellationToken cancellationToken);

    }

}
