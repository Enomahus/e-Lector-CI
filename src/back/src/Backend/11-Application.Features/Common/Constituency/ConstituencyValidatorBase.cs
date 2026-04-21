using Application.Common.Enums;
using Application.Features.Constituency.CreateConstituency;
using Application.Features.Constituency.UpdateConstituency;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Common.Constituency
{
    public class ConstituencyValidatorBase<T> : AbstractValidator<T> where T : ConstituencyModel
    {
        protected readonly ReadOnlyDbContext _context;

        public ConstituencyValidatorBase(ReadOnlyDbContext context)
        {
            _context = context;

            //RuleFor(v => v.Code)
            //   .NotEmpty()
            //   .WithMessage(ValidationErrorCode.Required.ToString());

            RuleFor(x => x.Wording)
            .NotEmpty()
            .WithMessage(ValidationErrorCode.Required.ToString())
            .MaximumLength(50)
            .WithMessage(ValidationErrorCode.MaxLength.ToString())
            .MustAsync(BeUniqueNameInParentAsync)
            .WithMessage(ValidationErrorCode.AlreadyExists.ToString());
            

            When(
                x => x.Level == LocationLevel.Region,
                () =>
                {
                    RuleFor(x => x.ParentId)
                        .Must(parentId => !parentId.HasValue)
                        .WithMessage(ValidationErrorCode.InvalidParent.ToString());
                }
            );

            When(
                x => x.Level != LocationLevel.Region,
                () =>
                {
                    RuleFor(x => x.ParentId)
                        .NotNull()
                        .WithMessage(ValidationErrorCode.ConstituencyMustHaveParent.ToString())
                        .DependentRules(() =>
                        {
                            RuleFor(x => x)
                                .MustAsync(ParentHasCorrectLevelAsync)
                                .WithMessage(ValidationErrorCode.InvalidLevel.ToString());
                        });
                }
            );



        }        

        private async Task<bool> BeUniqueNameInParentAsync(
            T command,
            string name,
            CancellationToken cancellationToken
        )
        {
            return !await _context.Constituencies.AnyAsync(
                x => x.Wording == name && x.ParentId == command.ParentId,
                cancellationToken
            );
        }

        private async Task<bool> ParentHasCorrectLevelAsync(
            T command,
            CancellationToken cancellationToken
        )
        {
            if (command.ParentId is null)
                return false;

            var parent = await _context.Constituencies.FirstOrDefaultAsync(
                x => x.Id == command.ParentId.Value,
                cancellationToken
            );
            if (parent is null)
                return false;

            var expectedParentLevel = (LocationLevel)((int)command.Level - 1);
            return parent.Level == expectedParentLevel;
        }
    }
}
