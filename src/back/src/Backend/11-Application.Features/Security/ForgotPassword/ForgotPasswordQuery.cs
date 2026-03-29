using Application.Audit;
using Application.Exceptions;
using Application.Interfaces.Services;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pcea.Core.Net.AuditTrail.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Tools.Configuration;
using Tools.Constants;
using Tools.Logging;

namespace Application.Features.Security.ForgotPassword
{
    [AuditParameters(
        Category = nameof(AuditCategory.Security),
        Action = nameof(AuditAction.ForgotPasswordMailSent)
    )]
    public class ForgotPasswordQuery : IRequest<Result>
    {
        public string? UserEmail { get; set; }
    }

    public class ForgotPasswordQueryValidator : AbstractValidator<ForgotPasswordQuery>
    {
        public ForgotPasswordQueryValidator()
        {
            RuleFor(c => c.UserEmail)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .EmailAddress()
                .WithMessage(ValidationErrorCode.InvalidEmail.ToString());
        }
    }

    public class ForgotPasswordQueryHandler(
        UserManager<UserDao> userManager,
        ReadOnlyDbContext context,
        IEmailService emailService,
        IOptions<AppConfiguration> config
    ) : IRequestHandler<ForgotPasswordQuery, Result>
    {
        public async Task<Result> Handle(ForgotPasswordQuery request, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start().AddParameter(request, r => r.UserEmail);

            try
            {
                var user =
                    await context.Users.FirstOrDefaultAsync(
                        u => u.Email == request.UserEmail,
                        cancellationToken
                    ) ?? throw new NotFoundException(nameof(UserDao), request.UserEmail);

                var pwdToken = await userManager.GeneratePasswordResetTokenAsync(user);
                var urlEncodedToken = HttpUtility.UrlEncode(pwdToken);
                var urlEncodedEmail = HttpUtility.UrlEncode(user.Email);
                var link = string.Format(
                    AppConstants.ConfirmPasswordResetLink,
                    config.Value.AppUrl,
                    urlEncodedToken,
                    urlEncodedEmail
                );

                await emailService.SendResetPasswordEmail(link, user.Email!);
                return AuditResult.From(user.UserName);
            }
            catch (Exception)
            {
                return Result.Default();
            }
        }
    }
}
