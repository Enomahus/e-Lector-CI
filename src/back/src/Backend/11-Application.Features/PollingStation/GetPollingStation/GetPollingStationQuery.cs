using Application.Features.Common.PollingStation;
using Application.Models;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tools.Logging;

namespace Application.Features.PollingStation.GetPollingStation;

public class GetPollingStationByIdQuery : IRequest<Result<PollingStationModel?>>
{
    public long Id { get; set; }
}

public class GetPollingStationByIdQueryHandler(ReadOnlyDbContext context)
    : IRequestHandler<GetPollingStationByIdQuery, Result<PollingStationModel?>>
{
    public async Task<Result<PollingStationModel?>> Handle(
        GetPollingStationByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySourceLog.CQRS.Start();

        var dao = await context.PollingStations
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

        if (dao is null)
            return Result<PollingStationModel?>.From(null);

        var model = PollingStationModel.FromDao(dao, DateTimeOffset.UtcNow);
        return Result<PollingStationModel?>.From(model);
    }
}

public class GetPollingStationsByConstituencyIdQuery : IRequest<Result<List<PollingStationModel>>>
{
    public long ConstituencyId { get; set; }
}

public class GetPollingStationsByConstituencyIdQueryHandler(ReadOnlyDbContext context)
    : IRequestHandler<GetPollingStationsByConstituencyIdQuery, Result<List<PollingStationModel>>>
{
    public async Task<Result<List<PollingStationModel>>> Handle(
        GetPollingStationsByConstituencyIdQuery query,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySourceLog.CQRS.Start();

        var daos = await context.PollingStations
            .Where(x => x.ConstituencyId == query.ConstituencyId)
            .ToListAsync(cancellationToken);

        var models = daos
            .Select(dao => PollingStationModel.FromDao(dao, DateTimeOffset.UtcNow))
            .ToList();

        return Result<List<PollingStationModel>>.From(models);
    }
}
