using Application.Common.Enums;
using Infrastructure.Persistence.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entities
{
    [Table("AppPermission")]
    public class AppPermissionDao : EntityBaseDao<long>
    {
        [MaxLength(50)]
        public AppPermission PermissionCode { get; set; }
        public virtual ICollection<AppActionDao> Actions { get; set; } = [];
    }
}
