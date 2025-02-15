using Microsoft.EntityFrameworkCore;
using Mshop.Domain.Entity;
using Mshop.Core.Enum.Paginated;
using Mshop.Core.Exception;
using Mshop.Core.Paginated;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;
using Mshop.Core.DomainObject;
using System.Net.Quic;


namespace Mshop.Infra.Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(RepositoryDbContext db) : base(db)
        {

        }

        public async Task<Category> GetCategoryProducts(Guid id)
        {
            return await _dbSet.Where(c => c.Id == id).Include(c => c.Products).FirstOrDefaultAsync();
        }

        public async Task<PaginatedOutPut<Category>> FilterPaginated(PaginatedInPut input)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = _dbSet.AsNoTracking();

            query = AddOrderToQuery(query, input.OrderBy, input.Order);

            if (!string.IsNullOrWhiteSpace(input.Search))
                query.Where(p => p.Name.Contains(input.Search));

            var total = await query.CountAsync();
            var category = await query.Skip(toSkip).Take(input.PerPage)
                         .ToListAsync();

            NotFoundException.ThrowIfnull(category);
            return new PaginatedOutPut<Category>(input.Page, input.PerPage, total, category);
        }

        private IQueryable<Category> AddOrderToQuery(IQueryable<Category> query, string orderBay, SearchOrder order)
        {
            if (string.IsNullOrWhiteSpace(orderBay))
                return query;


            return (orderBay.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name),
                ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name),
                ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
                ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
                _ => query.OrderBy(x => x.Name)
            };
        }

        public async Task<Category> GetByName(string name)
        {
            return await _dbSet.Where(c=>c.Name == name).FirstOrDefaultAsync();
        }

        public override async Task<PaginatedOutPut<Category>> FilterPaginated(PaginatedInPut input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;

            var query = _dbSet.AsNoTracking();
            query = AddOrderByToQuery(query, input.OrderBy, input.Order);

            if(!string.IsNullOrEmpty(input.Search))
                query = query.Where(c => c.Name.StartsWith(input.Search));
            
            var total = await query.CountAsync(cancellationToken);

            var category = await query.Skip(toSkip).Take(input.PerPage)
                .ToListAsync();

            return new PaginatedOutPut<Category>(input.Page, input.PerPage, total, category);

        }

        private IQueryable<Category> AddOrderByToQuery(IQueryable<Category> query, string orderBy, SearchOrder order)
        {
            if (string.IsNullOrEmpty(orderBy))
                return query;

            return (orderBy.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name),
                ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name),
                ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
                ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
                _ => query.OrderBy(x => x.Name)
            };
        }

    }
}
