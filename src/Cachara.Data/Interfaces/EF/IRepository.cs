using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cachara.Data.Interfaces;

public interface IRepository<TEntity> :
        IQueryableRepository<TEntity>,
        ICommandRepository<TEntity>,
        ICountRepository<TEntity>
        where TEntity : class
    {
        ValueTask<TEntity> FindByAsync(Expression<Func<TEntity, bool>> specification);
    }

    public interface IQueryableRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetEntities(params Expression<Func<TEntity, object>>[] includes);
    }

    public interface ICommandRepository<TEntity> where TEntity : class
    {
        ValueTask<TEntity> AddAsync(TEntity entity);

        ValueTask<TEntity> EditAsync(TEntity entity);

        Task RemoveAsync(TEntity entity);
    }

    public interface ICountRepository<TEntity>
        where TEntity : class
    {
        Task<long> GetCountAsync(IQueryable<TEntity> queryable);
    }