using Application.Common.Enums;
using Application.Exceptions;
using Application.Features.Common.Constituency;
using Application.Models;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

        // Générer le code de la circonscription
        var code = await GenerateConstituencyCode(command.ParentId, command.Level, cancellationToken);

        var newEntity = new ConstituencyDao
        {
            Code = code,
            Wording = command.Wording,
            Level = command.Level,
            ParentId = command.ParentId,
        };
        context.Constituencies.Add(newEntity);
        await context.SaveChangesAsync(cancellationToken);

        return Result<long>.From(newEntity.Id);
    }

    private async Task<string> GenerateConstituencyCode(long? parentId, LocationLevel level, CancellationToken cancellationToken)
    {
        if (parentId is null)
        {
            // C'est un niveau racine (Region)
            // Compter les circonscriptions existantes au même niveau sans parent
            var existingCount = await context.Constituencies
                .Where(c => c.Level == level && c.ParentId == null)
                .CountAsync(cancellationToken);
            
            return (existingCount + 1).ToString("D3"); // 001, 002, 003...
        }
        else
        {
            // C'est un sous-niveau
            var parent = await context.Constituencies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == parentId, cancellationToken)
                ?? throw new NotFoundException(nameof(ConstituencyDao), parentId);
            
            //if (parent is null)
            //    throw new InvalidOperationException($"Parent constituency with ID {parentId} not found");

            // Compter les enfants du même parent au même niveau
            var siblingCount = await context.Constituencies
                .Where(c => c.ParentId == parentId && c.Level == level)
                .CountAsync(cancellationToken);

            var siblingCode = (siblingCount + 1).ToString("D3");
            return parent.Code + siblingCode;
        }
    }
}
