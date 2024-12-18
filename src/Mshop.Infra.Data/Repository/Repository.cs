using Microsoft.EntityFrameworkCore;
using Mshop.Core.Data;
using Mshop.Core.DomainObject;
using Mshop.Core.Paginated;
using Mshop.Infra.Data.Context;
using System.Linq.Expressions;

namespace Mshop.Infra.Data.Repository
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly RepositoryDbContext _db;
        protected readonly DbSet<TEntity> _dbSet;

        protected Repository(RepositoryDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<TEntity>();
        }

        public async Task<List<TEntity>> Filter(Expression<Func<TEntity, bool>> predicate)
        {
            var result = await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
            return result;
        }

        public virtual async Task<TEntity?> GetById(Guid Id)
        {
            var result = await _dbSet.FindAsync(Id);            
            return result;
        }

        public virtual async Task<List<TEntity>> GetValuesList()
        {
            var result =  await _dbSet.ToListAsync();
            return result;
        }

        public virtual async Task<TEntity> GetLastRegister(Expression<Func<TEntity, bool>> predicate)
        {
            var result =  await _dbSet.AsNoTracking().Where(predicate).OrderByDescending(x=>x.Id).FirstAsync();            
            return result;
        }

        public virtual async Task Create(TEntity entity, CancellationToken cancellationToken)
        {
            await _dbSet.AddAsync(entity,cancellationToken);
        }

        public virtual async Task DeleteById(TEntity entity,CancellationToken cancellationToken)
        {
             await Task.FromResult(_dbSet.Remove(entity));
            //await SaveChanges();
        }

        public virtual async Task Update(TEntity entity, CancellationToken cancellation)
        {
            await Task.FromResult(_dbSet.Update(entity));
        }

        public async Task<int> SaveChanges()
        {
            return await _db.SaveChangesAsync();
        }
        
        public virtual Task<PaginatedOutPut<TEntity>> FilterPaginated(PaginatedInPut input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //_db?.Dispose();
        }

    }
}
