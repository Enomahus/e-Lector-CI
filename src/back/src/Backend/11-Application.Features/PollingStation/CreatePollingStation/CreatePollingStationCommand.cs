using Application.Common.Enums;
using Application.Features.Common.PollingStation;
using Application.Models;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Pcea.Core.Net.Authorization.Application.Attributes;
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
            StationNumber = command.StationNumber!,
            Wording = command.Wording!,
            ConstituencyId = command.ConstituencyId,
        };
        context.PollingStations.Add(newEntity);
        await context.SaveChangesAsync(cancellationToken);

        return Result<long>.From(newEntity.Id);
    }
}
