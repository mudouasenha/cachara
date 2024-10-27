using Cachara.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cachara.Data.Persistence.Repositories
{
    public class EntityFrameworkRepository<TDbContext, TEntity> : IRepository<TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        protected readonly TDbContext dbContext;

        public EntityFrameworkRepository(TDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        protected virtual IQueryable<TEntity> GetEntitiesBase() => dbContext.Set<TEntity>();

        public ValueTask<TEntity> AddAsync(TEntity entity)
        {
            dbContext.Set<TEntity>().Add(entity);
            return new ValueTask<TEntity>(entity);
        }

        public ValueTask<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities)
        {
            dbContext.Set<TEntity>().AddRange(entities);
            return new ValueTask<IEnumerable<TEntity>>(entities);
        }

        public ValueTask<TEntity> EditAsync(TEntity entity)
        {
            dbContext.Set<TEntity>().Update(entity);
            return new ValueTask<TEntity>(entity);
        }

        public ValueTask<IEnumerable<TEntity>> EditAsync(IEnumerable<TEntity> entities)
        {
            dbContext.Set<TEntity>().UpdateRange(entities);
            return new ValueTask<IEnumerable<TEntity>>(entities);
        }

        public Task RemoveAsync(TEntity entity)
        {
            dbContext.Set<TEntity>().Remove(entity);
            return Task.FromResult(1);
        }

        public Task RemoveAsync(IEnumerable<TEntity> entities)
        {
            dbContext.Set<TEntity>().RemoveRange(entities);
            return Task.FromResult(1);
        }

        public async Task<long> GetCountAsync(IQueryable<TEntity> queryable)
        {
            return await queryable.LongCountAsync();
        }

        public ValueTask<TEntity> FindByAsync(params object[] keyValues)
        {
            return dbContext.Set<TEntity>().FindAsync(keyValues);
        }

        public async ValueTask<TEntity> FindByAsync(Expression<Func<TEntity, bool>> specification)
            => await dbContext.Set<TEntity>().Where(specification).FirstOrDefaultAsync();

        public IQueryable<TEntity> GetEntities(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = GetEntitiesBase();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }
    }
}
