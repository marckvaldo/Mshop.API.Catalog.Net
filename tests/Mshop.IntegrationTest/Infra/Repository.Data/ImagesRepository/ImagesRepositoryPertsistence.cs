using Microsoft.EntityFrameworkCore;
using Mshop.Infra.Data.Context;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.IntegrationTests.Infra.Repository.Data.ImagesRepository
{
    public class ImagesRepositoryPertsistence
    {
        private readonly RepositoryDbContext _context;
        public ImagesRepositoryPertsistence(RepositoryDbContext context) 
        {
            _context = context; 
        }

        public async Task<DomainEntity.Image?> GetImage(Guid id)
        {
            return await _context.Images.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<IEnumerable<DomainEntity.Image>> GetAllCategories() 
        {
            return await _context.Images.ToListAsync();
        }

        public async Task Create(DomainEntity.Image image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task CreateList(IEnumerable<DomainEntity.Image> image)
        {
            await _context.AddRangeAsync(image);
            await _context.SaveChangesAsync();
        }
    }
}
