using System.Web;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Tools.Configuration;
using Tools.Constants;

namespace Application.Features.Users.Common
{
    public class UserCommandHandlerBase(
        WritableDbContext context,
        UserManager<UserDao> userManager,
        IOptions<AppConfiguration> config,
        //IEmailService emailService,
        TimeProvider timeProvider
    )
    {
        protected readonly UserManager<UserDao> _userManager = userManager;
        protected readonly TimeProvider _timeProvider = timeProvider;
        protected readonly WritableDbContext _context = context;

        public async Task MapToDaoAsync(
            UserModel model,
            UserDao dao,
            bool skipAdminFields = false,
            CancellationToken cancellationToken = default
        )
        {
            var dateNow = _timeProvider.GetUtcNow();

            if (dao.UserName == dao.Email)
            {
                dao.UserName = model.Email;
            }
            dao.Email = model.Email;

            dao.FirstName = model.FirstName;
            dao.LastName = model.LastName;
            dao.PhoneNumber = model.PhoneNumber;
            dao.Civility = model.Civility;
            dao.EmployeeNumber = model.EmployeeNumber;
            dao.ModifiedAt = dateNow;

            if (!skipAdminFields)
            {
                var rolesToRemove = dao.UserRoles.Where(ur => !model.Roles.Contains(ur.RoleId)).ToList();
                var roleIdsToAdd = model.Roles.Where(r => !dao.UserRoles.Any(ur => ur.RoleId == r)).ToList();
                foreach (var role in rolesToRemove)
                {
                    dao.UserRoles.Remove(role);
                }
                foreach (var roleId in roleIdsToAdd)
                {
                    dao.UserRoles.Add(new UserRoleDao() { UserId = dao.Id, RoleId = roleId });
                }
                if (!model.IsActive && dao.DisabledDate is null)
                {
                    dao.DisabledDate = dateNow;
                }
                else
                {
                    dao.DisabledDate = null;
                }
                if (dao.CreatedAt == default)
                {
                    dao.CreatedAt = dao.ModifiedAt;
                }
            }

            if (!dao.UserConstituencies.Any(us => us.ConstituencyId == model.ConstituencyId))
            {
                dao.UserConstituencies.Clear();
                dao.UserConstituencies.Add(
                    new UserConstituencyDao() { ConstituencyId = model.ConstituencyId!.Value }
                );
            }
        }

        protected async Task SendCreatePasswordEmailAsync(UserDao userDao)
        {
            var pwdToken = await _userManager.GeneratePasswordResetTokenAsync(userDao);
            var urlEncodedToken = HttpUtility.UrlEncode(pwdToken);
            var urlEncodedEmail = HttpUtility.UrlEncode(userDao.Email);
            var link = string.Format(
                AppConstants.ConfirmPasswordResetLink,
                config.Value.AppUrl,
                urlEncodedToken,
                urlEncodedEmail
            );
            //TODO: implementation du service
            //await emailService.SendCreatePasswordEmailAsync(link, userDao.Email!);
        }
    }
}
