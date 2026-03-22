using Application.Common.Enums;
using Infrastructure.Persistence.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entities
{
    [Table("AppAction")]
    public class AppActionDao : EntityBaseDao<long>
    {
        [MaxLength(50)]
        public AppAction ActionCode { get; set; }
        public virtual ICollection<RoleDao> Roles { get; set; } = [];
        public virtual ICollection<AppPermissionDao> Permissions { get; set; } = [];
    }
}
