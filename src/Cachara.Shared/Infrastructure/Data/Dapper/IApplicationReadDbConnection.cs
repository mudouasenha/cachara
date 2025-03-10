﻿using System.Data;

namespace Cachara.Shared.Infrastructure.Data.Dapper;

public interface IApplicationReadDbConnection
{
    Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TResult>> QueryMapAsync<T1, T2, TResult>(string sql, Func<T1, T2, TResult> map,
        object? param = null, IDbTransaction? transaction = null, string splitOn = "Id",
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TResult>> QueryMapAsync<T1, T2, T3, TResult>(string sql, Func<T1, T2, T3, TResult> map,
        object? param = null, IDbTransaction? transaction = null, string splitOn = "Id",
        CancellationToken cancellationToken = default);

    Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);
}
