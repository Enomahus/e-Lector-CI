using Infrastructure.Persistence.Entities;

namespace Application.Features.RegistrationRequests.Common;

/// <summary>
/// Extensions IQueryable partagées pour les requêtes sur <see cref="RegistrationRequestDao"/>.
/// Centralise le filtrage par recherche textuelle et le tri afin d'éviter la duplication
/// entre les différents handlers de demandes d'inscription.
/// </summary>
public static class RegistrationRequestQueryExtensions
{
    private static readonly HashSet<string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        "reference",
        "submissionDate",
        "status",
        "constituencyName",
        "citizenLastName",
        "citizenFirstName",
        "authorName",
    };

    /// <summary>
    /// Applique un filtre de recherche textuelle sur les champs courants d'une demande d'inscription.
    /// Aucun filtrage n'est appliqué si <paramref name="search"/> est vide ou null.
    /// </summary>
    public static IQueryable<RegistrationRequestDao> ApplySearch(
        this IQueryable<RegistrationRequestDao> query,
        string? search
    )
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;

        var term = search.ToLower();

        return query.Where(r =>
            (r.Reference != null && r.Reference.ToLower().Contains(term))
            || (r.Citizen.FirstName != null && r.Citizen.FirstName.ToLower().Contains(term))
            || (r.Citizen.LastName != null && r.Citizen.LastName.ToLower().Contains(term))
            || (r.Constituency.Wording != null && r.Constituency.Wording.ToLower().Contains(term))
            || (r.Author != null && r.Author.LastName != null && r.Author.LastName.ToLower().Contains(term))
        );
    }

    /// <summary>
    /// Applique le tri sur la requête en fonction des paramètres <paramref name="sort"/> et
    /// <paramref name="order"/>. Seuls les noms de colonnes de la whitelist sont acceptés ;
    /// toute valeur inconnue est ignorée et remplacée par le tri par défaut (submissionDate ASC/DESC).
    /// </summary>
    public static IQueryable<RegistrationRequestDao> ApplySort(
        this IQueryable<RegistrationRequestDao> query,
        string? sort,
        string? order
    )
    {
        bool descending = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
        string sortColumn = AllowedSortColumns.Contains(sort ?? "") ? sort!.ToLower() : "submissiondate";

        return (sortColumn, descending) switch
        {
            ("reference", false) => query.OrderBy(r => r.Reference),
            ("reference", true) => query.OrderByDescending(r => r.Reference),
            ("status", false) => query.OrderBy(r => r.Status),
            ("status", true) => query.OrderByDescending(r => r.Status),
            ("constituencyname", false) => query.OrderBy(r => r.Constituency.Wording),
            ("constituencyname", true) => query.OrderByDescending(r => r.Constituency.Wording),
            ("citizenlastname", false) => query.OrderBy(r => r.Citizen.LastName),
            ("citizenlastname", true) => query.OrderByDescending(r => r.Citizen.LastName),
            ("citizenfirstname", false) => query.OrderBy(r => r.Citizen.FirstName),
            ("citizenfirstname", true) => query.OrderByDescending(r => r.Citizen.FirstName),
            ("authorname", false) => query.OrderBy(r => r.Author!.LastName),
            ("authorname", true) => query.OrderByDescending(r => r.Author!.LastName),
            (_, false) => query.OrderBy(r => r.SubmissionDate),
            (_, true) => query.OrderByDescending(r => r.SubmissionDate),
        };
    }
}
