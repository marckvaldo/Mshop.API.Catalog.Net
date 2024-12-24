using Microsoft.EntityFrameworkCore;
using Mshop.Infra.Data.Context;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.IntegrationTests.Infra.Repository.Data.ProductRepository
{
    public class ProductRepositoryPersistence
    {
        private readonly RepositoryDbContext _context;

        public ProductRepositoryPersistence(RepositoryDbContext context)
        {
            _context = context;
        }

        public async Task<DomainEntity.Product?> GetById(Guid id)
        {
            return await _context.
                Products.
                AsNoTracking().
                FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<DomainEntity.Product?>> List()
        {
            return await _context.Products.ToListAsync();
        }

        public async void Create(DomainEntity.Product request)
        {
            await _context.AddAsync(request);
            await _context.SaveChangesAsync();  
        }

        public async Task CreateList(List<DomainEntity.Product> request)
        {
            await _context.AddRangeAsync(request);
            await _context.SaveChangesAsync();
        }
    }
}
