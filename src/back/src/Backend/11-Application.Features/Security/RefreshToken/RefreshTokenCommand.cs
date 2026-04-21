using Application.Exceptions.Auth;
using Application.Features.Security.Common;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tools.Helpers;
using Tools.Logging;

namespace Application.Features.Security.RefreshToken
{
    public class RefreshTokenCommand : IRequest<Result<TokenResponse>>
    {
        public string? UserName { get; set; }
        public string? RefreshToken { get; set; }
    }

    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator() 
        {
            RuleFor(u => u.UserName).NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());

            RuleFor(u => u.RefreshToken)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .DependentRules(() =>
                {
                    RuleFor(u => u.RefreshToken)
                        .Must(t => Base64Helper.IsBase64String(t!))
                        .WithMessage(ValidationErrorCode.Base64Format.ToString());
                });

        }
    }

    public class RefreshCommandHandler(
        WritableDbContext context,
        ITokenHelper tokenHelper,
        TimeProvider timeProvider
    ) : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
    {
        public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start();

            var dateNow = timeProvider.GetUtcNow();

            Guid? tokenId = Base64Helper.GetGuidFromBase64(request.RefreshToken!);

            var tokenQuery = context.RefreshTokens.Where(
                rt => rt.Id == tokenId && rt.User != null && rt.User.UserName == request.UserName
            );

            var user = await tokenHelper.GetUserForAuthenticationAsync(request.UserName!);
            var tokenDb = await tokenQuery.SingleOrDefaultAsync(cancellationToken);

            if(tokenDb is null || tokenDb.Expiry < dateNow)
            {
                throw new UserAuthenticationException(request.UserName!);
            }

            context.RefreshTokens.Remove(tokenDb);

            var model = await tokenHelper.GenerateTokenAsync(user,cancellationToken);
            
            return Result<TokenResponse>.From(model);
        }
    }
}
