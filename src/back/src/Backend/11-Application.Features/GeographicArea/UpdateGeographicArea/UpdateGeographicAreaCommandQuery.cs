using Application.Common.Enums;
using Application.Exceptions;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.GeographicArea.UpdateGeographicArea
{
    public class UpdateGeographicAreaCommandQuery : IRequest<Result<long>>
    {
        public long Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public LocationLevel Level { get; init; }
        public long? ParentId { get; init; }
    }

    public class UpdateGeographicAreaCommandValidator
        : AbstractValidator<UpdateGeographicAreaCommandQuery>
    {
        protected readonly ReadOnlyDbContext _context;

        public UpdateGeographicAreaCommandValidator(ReadOnlyDbContext context)
        {
            _context = context;

            //_dbContext = dbContext;
            //_currentUser = currentUser;

            // RG0 : seul SuperAdmin
            //RuleFor(x => x)
            //    .Must(_ => _currentUser.IsInRole("SuperAdmin"))
            //    .WithMessage("Seul un utilisateur avec le rôle 'SuperAdmin' peut modifier une zone géographique.");

            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);

            RuleFor(x => x.Level).IsInEnum();

            // RG2
            When(
                x => x.Level == LocationLevel.Continent,
                () =>
                {
                    RuleFor(x => x.ParentId)
                        .Must(pid => pid is null)
                        .WithMessage("Une zone de niveau Continent ne peut pas avoir de parent.");
                }
            );

            When(
                x => x.Level != LocationLevel.Continent,
                () =>
                {
                    RuleFor(x => x.ParentId)
                        .NotNull()
                        .WithMessage("Le parent est obligatoire pour ce niveau.")
                        .DependentRules(() =>
                        {
                            RuleFor(x => x)
                                .MustAsync(ParentHasCorrectLevelAsync)
                                .WithMessage(cmd =>
                                    $"Le parent doit être de niveau {(LocationLevel)((int)cmd.Level - 1)}."
                                );
                        });
                }
            );

            // RG1 : nom unique dans la même zone parente (en excluant l'entité courante)
            RuleFor(x => x)
                .MustAsync(NameUniqueInParentAsync)
                .WithMessage("Le nom doit être unique dans la même zone parente.");
        }

        private async Task<bool> NameUniqueInParentAsync(
            UpdateGeographicAreaCommandQuery cmd,
            CancellationToken ct
        )
        {
            return !await _context.GeographicAreas.AnyAsync(
                g => g.Id != cmd.Id && g.Name == cmd.Name && g.ParentId == cmd.ParentId,
                ct
            );
        }

        private async Task<bool> ParentHasCorrectLevelAsync(
            UpdateGeographicAreaCommandQuery cmd,
            CancellationToken ct
        )
        {
            if (cmd.ParentId is null)
                return false;

            var parent = await _context.GeographicAreas.FirstOrDefaultAsync(
                x => x.Id == cmd.ParentId.Value,
                ct
            );

            if (parent is null)
                return false;

            var expectedParentLevel = (LocationLevel)((int)cmd.Level - 1);
            return parent.Level == expectedParentLevel;
        }
    }

    public class UpdateGeographicAreaCommandHandler(WritableDbContext context)
        : IRequestHandler<UpdateGeographicAreaCommandQuery, Result<long>>
    {
        public async Task<Result<long>> Handle(
            UpdateGeographicAreaCommandQuery command,
            CancellationToken cancellationToken
        )
        {
            var entity =
                await context.GeographicAreas.FirstOrDefaultAsync(
                    x => x.Id == command.Id,
                    cancellationToken
                ) ?? throw new NotFoundException(nameof(GeographicAreaDao), command.Id);

            entity.Name = command.Name;
            entity.Level = command.Level;
            entity.ParentId = command.ParentId;

            await context.SaveChangesAsync(cancellationToken);

            return Result<long>.From(entity.Id);
        }
    }
}
