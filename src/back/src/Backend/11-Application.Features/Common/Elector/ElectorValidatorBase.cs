using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Common.Elector
{
    public class ElectorValidatorBase : AbstractValidator<ElectorModel>
    {
        protected readonly ReadOnlyDbContext _context;

        public ElectorValidatorBase(ReadOnlyDbContext context)
        {
            _context = context;

            RuleFor(v => v.VoterNumber).NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString());

            RuleFor(v => v.FirstNames).MaximumLength(150).WithMessage(ValidationErrorCode.MaxLength.ToString());

            RuleFor(v => v.LastName).MaximumLength(100).WithMessage(ValidationErrorCode.MaxLength.ToString());

            RuleFor(v => v.MarriedName).MaximumLength(50).WithMessage(ValidationErrorCode.MaxLength.ToString());

            RuleFor(v => v.DateOfBirth)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .DependentRules(() =>
                {
                    RuleFor(x => x.DateOfBirth)
                    .Must(BeOver18)
                    .WithMessage(ValidationErrorCode.InvalidBirthDate.ToString());
                });

            RuleFor(v => v.PlaceOfBirth).MaximumLength(100).WithMessage(ValidationErrorCode.MaxLength.ToString());
            
            RuleFor(v => v.PhysicalAddress).NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString());

            RuleFor(v => v.PollingStationId)
                .NotNull()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .DependentRules(() =>
                {
                    RuleFor(p => p)
                    .MustAsync(CheckPollingStationMustExistAsync)
                    .WithMessage(ValidationErrorCode.PollingStationMustExist.ToString());
                });
        }

        private static bool BeOver18(DateTime birthDate)
        {           
            return birthDate <= DateTime.Today.AddYears(-18);            
        }

        private async Task<bool> CheckPollingStationMustExistAsync(ElectorModel model, CancellationToken token)
        {
            return await _context.PollingStations.AnyAsync(p => p.Id == model.PollingStationId, token);
        }
    }
}
