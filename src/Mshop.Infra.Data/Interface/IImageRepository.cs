using Mshop.Domain.Entity;
using Mshop.Core.Data;


namespace Mshop.Infra.Data.Interface
{
    public interface IImageRepository : IRepository<Image>
    {
        Task CreateRange(List<Image> images, CancellationToken cancellationToken);

        Task DeleteByIdProduct(Guid productId);

        Task<IEnumerable<Image>> GetImagesByProductId(Guid productId);

    }
}
