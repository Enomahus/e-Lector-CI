using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tools.Logging;

namespace Application.Features.PollingStation.GetPollingStations
{
    public class GetPollingStationsByConstituencyIdQuery(long id): IRequest<Result<List<GetPollingStationsByConstituencyIdResponse>>>
    {
        public long ConstituencyId { get; set; } = id;
    }

    public class GetPollingStationsByConstituencyIdQueryValidator : AbstractValidator<GetPollingStationsByConstituencyIdQuery>
    {
        private readonly ReadOnlyDbContext _context;
        public GetPollingStationsByConstituencyIdQueryValidator(ReadOnlyDbContext context)
        {
            _context = context;

            RuleFor(v => v.ConstituencyId).NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString())
                .DependentRules(() =>
                {
                    RuleFor(v => v.ConstituencyId)
                    .MustAsync(CheckConstituencyExistsAsync)
                    .WithMessage(ValidationErrorCode.ConstituencyMustExist.ToString());
                });
        }

        private async Task<bool> CheckConstituencyExistsAsync(long constituencyId, CancellationToken token)
        {
            return await _context.Constituencies.AnyAsync(c => c.Id == constituencyId, token);
        }
    }

    public class GetPollingStationsByConstituencyIdQueryHandler(
        ReadOnlyDbContext context,
        TimeProvider timeProvider
    )
    : IRequestHandler<GetPollingStationsByConstituencyIdQuery, Result<List<GetPollingStationsByConstituencyIdResponse>>>
    {
        public async Task<Result<List<GetPollingStationsByConstituencyIdResponse>>> Handle(
            GetPollingStationsByConstituencyIdQuery query,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var now = timeProvider.GetUtcNow();

            var daos = await context.PollingStations
                .Include(ps => ps.Constituency)
                .Where(x => x.ConstituencyId == query.ConstituencyId)
                .ToListAsync(cancellationToken);

            var models = daos
                .Select(dao => GetPollingStationsByConstituencyIdResponse.From(dao, now))
                .ToList();

            return Result<List<GetPollingStationsByConstituencyIdResponse>>.From(models);
        }
    }
}
