using Application.Common.Enums;
using Application.Exceptions;
using Application.Features.Common.Constituency;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;

namespace Application.Features.Constituency.UpdateConstituency
{
    [WithPermission(nameof(AppPermission.UpdateConstituency))]
    public class UpdateConstituencyCommandQuery : ConstituencyModel, IRequest<Result<long>>
    {
        public long? Id { get; set; }
    }

    public class UpdateConstituencyCommandValidator
        : ConstituencyValidatorBase<UpdateConstituencyCommandQuery>
    {
        public UpdateConstituencyCommandValidator(ReadOnlyDbContext context): base(context)
        {
            RuleFor(v => v.Id).NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());
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

            entity.Wording = command.Wording;
            entity.Level = command.Level;
            entity.ParentId = command.ParentId;

            await context.SaveChangesAsync(cancellationToken);

            return Result<long>.From(entity.Id);
        }
    }
}
