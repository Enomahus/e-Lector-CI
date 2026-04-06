using Application.Common.Enums;
using Application.Features.Users.Common;
using Infrastructure.Persistence.Entities;

namespace Application.Features.Users.GetCurrentUser
{
    public class GetCurrentUserResponse : UserModel
    {
        public Guid UserId { get; private set; }
        public List<AppPermission> Permissions { get; private set; } = [];

        public static GetCurrentUserResponse FromDao(
            UserDao userDao, 
            List<AppPermission> permissions, 
            DateTimeOffset dateNow
        )
        {
            var result = new GetCurrentUserResponse();
            UserModel.MapDaoToModel(userDao,result,dateNow);
            result.UserId = userDao.Id;
            result.Permissions = permissions;
            return result;
        }
    }
}
