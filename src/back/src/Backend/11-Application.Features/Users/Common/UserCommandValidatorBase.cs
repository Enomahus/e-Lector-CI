using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;
using Tools.Constants;

namespace Application.Features.Users.Common
{
    public class UserCommandValidatorBase<T> : AbstractValidator<T>
        where T : UserModel
    {
        protected readonly ReadOnlyDbContext _context;

        public UserCommandValidatorBase(ReadOnlyDbContext context, bool validateRoles = true)
        {
            _context = context;

            RuleFor(v => v.Civility).NotNull().WithMessage(ValidationErrorCode.Required.ToString());

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

            RuleFor(v => v.EmployeeNumber)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .MustAsync(
                    async (model, employerNumber, token) =>
                    {
                        bool needsEmployeeNumber = await _context.Roles.AnyAsync(
                            r =>
                                model.Roles.Contains(r.Id)
                                && (
                                    r.Name == AppConstants.SuperAdminRole
                                    || r.Name == AppConstants.OrganismAgentRole
                                ),
                            token
                        );

                        if (needsEmployeeNumber)
                        {
                            return !string.IsNullOrWhiteSpace(employerNumber);
                        }

                        return true;
                    }
                );

            RuleFor(v => v.ConstituencyId)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .DependentRules(() =>
                {
                    RuleFor(v => v.ConstituencyId)
                        .MustAsync(ConstituencyExistsAsync)
                        .WithMessage(ValidationErrorCode.ConstituencyMustExist.ToString());
                });

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

        private async Task<bool> ConstituencyExistsAsync(
            long? constituencyId,
            CancellationToken cancellationToken
        )
        {
            return await _context.Constituencies.AnyAsync(c => c.Id == constituencyId, cancellationToken);
        }
    }
}
