using Application.Common.Enums;
using Application.Features.Security.Common;
using Infrastructure.ExternalAuth;
using Infrastructure.ExternalAuth.Interfaces;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Features.Security.AuthenticateGoogle
{
    public class AuthenticateGoogleCommand : AuthenticateExternalCommandBase {}

    public class AuthenticateGoogleCommandValidator
        : AuthenticateExternalCommandValidatorBase<AuthenticateGoogleCommand>
    { }

    public class AuthenticateGoogleCommandHandler(
        UserManager<UserDao> userManager,
        ITokenHelper tokenHelper,
        TimeProvider timeProvider,
        [FromKeyedServices(ExternalAuthServiceKeys.GoogleAuthService)] IExternalAuthService googleAuthService,
        ReadOnlyDbContext context
    )
        : AuthenticateExternalCommandHandlerBase<AuthenticateGoogleCommand>(
            AuthProvider.Google,
            userManager,
            tokenHelper,
            timeProvider,
            googleAuthService,
            context
        )
    { }
}
