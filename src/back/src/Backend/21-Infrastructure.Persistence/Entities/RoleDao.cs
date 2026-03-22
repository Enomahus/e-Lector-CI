using Microsoft.AspNetCore.Identity;
using Pcea.Core.Net.Authorization.Persistence.Interfaces.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entities;

public class RoleDao : IdentityRole<Guid>, IRole
{
    public RoleDao(): base() { }

    public RoleDao(string roleName) : base(roleName) { }

    public virtual ICollection<AppActionDao> Actions { get; set; } = [];
    public virtual ICollection<UserConstituencyDao> UserConstituencies { get; set; } = [];

    [NotMapped]
    public ICollection<IAction> RoleActions
    {
        get => (ICollection<IAction>)Actions;
        set => Actions = (ICollection <AppActionDao>) value;
    }
}
