using System.Data;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Common.Elector
{
    public static class ElectorValidationExtensions
    {
        public static IRuleBuilderOptions<T, Guid> IsValidCitizen<T>(this IRuleBuilder<T, Guid> ruleBuilder)
        {
            return ruleBuilder.NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());
        }

        public static IRuleBuilderOptions<T, long> IsValidPollingStation<T>(
            this IRuleBuilder<T, long> ruleBuilder
        )
        {
            return ruleBuilder.NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());
        }
    }

    public class ElectorValidatorBase : AbstractValidator<ElectorModel>
    {
        protected readonly ReadOnlyDbContext _context;
        protected readonly TimeProvider _timeProvider;

        public ElectorValidatorBase(ReadOnlyDbContext context, TimeProvider timeProvider)
        {
            _context = context;
            _timeProvider = timeProvider;

            RuleFor(x => x.CitizenId).IsValidCitizen();

            RuleFor(x => x.PollingStationId).IsValidPollingStation();

            RuleFor(x => x.CitizenId)
                .MustAsync(BeUniqueElectorAsync)
                .WithMessage(ValidationErrorCode.AlreadyRegisteredAsElector.ToString());

            RuleFor(x => x.PollingStationId)
                .MustAsync(BeActivePollingStationAsync)
                .WithMessage(ValidationErrorCode.PollingStationMustExist.ToString());
        }

        private async Task<bool> BeUniqueElectorAsync(Guid citizenId, CancellationToken ct)
        {
            return !await _context.Electors.AnyAsync(e => e.Citizen.Id == citizenId, ct);
        }

        private async Task<bool> BeActivePollingStationAsync(long stationId, CancellationToken ct)
        {
            var dateNow = _timeProvider.GetUtcNow();

            return await _context.PollingStations.AnyAsync(
                ps => ps.Id == stationId && (ps.DisabledDate == null || ps.DisabledDate > dateNow),
                ct
            );
        }
    }
}
