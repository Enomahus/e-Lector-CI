using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Common
{
    public class UserCommandValidatorBase<T> : AbstractValidator<T> where T : UserModel
    {
        protected readonly ReadOnlyDbContext _context;

        public UserCommandValidatorBase(ReadOnlyDbContext context, bool validateRoles = true)
        {
            _context = context;

            RuleFor(v => v.FirstName)
               .NotEmpty()
               .WithMessage(ValidationErrorCode.Required.ToString())
               .MaximumLength(50)
               .WithMessage(ValidationErrorCode.MaxLength.ToString());

            RuleFor(v => v.LastName)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .MaximumLength(50)
                .WithMessage(ValidationErrorCode.MaxLength.ToString());

            RuleFor(u => u.PhoneNumber)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .MaximumLength(50)
                .WithMessage(ValidationErrorCode.MaxLength.ToString());

            RuleFor(v => v.Email)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .DependentRules(() =>
                {
                    RuleFor(v => v.Email)
                        .EmailAddress()
                        .WithMessage(ValidationErrorCode.InvalidEmail.ToString());
                    RuleFor(v => v.Email!)
                        .MustAsync(BeUniqueEmailAsync)
                        .WithMessage(ValidationErrorCode.Unique.ToString());
                });

            When(
                v => v.ConstituencyId is null,
                () =>
                {
                    RuleFor(v => v.NewStakeholder)
                        .NotEmpty()
                        .WithMessage(ValidationErrorCode.Required.ToString())
                        .DependentRules(() =>
                        {
                            RuleFor(v => v.NewStakeholder!)
                                .SetValidator(new StakeholderValidatorBase<StakeholderModel>(context));
                        });
                }
            );

            if (validateRoles)
            {
                RuleFor(v => v.Roles)
                    .NotEmpty()
                    .WithMessage(ValidationErrorCode.Required.ToString())
                    .DependentRules(() =>
                    {
                        RuleFor(v => v.Roles)
                            .MustAsync(RolesExistAsync)
                            .WithMessage(ValidationErrorCode.RoleMustExist.ToString());
                    });
            }
        }

        protected virtual Task<bool> BeUniqueEmailAsync(
            T command,
            string email,
            CancellationToken cancellationToken
        )
        {
            return _context.Users.AllAsync(u => u.UserName != email && u.Email != email, cancellationToken);
        }

        private async Task<bool> RolesExistAsync(List<Guid> rolesList, CancellationToken cancellationToken)
        {
            var existingRolesCount = await _context
                .Roles.Where(r => rolesList.Contains(r.Id))
                .CountAsync(cancellationToken);
            return existingRolesCount == rolesList.Count;
        }

        private Task<bool> ConstituencyExistsAsync(long? stakeholderId, CancellationToken cancellationToken)
        {
            return _context.Constituencies.AnyAsync(s => s.Id == stakeholderId, cancellationToken);
        }
    }
}
