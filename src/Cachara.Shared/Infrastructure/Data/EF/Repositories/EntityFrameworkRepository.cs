using System.Linq.Expressions;
using Cachara.Shared.Infrastructure.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cachara.Shared.Infrastructure.Data.EF.Repositories;

public class EntityFrameworkRepository<TDbContext, TEntity> : IRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    protected readonly TDbContext dbContext;

    public EntityFrameworkRepository(TDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public ValueTask<TEntity> AddAsync(TEntity entity)
    {
        dbContext.Set<TEntity>().Add(entity);
        return new ValueTask<TEntity>(entity);
    }

    public ValueTask<TEntity> EditAsync(TEntity entity)
    {
        dbContext.Set<TEntity>().Update(entity);
        return new ValueTask<TEntity>(entity);
    }

    public Task RemoveAsync(TEntity entity)
    {
        dbContext.Set<TEntity>().Remove(entity);
        return Task.FromResult(1);
    }

    public async Task<long> GetCountAsync(IQueryable<TEntity> queryable)
    {
        return await queryable.LongCountAsync();
    }

    public async ValueTask<TEntity> FindByAsync(Expression<Func<TEntity, bool>> specification)
    {
        return await dbContext.Set<TEntity>().Where(specification).FirstOrDefaultAsync();
    }

    public IQueryable<TEntity> GetEntities(params Expression<Func<TEntity, object>>[] includes)
    {
        var query = GetEntitiesBase();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }

    protected virtual IQueryable<TEntity> GetEntitiesBase()
    {
        return dbContext.Set<TEntity>();
    }

    public ValueTask<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities)
    {
        dbContext.Set<TEntity>().AddRange(entities);
        return new ValueTask<IEnumerable<TEntity>>(entities);
    }

    public ValueTask<IEnumerable<TEntity>> EditAsync(IEnumerable<TEntity> entities)
    {
        dbContext.Set<TEntity>().UpdateRange(entities);
        return new ValueTask<IEnumerable<TEntity>>(entities);
    }

    public Task RemoveAsync(IEnumerable<TEntity> entities)
    {
        dbContext.Set<TEntity>().RemoveRange(entities);
        return Task.FromResult(1);
    }

    public ValueTask<TEntity> FindByAsync(params object[] keyValues)
    {
        return dbContext.Set<TEntity>().FindAsync(keyValues);
    }
}
