using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Security.Common
{
    public static class TokenHelperInclude
    {
        public static IQueryable<UserDao> IncludeUser(this IQueryable<UserDao> query)
        {
            return query
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.Actions)
                .ThenInclude(a => a.Permissions)
                .Include(u => u.UserConstituencies)
                .ThenInclude(ue => ue.SpecificRoles)
                .ThenInclude(r => r.Actions)
                .ThenInclude(a => a.Permissions);                
        }
    }
}
