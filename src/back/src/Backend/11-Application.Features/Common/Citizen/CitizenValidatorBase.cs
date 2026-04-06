using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using Application.Common.Enums;

namespace Application.Features.Common.Citizen
{
    public static class CitizenValidationExtensions 
    {
        public static IRuleBuilderOptions<T, string> IsRequiredName<T>(this IRuleBuilder<T,string> ruleBuilder, int maxLength = 50)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .MaximumLength(maxLength)
                .WithMessage(ValidationErrorCode.MaxLength.ToString());
        }

        public static IRuleBuilderOptions<T, DateTimeOffset> IsAdult<T>(this IRuleBuilder<T, DateTimeOffset> ruleBuilder, TimeProvider timeProvider)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationErrorCode.Required.ToString())
                .Must(date => date <= timeProvider.GetUtcNow().AddYears(-18))
                .WithMessage(ValidationErrorCode.InvalidDate.ToString());
        }
    }
    public class CitizenValidatorBase<T> : AbstractValidator<T> where T : CitizenModel
    {
        protected readonly ReadOnlyDbContext _context;
        protected readonly TimeProvider _timeProvider;

        public CitizenValidatorBase(ReadOnlyDbContext context, TimeProvider timeProvider)
        {
            _context = context;
            _timeProvider = timeProvider;

            ApplyBaseRules();
            
            RuleFor(v => v.MarriedName)
                .NotEmpty()
                .When(v => v.MaritalStatus == MaritalStatus.Married)
                .WithMessage(ValidationErrorCode.MarriedNameRequired.ToString());

            RuleFor(v => v.PhysicalAddress)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString());

            RuleFor(v => v.Father)
                .SetValidator(new InternalParentValidator(_timeProvider))
                .When(v => v.Father != null && !v.FatherId.HasValue);

            RuleFor(v => v.Mother)
                .SetValidator(new InternalParentValidator(_timeProvider))
                .When(v => v.Mother != null && !v.MotherId.HasValue);
        }

        protected void ApplyBaseRules()
        {
            RuleFor(v => v.Gender).NotNull().WithMessage(ValidationErrorCode.Required.ToString());
            RuleFor(v => v.FirstName).IsRequiredName(100); 
            RuleFor(v => v.LastName).IsRequiredName();
            RuleFor(v => v.BirthDate).IsAdult(_timeProvider);
            RuleFor(v => v.BirthPlace).NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());
        }

    }

    internal class InternalParentValidator : AbstractValidator<CitizenModel>
    {
        public InternalParentValidator(TimeProvider timeProvider) 
        {
            RuleFor(v => v.Gender).NotNull().WithMessage(ValidationErrorCode.Required.ToString());

            RuleFor(v => v.FirstName).IsRequiredName(100);

            RuleFor(v => v.LastName).IsRequiredName();

            RuleFor(x => x.BirthDate).IsAdult(timeProvider);

            RuleFor(v => v.BirthPlace)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString());
        }        
    }
}
