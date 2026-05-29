using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Common;

public static class QueryablePagedExtensions
{
    /// <summary>
    /// Applique la pagination sur la requête source, compte le total, projette les résultats
    /// via le sélecteur et retourne un <see cref="PagedList{TResponse}"/>.
    /// Le COUNT est exécuté sur la requête DAO (avant projection) pour des performances optimales.
    /// </summary>
    public static async Task<PagedList<TResponse>> ToPagedListAsync<TSource, TResponse>(
        this IQueryable<TSource> source,
        int pageIndex,
        int pageSize,
        Expression<Func<TSource, TResponse>> selector,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentOutOfRangeException.ThrowIfNegative(pageIndex);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        cancellationToken.ThrowIfCancellationRequested();

        var totalCount = await source.CountAsync();

        if (totalCount == 0)
        {
            return new PagedList<TResponse>([], 0);
        }

        var items = await source
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedList<TResponse>(items, totalCount);
    }

    public static async Task<(List<TResponse> Items, int TotalCount)> ToPagedResultAsync<TSource, TResponse>(
        this IQueryable<TSource> query,
        int pageIndex,
        int pageSize,
        Expression<Func<TSource, TResponse>> selector,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageIndex, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pageSize, 0);

        var totalCount = query.Count();

        if (totalCount == 0 || pageIndex * pageSize >= totalCount)
        {
            return ([], totalCount);
        }

        var items = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
