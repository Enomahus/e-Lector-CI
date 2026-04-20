using Application.Common.Enums;
using Application.Features.Common.Constituency;
using Application.Models;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.Constituency.CreateConstituency;

[WithPermission(nameof(AppPermission.CreateConstituency))]
public class CreateConstituencyCommand : ConstituencyModel, IRequest<Result<long>>
{
}

public class CreateConstituencyCommandValidator : ConstituencyValidatorBase<CreateConstituencyCommand>
{    
    public CreateConstituencyCommandValidator(ReadOnlyDbContext context): base(context)
    {                       
    }

}

public class CreateConstituencyCommandHandler(WritableDbContext context)
    : IRequestHandler<CreateConstituencyCommand, Result<long>>
{
    public async Task<Result<long>> Handle(
        CreateConstituencyCommand command,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySourceLog.CQRS.Start();

        var newEntity = new ConstituencyDao
        {
            //Code = command.Code,
            Wording = command.Wording,
            Level = command.Level,
            ParentId = command.ParentId,
        };
        context.Constituencies.Add(newEntity);
        await context.SaveChangesAsync(cancellationToken);

        return Result<long>.From(newEntity.Id);
    }
}
