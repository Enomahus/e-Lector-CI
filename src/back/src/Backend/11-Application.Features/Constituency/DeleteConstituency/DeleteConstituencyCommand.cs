using Application.Common.Enums;
using Application.Exceptions;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Tools.Logging;

namespace Application.Features.Constituency.DeleteConstituency
{
    [WithPermission(nameof(AppPermission.DeleteConstituency))]
    public class DeleteConstituencyCommand(long id) : IRequest<Result>
    {
        public long Id { get; set; } = id;
    }

    public class DeleteConstituencyCommandValidator : AbstractValidator<DeleteConstituencyCommand> 
    {
        private readonly ReadOnlyDbContext _context;
        public DeleteConstituencyCommandValidator(ReadOnlyDbContext context)
        {
            _context = context;

            RuleFor(c => c.Id).NotEmpty().WithMessage(ValidationErrorCode.Required.ToString())
                .DependentRules(() =>
                {
                    RuleFor(c => c.Id)
                    .MustAsync(
                        (pollingStationId, token) =>
                        _context.PollingStations.AnyAsync(ps => ps.Id == pollingStationId, token)
                    )
                    .WithMessage(ValidationErrorCode.PollingStationMustExist.ToString());
                });
        }
    }

    public class DeleteConstituencyCommandHandler(WritableDbContext context) : IRequestHandler<DeleteConstituencyCommand, Result>
    {
        public async Task<Result> Handle(DeleteConstituencyCommand command, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start().AddParameter(command, c => c.Id);

            var constituencyDao = await context.Constituencies
                .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(ConstituencyDao), command.Id);

            context.Constituencies.Remove(constituencyDao);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Default();

        }
    }
}
