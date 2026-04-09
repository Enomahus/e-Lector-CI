using Application.Common.Enums;
using Application.Features.Security.Common;
using Infrastructure.ExternalAuth;
using Infrastructure.ExternalAuth.Interfaces;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Security.AuthenticateMicrosoft
{
    public class AuthenticateMicrosoftCommand : AuthenticateExternalCommandBase { }

    public class AuthenticateMicrosoftCommandValidator
        : AuthenticateExternalCommandValidatorBase<AuthenticateMicrosoftCommand>
    { }

    public class AuthenticateMicrosoftCommandHandler(
        UserManager<UserDao> userManager,
        ITokenHelper tokenHelper,
        TimeProvider timeProvider,
        [FromKeyedServices(ExternalAuthServiceKeys.MicrosoftAuthService)]
            IExternalAuthService microsoftAuthService,
        ReadOnlyDbContext context
    )
        : AuthenticateExternalCommandHandlerBase<AuthenticateMicrosoftCommand>(
            AuthProvider.Microsoft,
            userManager,
            tokenHelper,
            timeProvider,
            microsoftAuthService,
            context
        )
    { }
}
