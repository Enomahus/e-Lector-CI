using Application.Common.Enums;
using Application.Features.Common.PollingStation;
using Application.Models;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Constants;
using Tools.Logging;

namespace Application.Features.PollingStation.CreatePollingStation;

[WithPermission(nameof(AppPermission.CreatePollingStation))]
public class CreatePollingStationCommand : PollingStationModel, IRequest<Result<long>> { }

public class CreatePollingStationCommandValidator : PollingStationValidatorBase<CreatePollingStationCommand>
{
    public CreatePollingStationCommandValidator(ReadOnlyDbContext context)
        : base(context) { }
}

public class CreatePollingStationCommandHandler(WritableDbContext context)
    : IRequestHandler<CreatePollingStationCommand, Result<long>>
{
    public async Task<Result<long>> Handle(
        CreatePollingStationCommand command,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySourceLog.CQRS.Start();

        var newEntity = new PollingStationDao
        {
            StationNumber = await GetStationNumber(command.ConstituencyId, cancellationToken),
            Wording = command.Wording!,
            ConstituencyId = command.ConstituencyId,
        };
        context.PollingStations.Add(newEntity);
        await context.SaveChangesAsync(cancellationToken);

        return Result<long>.From(newEntity.Id);
    }

    private async Task<string> GetStationNumber(long constituencyId, CancellationToken cancellationToken)
    {
        const int maxElectorsPerStation = AppConstants.MAX_ELECTORS_PER_STATION;

        var totalElectors = await context.PollingStations
            .Where(ps => ps.ConstituencyId == constituencyId)
            .SelectMany(ps => ps.Electors)
            .CountAsync(cancellationToken);

        var stationNumber = totalElectors / maxElectorsPerStation + 1;

        return $"{stationNumber:D2}";
    }
}
