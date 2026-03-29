using Application.Audit;
using Application.Exceptions.Auth;
using Application.Features.Security.Common;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pcea.Core.Net.AuditTrail.Attributes;
using Tools.Logging;
using Tools.Serialization;

namespace Application.Features.Security.Authenticate
{
    [AuditParameters(
        Category = nameof(AuditCategory.Security),
        Action = nameof(AuditAction.SuccesfullyAuthenticated)
    )]
    public class AuthenticateCommand : IRequest<Result<TokenResponse>>
    {
        public string? UserName { get; set; }
        [SensitiveData]
        public string? Password { get; set; }
    }

    public class AuthenticateCommandValidator : AbstractValidator<AuthenticateCommand>
    {
        public AuthenticateCommandValidator(ReadOnlyDbContext context) 
        {
            RuleFor(u => u.UserName).NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString());
            RuleFor(u => u.Password).NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString());
        }
    }

    public class AuthenticateCommandHandler(UserManager<UserDao> userManager, ITokenHelper tokenHelper, TimeProvider timeProvider)
        : IRequestHandler<AuthenticateCommand, Result<TokenResponse>>
    {
        public async Task<Result<TokenResponse>> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var dateNow = timeProvider.GetUtcNow();

            UserDao user = await tokenHelper.GetUserForAuthenticationAsync(request.UserName!);

            var passwordValid = await userManager.CheckPasswordAsync(user, request.Password!);
            var activeUser = user.DisabledDate == null || user.DisabledDate > dateNow;


            var valid = passwordValid && activeUser;
            if (!valid)
            {
                throw new UserAuthenticationException(request.UserName!);
            }

            TokenResponse model = await tokenHelper.GenerateTokenAsync(user, cancellationToken);
            return AuditResult<TokenResponse>.From(user.UserName, model);
        }
    }
}
