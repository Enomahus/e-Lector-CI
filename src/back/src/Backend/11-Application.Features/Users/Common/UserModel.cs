using Application.Common.Enums;
using Infrastructure.Persistence.Entities;
using Tools.Constants;

namespace Application.Features.Users.Common
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }        
        public PersonTitle Civility { get; set; }
        public bool? IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> Roles { get; set; } = [];
        public long? ConstituencyId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public static UserModel FromDao(UserDao userDao, DateTimeOffset dateNow)
        {
            var model = new UserModel();
            MapDaoToModel(userDao, model, dateNow);
            return model;
        }

        public static void MapDaoToModel(UserDao dao, UserModel model, DateTimeOffset dateNow)
        {
            var isAdmin = dao.UserRoles?.Any(ur => 
                ur.Role.Name == AppConstants.SuperAdminRole) ?? false;

            var isActive = dao.DisabledDate is null || dao.DisabledDate > dateNow;

            model.Id = dao.Id;
            model.Civility = dao.Civility;
            model.FirstName = dao.FirstName;
            model.LastName = dao.LastName;
            model.PhoneNumber = dao.PhoneNumber;
            model.Email = dao.Email;
            model.IsAdmin = isAdmin;
            model.IsActive = isActive;
            model.Roles = [.. dao.UserRoles!.Select(ur => ur.RoleId)];
            model.ConstituencyId = dao.UserConstituencies.FirstOrDefault()?.ConstituencyId;
            model.CreatedAt = dao.CreatedAt;
        }
    }
}
