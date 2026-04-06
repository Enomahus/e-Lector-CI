using Application.Features.Common.Citizen;
using Application.Features.Users.Common;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

            RuleFor(r => r.RegistrationRequest).NotNull();

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

            When(
                r => r.RegistrationRequest!.Citizen != null,
                () => 
                    RuleFor(r => r.RegistrationRequest!.Citizen!)
                        .SetValidator(new CitizenValidatorBase<CitizenModel>(_context,_timeProvider))
            );

            When(
                r => r.RegistrationRequest!.Author != null,
                () => 
                RuleFor(r => r.RegistrationRequest!.Author!)
                    .SetValidator(new UserCommandValidatorBase<UserModel>(_context, true))
            );

            RuleFor(r => r.RegistrationRequestAttachments)
                .Must(p => p.Count <= 10)
                .WithMessage(ValidationErrorCode.TooManyAttachments.ToString());
        }

        protected virtual Task<bool> CheckConstituencyMustExistAsync(long constituencyId, CancellationToken cancellationToken)
        {
            return _context.Constituencies.AnyAsync(c => c.Id == constituencyId, cancellationToken);
        }
    }
}
