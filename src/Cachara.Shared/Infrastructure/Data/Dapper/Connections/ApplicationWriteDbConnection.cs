using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Cachara.Shared.Infrastructure.Data.Dapper.Connections;

public class ApplicationWriteDbConnection<TDbContext> : IApplicationWriteDbConnection
    where TDbContext : DbContext
{
    private readonly TDbContext context;

    public ApplicationWriteDbConnection(TDbContext context)
    {
        this.context = context;
    }

    public async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        return await context.Database.GetDbConnection().ExecuteAsync(sql, param, transaction);
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null,
        IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return (await context.Database.GetDbConnection().QueryAsync<T>(sql, param, transaction)).AsList();
    }

    public async Task<IEnumerable<TResult>> QueryMapAsync<T1, T2, TResult>(string sql, Func<T1, T2, TResult> map,
        object? param = null, IDbTransaction? transaction = null, string splitOn = "Id",
        CancellationToken cancellationToken = default)
    {
        return await context.Database.GetDbConnection().QueryAsync(sql, map, param, transaction, true, splitOn);
    }

    public async Task<IEnumerable<TResult>> QueryMapAsync<T1, T2, T3, TResult>(string sql,
        Func<T1, T2, T3, TResult> map, object? param = null, IDbTransaction? transaction = null, string splitOn = "Id",
        CancellationToken cancellationToken = default)
    {
        return await context.Database.GetDbConnection().QueryAsync(sql, map, param, transaction, true, splitOn);
    }

    public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null,
        IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await context.Database.GetDbConnection().QueryFirstOrDefaultAsync<T>(sql, param, transaction);
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        return await context.Database.GetDbConnection().QuerySingleAsync<T>(sql, param, transaction);
    }
}
