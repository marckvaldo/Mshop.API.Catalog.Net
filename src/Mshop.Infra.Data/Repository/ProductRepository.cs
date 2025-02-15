using Microsoft.EntityFrameworkCore;
using Mshop.Core.Enum.Paginated;
using Mshop.Core.Paginated;
using Mshop.Domain.Entity;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;


namespace Mshop.Infra.Data.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(RepositoryDbContext db) : base(db)
        {

        }
        public async Task<Product> GetProductWithCategory(Guid id)
        {
            return await _dbSet.Where(p => p.Id == id)
                .Include(c => c.Category)
                .FirstOrDefaultAsync();
        }
        
        public async Task<PaginatedOutPut<Product>> FilterPaginatedQuery(PaginatedInPut input, Guid categoryId, bool onlyPromotion, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = _dbSet.AsNoTracking();

            query = AddOrderToQuery(query, input.OrderBy, input.Order);

            if (!string.IsNullOrWhiteSpace(input.Search))
                query.Where(p => p.Name.Contains(input.Search));

            if(categoryId != Guid.Empty)
                query.Where(p => p.CategoryId == categoryId);

            if (onlyPromotion)
                query.Where(p => p.IsSale == true);

            var total = await query.CountAsync();
            var product = await query.Skip(toSkip).Take(input.PerPage)
                         .Include(c => c.Category).ToListAsync();

            //NotFoundException.ThrowIfnull(product);
            return new PaginatedOutPut<Product>(input.Page, input.PerPage, total, product);

        }
        public async Task<PaginatedOutPut<Product>> FilterPaginatedByCategoryId(PaginatedInPut input, Guid categoryId, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = _dbSet.AsNoTracking();

            query = AddOrderToQuery(query, input.OrderBy, input.Order);

            if (!string.IsNullOrWhiteSpace(input.Search))
                query.Where(p => p.Name.Contains(input.Search));

            query.Where(p=>p.CategoryId == categoryId);

            var total = await query.CountAsync();
            var product = await query.Skip(toSkip).Take(input.PerPage)
                         .Include(c => c.Category).ToListAsync();

            //NotFoundException.ThrowIfnull(product);
            return new PaginatedOutPut<Product>(input.Page, input.PerPage, total, product);

        }
        public async Task<PaginatedOutPut<Product>> FilterPaginatedPromotion(PaginatedInPut input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = _dbSet.AsNoTracking();

            query = AddOrderToQuery(query, input.OrderBy, input.Order);

            if (!string.IsNullOrWhiteSpace(input.Search))
                query.Where(p => p.Name.Contains(input.Search));

            query.Where(p => p.IsSale == true);

            var total = await query.CountAsync();
            var product = await query.Skip(toSkip).Take(input.PerPage)
                         .Include(c => c.Category).ToListAsync();

            //NotFoundException.ThrowIfnull(product);
            return new PaginatedOutPut<Product>(input.Page, input.PerPage, total, product);

        }

        public async Task<List<Product>> GetProductsPromotions()
        {
            var result = await _dbSet.Where(c => c.IsSale == true).Include(c => c.Category).ToListAsync();
            return result;
        }
        public async Task<List<Product>> GetProductsByCategoryId(Guid categoryId)
        {
            return await _dbSet.Where(c=>c.CategoryId == categoryId).ToListAsync();
        }

        public async Task<List<Product>> GetProductAll(bool OnlyActive = false)
        {
            var query = _dbSet.Include(c => c.Category);

            if (OnlyActive)
                query.Where(x => x.IsActive == OnlyActive);
            
            var result = await query.ToListAsync();

            return result;
        }

        private IQueryable<Product> AddOrderToQuery(IQueryable<Product> query, string orderBay, SearchOrder order)
        {
            if (string.IsNullOrWhiteSpace(orderBay))
                return query;
                

            return (orderBay.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name),
                ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name),
                ("id", SearchOrder.Asc) => query.OrderBy(x=>x.Id),
                ("id", SearchOrder.Desc) => query.OrderByDescending(x=>x.Id),
                ("price", SearchOrder.Asc) => query.OrderBy(x=>x.Price),
                ("price", SearchOrder.Desc) => query.OrderByDescending(x=>x.Price),
                _ => query.OrderBy(x => x.Name)
            };
        }

    }
}
