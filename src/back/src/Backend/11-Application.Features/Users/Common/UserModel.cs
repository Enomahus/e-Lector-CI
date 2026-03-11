using Application.Common.Enums;
using Infrastructure.Persistence.Entities;
using Tools.Constants;

namespace Application.Features.Users.Common
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public PersonTitle Civility { get; set; }
        public bool? IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> Roles { get; set; } = [];

        public static UserModel FromDao(UserDao user, DateTimeOffset now)
        {
            var isAdmin = user.UserRoles?.Any(ur => 
                ur.Role.Name == AppConstants.SuperAdminRole) ?? false;

            var isActive = !user.DisabledDate.HasValue || user.DisabledDate.Value > now;

            var userRoles = user.UserRoles?.Select(ur => ur.Role)
                                .Where(r => r.Name != AppConstants.SuperAdminRole)
                                .Select(r => r.Id)
                                .ToList();

            return new UserModel() 
            { 
                Id = user.Id,
                Civility = user.Civility,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                IsAdmin = isAdmin,
                IsActive = isActive,
                Roles = userRoles ?? []
            };

        }
    }
}
