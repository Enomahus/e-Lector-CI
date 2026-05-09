using Infrastructure.Persistence.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entities
{
    [Table("UserConstituency")]
    public class UserConstituencyDao : EntityBaseDao<Guid>
    {
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserDao User { get; set; }
        public long ConstituencyId { get; set; }
        [ForeignKey(nameof(ConstituencyId))]
        public ConstituencyDao Constituency { get; set; }
        public virtual ICollection<RoleDao> SpecificRoles { get; set; } = [];
    }
}
