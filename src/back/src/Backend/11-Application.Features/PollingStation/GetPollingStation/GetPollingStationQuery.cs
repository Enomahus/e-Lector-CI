using Application.Exceptions;
using Application.Features.Common.PollingStation;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tools.Logging;

namespace Application.Features.PollingStation.GetPollingStation;

public class GetPollingStationByIdQuery : IRequest<Result<PollingStationModel>>
{
    public long Id { get; set; }
}

public class GetPollingStationByIdQueryValidator : AbstractValidator<GetPollingStationByIdQuery>
{
    public GetPollingStationByIdQueryValidator()
    {
        RuleFor(v => v.Id).NotEmpty()
            .WithMessage(ValidationErrorCode.Required.ToString());
    }
}

public class GetPollingStationByIdQueryHandler(
    ReadOnlyDbContext context,
    TimeProvider timeProvider
 )
    : IRequestHandler<GetPollingStationByIdQuery, Result<PollingStationModel>>
{
    public async Task<Result<PollingStationModel>> Handle(
        GetPollingStationByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySourceLog.CQRS.Start();

        var dateNow = timeProvider.GetUtcNow();

        var dao = await context.PollingStations
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PollingStationDao), query.Id);

        var model = PollingStationModel.FromDao(dao, dateNow);
        return Result<PollingStationModel>.From(model);
    }
}
