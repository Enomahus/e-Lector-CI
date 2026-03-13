using Application.Common.Enums;
using Application.Exceptions;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Constituency.UpdateConstituency
{
    public class UpdateConstituencyCommandQuery : IRequest<Result<long>>
    {
        public long Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public LocationLevel Level { get; init; }
        public long? ParentId { get; init; }
    }

    public class UpdateConstituencyCommandValidator
        : AbstractValidator<UpdateConstituencyCommandQuery>
    {
        protected readonly ReadOnlyDbContext _context;

        public UpdateConstituencyCommandValidator(ReadOnlyDbContext context)
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
                x => x.Level == LocationLevel.Region,
                () =>
                {
                    RuleFor(x => x.ParentId)
                        .Must(pid => pid is null)
                        .WithMessage("Une zone de niveau Region ne peut pas avoir de parent.");
                }
            );

            When(
                x => x.Level != LocationLevel.Region,
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
            UpdateConstituencyCommandQuery cmd,
            CancellationToken ct
        )
        {
            return !await _context.Constituencies.AnyAsync(
                g => g.Id != cmd.Id && g.Name == cmd.Name && g.ParentId == cmd.ParentId,
                ct
            );
        }

        private async Task<bool> ParentHasCorrectLevelAsync(
            UpdateConstituencyCommandQuery cmd,
            CancellationToken ct
        )
        {
            if (cmd.ParentId is null)
                return false;

            var parent = await _context.Constituencies.FirstOrDefaultAsync(
                x => x.Id == cmd.ParentId.Value,
                ct
            );

            if (parent is null)
                return false;

            var expectedParentLevel = (LocationLevel)((int)cmd.Level - 1);
            return parent.Level == expectedParentLevel;
        }
    }

    public class UpdateConstituencyCommandHandler(WritableDbContext context)
        : IRequestHandler<UpdateConstituencyCommandQuery, Result<long>>
    {
        public async Task<Result<long>> Handle(
            UpdateConstituencyCommandQuery command,
            CancellationToken cancellationToken
        )
        {
            var entity =
                await context.Constituencies.FirstOrDefaultAsync(
                    x => x.Id == command.Id,
                    cancellationToken
                ) ?? throw new NotFoundException(nameof(ConstituencyDao), command.Id);

            entity.Name = command.Name;
            entity.Level = command.Level;
            entity.ParentId = command.ParentId;

            await context.SaveChangesAsync(cancellationToken);

            return Result<long>.From(entity.Id);
        }
    }
}
