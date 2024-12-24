using Microsoft.EntityFrameworkCore;
using Mshop.Infra.Data.Context;
using DomainEntity = Mshop.Domain.Entity;

namespace Mshop.IntegrationTests.Infra.Repository.Data.CategoryRepository
{
    public class CategoryRepositoryPertsistence
    {
        private readonly RepositoryDbContext _context;
        public CategoryRepositoryPertsistence(RepositoryDbContext context) 
        {
            _context = context; 
        }

        public async Task<DomainEntity.Category?> GetCategory(Guid id)
        {
            return await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<IEnumerable<DomainEntity.Category>> GetAllCategories() 
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task Create(DomainEntity.Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task CreateList(IEnumerable<DomainEntity.Category> category)
        {
            await _context.AddRangeAsync(category);
            await _context.SaveChangesAsync();
        }
    }
}
