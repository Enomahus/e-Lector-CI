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
using Tools.Logging;

namespace Application.Features.Constituency.GetConstituency
{
    [WithPermission(nameof(AppPermission.GetConstituency))]
    public class GetConstituencyQuery(long id) : IRequest<Result<GetConstituencyResponse>>
    {
        public long Id { get; set; } = id;
    }

    public class GetConstituencyQueryValidator : AbstractValidator<GetConstituencyQuery>
    {
        public GetConstituencyQueryValidator()
        {
            RuleFor(v => v.Id).NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());
        }
    }

    public class GetConstituencyQueryHandler(
        ReadOnlyDbContext context,
        TimeProvider timeProvider
    )
        : IRequestHandler<GetConstituencyQuery, Result<GetConstituencyResponse>>
    {
        public async Task<Result<GetConstituencyResponse>> Handle(GetConstituencyQuery request, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start().AddParameter(request, r => r.Id);
            var dateNow = timeProvider.GetUtcNow();

            var constituencyDao = await context.Constituencies
                .Include(c => c.Subconstituency)
                .Include(c => c.PollingStations)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(ConstituencyDao), request.Id);

            var result = GetConstituencyResponse.FromDao(constituencyDao, dateNow);

            return Result<GetConstituencyResponse>.From(result);
        }
    }
}
