using Infrastructure.Persistence.Entities;

namespace Application.Features.PollingStation.Common;

/// <summary>
/// Extensions IQueryable partagées pour les requêtes sur <see cref="PollingStationDao"/>.
/// Centralise le filtrage par recherche textuelle et le tri afin d'éviter la duplication
/// entre les différents handlers de bureaux de vote.
/// </summary>
public static class PollingStationQueryExtensions
{
    private static readonly HashSet<string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        "stationNumber",
        "votingLocationName",
        "municipalityName",
        "subPrefectureName",
        "departmentName",
        "regionName",
    };

    /// <summary>
    /// Applique un filtre de recherche textuelle sur les champs courants d'un bureau de vote.
    /// Aucun filtrage n'est appliqué si <paramref name="search"/> est vide ou null.
    /// </summary>
    public static IQueryable<PollingStationDao> ApplySearch(
        this IQueryable<PollingStationDao> query,
        string? search
    )
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;

        var term = search.ToLower();

        return query.Where(ps =>
            ps.StationNumber.ToLower().Contains(term)
            || (ps.Constituency.Wording != null && ps.Constituency.Wording.ToLower().Contains(term))
            || (ps.Constituency.Parent != null && ps.Constituency.Parent.Wording.ToLower().Contains(term))
            || (
                ps.Constituency.Parent.Parent != null
                && ps.Constituency.Parent.Parent.Wording.ToLower().Contains(term)
            )
            || (
                ps.Constituency.Parent.Parent.Parent != null
                && ps.Constituency.Parent.Parent.Parent.Wording.ToLower().Contains(term)
            )
            || (
                ps.Constituency.Parent.Parent.Parent.Parent != null
                && ps.Constituency.Parent.Parent.Parent.Parent.Wording.ToLower().Contains(term)
            )
        );
    }

    /// <summary>
    /// Applique le tri sur la requête en fonction des paramètres <paramref name="sort"/> et
    /// <paramref name="order"/>. Seuls les noms de colonnes de la whitelist sont acceptés ;
    /// toute valeur inconnue est ignorée et remplacée par le tri par défaut (stationNumber ASC/DESC).
    /// </summary>
    public static IQueryable<PollingStationDao> ApplySort(
        this IQueryable<PollingStationDao> query,
        string? sort,
        string? order
    )
    {
        bool descending = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
        string sortColumn = AllowedSortColumns.Contains(sort ?? "") ? sort!.ToLower() : "stationnumber";

        return (sortColumn, descending) switch
        {
            ("votinglocationname", false) => query.OrderBy(ps => ps.Constituency.Wording),
            ("votinglocationname", true) => query.OrderByDescending(ps => ps.Constituency.Wording),
            ("municipalityname", false) => query.OrderBy(ps => ps.Constituency.Parent.Wording),
            ("municipalityname", true) => query.OrderByDescending(ps => ps.Constituency.Parent.Wording),
            ("subprefecturename", false) => query.OrderBy(ps => ps.Constituency.Parent.Parent.Wording),
            ("subprefecturename", true) => query.OrderByDescending(ps =>
                ps.Constituency.Parent.Parent.Wording
            ),
            ("departmentname", false) => query.OrderBy(ps => ps.Constituency.Parent.Parent.Parent.Wording),
            ("departmentname", true) => query.OrderByDescending(ps =>
                ps.Constituency.Parent.Parent.Parent.Wording
            ),
            ("regionname", false) => query.OrderBy(ps => ps.Constituency.Parent.Parent.Parent.Parent.Wording),
            ("regionname", true) => query.OrderByDescending(ps =>
                ps.Constituency.Parent.Parent.Parent.Parent.Wording
            ),
            (_, false) => query.OrderBy(ps => ps.StationNumber),
            (_, true) => query.OrderByDescending(ps => ps.StationNumber),
        };
    }
}
