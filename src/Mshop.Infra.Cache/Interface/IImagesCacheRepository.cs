using Mshop.Core.Cache;
using Mshop.Core.Paginated;
using Mshop.Domain.Entity;

namespace Mshop.Infra.Cache.Interface
{
    public interface IImagesCacheRepository : ICacheRepository<Image>
    {
        Task<IEnumerable<Image>> GetImageByProductId(Guid productId);
    }

}
