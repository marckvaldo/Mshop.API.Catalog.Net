using Microsoft.EntityFrameworkCore;
using Mshop.Domain.Entity;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;

namespace Mshop.Infra.Data.Repository
{
    public class ImagesRepository : Repository<Image>, IImageRepository
    {
        public ImagesRepository(RepositoryDbContext db) : base(db)
        {

        }

        public async Task CreateRange(List<Image> images, CancellationToken cancellationToken)
        {
            await _dbSet.AddRangeAsync(images,cancellationToken);
        }

        public async Task DeleteByIdProduct(Guid productId)
        {
            var images =  _dbSet.Where(x=>x.ProductId == productId).ToList();
            foreach (Image image in images)
             _dbSet.Remove(image);

            await SaveChanges();
        }

        public async Task<IEnumerable<Image>> GetImagesByProductId(Guid productId)
        {
            return await _dbSet.Where(x => x.ProductId == productId).ToListAsync();
        }

    }
}
