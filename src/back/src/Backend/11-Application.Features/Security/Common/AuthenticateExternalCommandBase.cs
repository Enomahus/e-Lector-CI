using Application.Common.Enums;
using Application.Exceptions.Auth;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.ExternalAuth.Interfaces;
using Infrastructure.ExternalAuth.Models;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tools.Constants;
using Tools.Logging;

namespace Application.Features.Security.Common
{
    public abstract class AuthenticateExternalCommandBase : IRequest<Result<TokenResponse>>
    {
        public string? AuthCode { get; set; }
    }

    public abstract class AuthenticateExternalCommandValidatorBase<T> : AbstractValidator<T>
        where T : AuthenticateExternalCommandBase
    {
        protected AuthenticateExternalCommandValidatorBase()
        {
            RuleFor(v => v.AuthCode).NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());
        }
    }

    public abstract class AuthenticateExternalCommandHandlerBase<T>(
        AuthProvider provider,
        UserManager<UserDao> userManager,
        ITokenHelper tokenHelper,
        TimeProvider timeProvider,
        IExternalAuthService authService,
        ReadOnlyDbContext context
    ): IRequestHandler<T, Result<TokenResponse>>
        where T : AuthenticateExternalCommandBase
    {
        public async Task<Result<TokenResponse>> Handle(T request, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            ExternallyAuthenticatedPersonModel extUser;

            try
            {
                extUser = await authService.CheckAuthorizationCodeAsync(request.AuthCode!, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new UserAuthenticationException("", ex);
            }

            var userExists = await context.Users.AnyAsync(u => u.Email == extUser.Email, cancellationToken);

            if (!userExists)
            {
                await CreateExternallyAuthenticatedUserAsync(extUser, cancellationToken);
            }

            UserDao user = await tokenHelper.GetUserForAuthenticationAsync(extUser.Email!);

            if (user.DisabledDate <= timeProvider.GetUtcNow() || user.AuthProvider != provider)
            {
                throw new UserAuthenticationException(user.UserName!);
            }

            TokenResponse model = await tokenHelper.GenerateTokenAsync(user, cancellationToken);
            return Result<TokenResponse>.From(model);
        }

        private async Task CreateExternallyAuthenticatedUserAsync(
            ExternallyAuthenticatedPersonModel extUser,
            CancellationToken cancellationToken
        )
        {
            var newUser = new UserDao()
            {
                Email = extUser.Email,
                UserName = extUser.Email,
                FirstName = extUser.FirstName,
                LastName = extUser.LastName,
                PhoneNumber = extUser.Phone,
                AuthProvider = extUser.AuthProvider,
            };
            newUser.CreatedAt = newUser.ModifiedAt = timeProvider.GetUtcNow();

            var defaultRoleId = await context
                .Roles.Where(r => r.Name == AppConstants.ElectorRole)
                .Select(r => r.Id)
                .FirstAsync(cancellationToken);
            newUser.UserRoles.Add(new UserRoleDao() { RoleId = defaultRoleId });

            await userManager.CreateAsync(newUser);
        }
    }

}
