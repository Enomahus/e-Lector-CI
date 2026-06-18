using Application.Features.Common.Citizen;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.RegistrationRequests.Common
{
    public class RegistrationRequestValidationBase<T_Command> : AbstractValidator<T_Command>
        where T_Command : RegistrationRequestCommandBase
    {
        protected readonly ReadOnlyDbContext _context;
        protected readonly TimeProvider _timeProvider;

        public RegistrationRequestValidationBase(ReadOnlyDbContext context, TimeProvider timeProvider)
        {
            _context = context;
            _timeProvider = timeProvider;

            RuleFor(r => r.RegistrationRequest)
                .NotNull()
                .WithMessage(ValidationErrorCode.Required.ToString());

            When(
                r => r.RegistrationRequest != null,
                () =>
                {
                    RuleFor(r => r.RegistrationRequest!.ConstituencyId)
                        .NotNull()
                        .WithMessage(ValidationErrorCode.Required.ToString())
                        .DependentRules(() =>
                        {
                            RuleFor(r => r.RegistrationRequest!.ConstituencyId)
                                .MustAsync(
                                    (constituencyId, token) =>
                                    {
                                        return CheckConstituencyMustExistAsync(constituencyId!.Value, token);
                                    }
                                )
                                .WithMessage(ValidationErrorCode.ConstituencyMustExist.ToString());
                        });

                    // Validation imbriquée du citoyen
                    When(
                        r => r.RegistrationRequest!.Citizen != null,
                        () =>
                            RuleFor(r => r.RegistrationRequest!.Citizen!)
                                .SetValidator(new CitizenValidatorBase<CitizenModel>(_context, _timeProvider))
                    );
                }
            );

            RuleFor(r => r.RegistrationRequestCniOrCertificateAttachments)
                .NotNull()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .DependentRules(() =>
                {
                    RuleFor(r => r.RegistrationRequestCniOrCertificateAttachments)
                        .Must(file => file!.Length > 0)
                        .WithMessage(ValidationErrorCode.Required.ToString());
                });

            RuleFor(r => r.Photo).NotNull().WithMessage(ValidationErrorCode.Required.ToString());
        }

        protected virtual Task<bool> CheckConstituencyMustExistAsync(
            long constituencyId,
            CancellationToken cancellationToken
        )
        {
            return _context.Constituencies.AnyAsync(c => c.Id == constituencyId, cancellationToken);
        }
    }
}
