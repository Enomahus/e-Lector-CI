using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence.Entities;

public class RoleDao : IdentityRole<Guid>
{
    public RoleDao(): base() { }

    public RoleDao(string roleName) : base(roleName) { }
    public virtual ICollection<UserConstituencyDao> UserConstituencies { get; set; } = [];
}
