using Application.Common.Enums;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tools.Logging;

namespace Application.Features.GeographicArea.CreateGeographicArea;

public class CreateGeographicAreaCommand : IRequest<Result<long>>
{
    public string Name { get; init; } = string.Empty;
    public LocationLevel Level { get; init; }
    public long? ParentId { get; init; }
}

public class CreateGeographicAreaCommandValidator : AbstractValidator<CreateGeographicAreaCommand>
{
    protected readonly ReadOnlyDbContext _context;

    public CreateGeographicAreaCommandValidator(ReadOnlyDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationErrorCode.Required.ToString())
            .MaximumLength(50)
            .MustAsync(
                async (command, name, cancellationToken) =>
                {
                    // On vérifie si une zone avec le même nom existe déjà sous le même parent
                    // (Si ParentId est null, on vérifie au niveau racine)
                    return !await _context.GeographicAreas.AnyAsync(
                        x => x.Name == name && x.ParentId == command.ParentId,
                        cancellationToken
                    );
                }
            )
            .WithMessage(ValidationErrorCode.AlreadyExists.ToString());

        RuleFor(x => x.ParentId)
            // 1. Validation pour le niveau Continent (Level 1)
            .Must((command, parentId) => !parentId.HasValue)
            .When(x => x.Level == LocationLevel.Continent)
            .WithMessage(ValidationErrorCode.InvalidParent.ToString())
            // 2. Validation pour les autres niveaux (Level > 1)
            .NotNull()
            .When(x => x.Level != LocationLevel.Continent)
            .WithMessage(ValidationErrorCode.GepgraphicAreaMustHaveParent.ToString())
            // 3. Validation de la hiérarchie en base de données
            .MustAsync(
                async (command, parentId, cancellationToken) =>
                {
                    if (command.Level == LocationLevel.Continent)
                        return true;
                    if (!parentId.HasValue)
                        return false;

                    var expectedParentLevel = (LocationLevel)((int)command.Level - 1);

                    return await _context.GeographicAreas.AnyAsync(
                        x => x.Id == parentId.Value && x.Level == expectedParentLevel,
                        cancellationToken
                    );
                }
            )
            .When(x => x.Level != LocationLevel.Continent)
            .WithMessage(
                (command, parentId) =>
                    $"Parent must be level {(LocationLevel)((int)command.Level - 1)}."
            );
    }
}

public class CreateGeographicAreaCommandHandler(WritableDbContext context)
    : IRequestHandler<CreateGeographicAreaCommand, Result<long>>
{
    public async Task<Result<long>> Handle(
        CreateGeographicAreaCommand command,
        CancellationToken cancellationToken
    )
    {
        using var activity = ActivitySourceLog.CQRS.Start();

        var newEntity = new GeographicAreaDao
        {
            Name = command.Name,
            Level = command.Level,
            ParentId = command.ParentId,
        };
        context.GeographicAreas.Add(newEntity);
        await context.SaveChangesAsync(cancellationToken);

        return Result<long>.From(newEntity.Id);
    }
}
