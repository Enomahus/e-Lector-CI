using Infrastructure.Persistence.Entities;

namespace Application.Features.Users.Common
{
    public static class UserQueryExtensions
    {
        private static readonly HashSet<string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "lastName",
            "firstName",
            "email",
            "createdAt",
            "isActive",
        };

        public static IQueryable<UserDao> ApplySearch(this IQueryable<UserDao> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            var term = search.Trim();

            return query.Where(u =>
                (u.LastName != null && u.LastName.Contains(term, StringComparison.OrdinalIgnoreCase))
                || (u.FirstName != null && u.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase))
                || (u.Email != null && u.Email.Contains(term, StringComparison.OrdinalIgnoreCase))
                || (u.PhoneNumber != null && u.PhoneNumber.Contains(term, StringComparison.OrdinalIgnoreCase))
            );
        }

        public static IQueryable<UserDao> ApplySort(
            this IQueryable<UserDao> query,
            string? sort,
            string? order
        )
        {
            bool descending = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
            string sortColumn = AllowedSortColumns.Contains(sort ?? "") ? sort!.ToLower() : "lastname";
            return sortColumn switch
            {
                "lastname" => descending
                    ? query.OrderByDescending(u => u.LastName)
                    : query.OrderBy(u => u.LastName),
                "firstname" => descending
                    ? query.OrderByDescending(u => u.FirstName)
                    : query.OrderBy(u => u.FirstName),
                "email" => descending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "createdat" => descending
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt),
                "isactive" => descending
                    ? query.OrderByDescending(u => u.DisabledDate)
                    : query.OrderBy(u => u.DisabledDate),
                _ => query.OrderBy(u => u.LastName), // Tri par défaut
            };
        }
    }
}
