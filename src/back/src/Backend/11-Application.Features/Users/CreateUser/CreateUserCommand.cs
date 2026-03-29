using Application.Audit;
using Application.Exceptions;
using Application.Features.Users.Common;
using Application.Interfaces.Services;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Web;
using Tools.Configuration;
using Tools.Constants;
using Tools.Logging;

namespace Application.Features.Users.CreateUser
{
    public class CreateUserCommand : UserModel, IRequest<Result<Guid>> { }

    public class CreateUserCommandValidator(): AbstractValidator<CreateUserCommand>
    {
        private readonly ReadOnlyDbContext _context;

        public CreateUserCommandValidator(ReadOnlyDbContext context, IOptions<AppConfiguration> config)
        {
            _context = context;

            RuleFor(v => v.FirstName)
            .NotEmpty()
            .WithMessage(ValidationErrorCode.Required.ToString())
            .MaximumLength(100)
            .WithMessage(ValidationErrorCode.MaxLength.ToString());

            RuleFor(v => v.LastName)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .MaximumLength(100)
                .WithMessage(ValidationErrorCode.MaxLength.ToString());

            RuleFor(v => v.Email)
            .NotEmpty()
            .WithMessage(ValidationErrorCode.Required.ToString())
            .MaximumLength(100)
            .WithMessage(ValidationErrorCode.MaxLength.ToString())
            .DependentRules(() =>
            {
                RuleFor(v => v.Email)
                    .EmailAddress()
                    .WithMessage(ValidationErrorCode.InvalidEmail.ToString())
                    .MustAsync(
                        (userName, cancellationToken) => BeUniqueEmailAsync(userName!, cancellationToken)
                    )
                    .WithMessage(ValidationErrorCode.Unique.ToString());
            });

            RuleFor(v => v.ConstituencyId)
            .NotEmpty()
            .WithMessage(ValidationErrorCode.Required.ToString())
            .DependentRules(() =>
            {
                RuleFor(v => v.ConstituencyId)
                    .MustAsync(
                        (constituencyId, cancellationToken) =>
                            _context.Constituencies.AnyAsync(s => s.Id == constituencyId, cancellationToken)
                    )
                    .WithMessage(ValidationErrorCode.CountryMustExist.ToString());
            });

            RuleFor(v => v.Civility).NotNull().WithMessage(ValidationErrorCode.Required.ToString());

            RuleFor(u => u.PhoneNumber)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .MaximumLength(50)
                .WithMessage(ValidationErrorCode.MaxLength.ToString());
           
        }

        private Task<bool> BeUniqueEmailAsync(string email, CancellationToken cancellationToken)
        {
            return _context.Users.AllAsync(u => u.UserName != email && u.Email != email, cancellationToken);
        }
    }

    public class CreateUserCommandHandler(
    UserManager<UserDao> userManager,
    WritableDbContext context,
    IEmailService emailService,
    IOptions<AppConfiguration> config
) : UserCommandBase(context), IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        private const StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase;

        public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start();

            var stakeholder =
                await _context
                    .Constituencies.Include(s => s.StakeholderActivities)
                    .ThenInclude(sa => sa.Activity)
                    .FirstOrDefaultAsync(a => a.Id == request.ConstituencyId, cancellationToken)
                ?? throw new NotFoundException("Stakeholder", request.ConstituencyId);

            // Entity is validated, we can ignore null warnings
            var userDao = new UserDao()
            {
                UserName = request.Email,
                FirstName = request.FirstName!,
                LastName = request.LastName!,
                Email = request.Email,
                Civility = request.Civility,
                PhoneNumber = request.PhoneNumber,
                
                UserConstituencies = [new() { ConstituencyId = request.ConstituencyId }],
            };

            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteInTransactionAsync(
                async () =>
                {
                    IdentityResult result = await userManager.CreateAsync(userDao);

                    if (!result.Succeeded)
                    {
                        var usernameErrors = result
                            .Errors.Where(e => e.Code.Contains("username", ignoreCase))
                            .ToList();

                        var otherErrors = result
                            .Errors.Where(e => !e.Code.Contains("username", ignoreCase))
                            .ToList();
                        var errors = new List<ValidationFailure>();

                        errors.AddRange(
                            usernameErrors.Select(e => new ValidationFailure(nameof(request.Email), e.Code))
                        );
                        errors.AddRange(otherErrors.Select(e => new ValidationFailure("Other", e.Code)));

                        throw new Exceptions.ValidationException(errors);
                    }

                    //if (stakeholder.SIRET == config.Value.SorenSiret)
                    //{
                    //    await userManager.AddToRoleAsync(userDao, AppConstants.SuperAdminRole);
                    //}
                    //else
                    //{
                    //    await AddUserRolesAndPrincipalActivity(request, userDao, cancellationToken);
                    //    if (request.IsAdmin == true)
                    //    {
                    //        await userManager.AddToRoleAsync(userDao, AppConstants.StakeholderAdminRole);
                    //    }
                    //}

                    var pwdToken = await userManager.GeneratePasswordResetTokenAsync(userDao);
                    var urlEncodedToken = HttpUtility.UrlEncode(pwdToken);
                    var emailEncode = HttpUtility.UrlEncode(userDao.Email);
                    var link = string.Format(
                        AppConstants.ConfirmPasswordResetLink,
                        config.Value.AppUrl,
                        urlEncodedToken,
                        emailEncode
                    );

                    await emailService.ActivateAccountEmail(link, request.Email!);

                    activity.AddParameter(userDao, u => u.Id);
                },
                () => Task.FromResult(true)
            );

            return AuditResult<Guid>.From(userDao.UserName, stakeholder.Name, userDao.Id);
        }
    }
}
