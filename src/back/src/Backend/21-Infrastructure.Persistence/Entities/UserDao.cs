using Application.Common.Enums;
using Infrastructure.Persistence.Common;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entities;

public class UserDao : IdentityUser<Guid>, IEntityBaseDao<Guid>, ITimestampedEntity
{
    public PersonTitle Civility { get; set; }
    [MaxLength(50)]
    public string FirstName { get; set; }

    [MaxLength(50)]
    public string LastName { get; set; }

    public string EmployeeNumber { get; set; }

    [MaxLength(20)]
    public AuthProvider? AuthProvider { get; set; }
    public DateTimeOffset? DisabledDate { get; set; }

    public virtual ICollection<RefreshTokenDao> RefreshTokens { get; set; } = [];
    public virtual ICollection<UserRoleDao> UserRoles { get; set; } = [];
    public virtual ICollection<RegistrationRequestDao> CreatedRegistrationRequests { get; set; } = [];
    public virtual ICollection<RegistrationRequestDao> UpdatedRegistrationRequests { get; set; } = [];
    public virtual ICollection<UserConstituencyDao> UserConstituencies { get; set;} = [];

    public DateTimeOffset ModifiedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
