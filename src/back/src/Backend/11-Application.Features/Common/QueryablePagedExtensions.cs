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
        var totalCount = await source.CountAsync(cancellationToken);

        var items = await source
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return new PagedList<TResponse>(items, totalCount);
    }
}
