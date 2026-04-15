using Application.Common.Enums;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Common.PollingStation
{
    public class PollingStationValidatorBase<T> : AbstractValidator<T> where T : PollingStationModel
    {
        protected readonly ReadOnlyDbContext _context;

        public PollingStationValidatorBase(ReadOnlyDbContext context)
        {
            _context = context;

            RuleFor(v => v.StationNumber)
               .NotEmpty()
               .WithMessage(ValidationErrorCode.Required.ToString());

            RuleFor(x => x.Wording)
            .NotEmpty()
            .WithMessage(ValidationErrorCode.Required.ToString())
            .MaximumLength(100)
            .WithMessage(ValidationErrorCode.MaxLength.ToString());

            RuleFor(x => x.ConstituencyId)
                .NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .DependentRules(() =>
                {
                    RuleFor(x => x)
                        .MustAsync(ConstituencyMustBeVotingLocationAsync)
                        .WithMessage(ValidationErrorCode.InvalidLevel.ToString());
                });
        }

        private async Task<bool> ConstituencyMustBeVotingLocationAsync(
            T model,
            CancellationToken cancellationToken
        )
        {
            if (model.ConstituencyId <= 0)
                return false;

            var constituency = await _context.Constituencies.FirstOrDefaultAsync(
                x => x.Id == model.ConstituencyId,
                cancellationToken
            );

            if (constituency is null)
                return false;

            // Vérifier que la circonscription est de niveau VotingLocation
            return constituency.Level == LocationLevel.VotingLocation;
        }
    }
}
