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
            .WithMessage(ValidationErrorCode.MaxLength.ToString())
            .MustAsync(BeUniqueNameInParentAsync)
            .WithMessage(ValidationErrorCode.AlreadyExists.ToString());

        When(
            x => x.Level == LocationLevel.Continent,
            () =>
            {
                RuleFor(x => x.ParentId)
                    .Must(parentId => !parentId.HasValue)
                    .WithMessage(ValidationErrorCode.InvalidParent.ToString());
            }
        );

        When(
            x => x.Level != LocationLevel.Continent,
            () =>
            {
                RuleFor(x => x.ParentId)
                    .NotNull()
                    .WithMessage(ValidationErrorCode.GeographicAreaMustHaveParent.ToString())
                    .DependentRules(() =>
                    {
                        RuleFor(x => x)
                            .MustAsync(ParentHasCorrectLevelAsync)
                            .WithMessage(ValidationErrorCode.InvalidLevel.ToString());
                    });
            }
        );
    }

    private async Task<bool> BeUniqueNameInParentAsync(
        CreateGeographicAreaCommand command,
        string name,
        CancellationToken cancellationToken
    )
    {
        return !await _context.GeographicAreas.AnyAsync(
            x => x.Name == name && x.ParentId == command.ParentId,
            cancellationToken
        );
    }

    private async Task<bool> ParentHasCorrectLevelAsync(
        CreateGeographicAreaCommand command,
        CancellationToken cancellationToken
    )
    {
        if (command.ParentId is null)
            return false;

        var parent = await _context.GeographicAreas.FirstOrDefaultAsync(
            x => x.Id == command.ParentId.Value,
            cancellationToken
        );
        if (parent is null)
            return false;

        var expectedParentLevel = (LocationLevel)((int)command.Level - 1);
        return parent.Level == expectedParentLevel;
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
