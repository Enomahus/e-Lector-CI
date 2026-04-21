using Application.Common.Enums;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.Constituency.GetConstituencies
{
    [WithPermission(nameof(AppPermission.GetConstituencies))]
    public class GetConstituenciesQuery : IRequest<Result<IEnumerable<GetConstituenciesResponse>>>
    {

    }

    public class GetConstituenciesQueryValidator :  AbstractValidator<GetConstituenciesQuery>
    {
        public GetConstituenciesQueryValidator() { }
    }

    public class GetConstituenciesQueryHandler(
        ReadOnlyDbContext context,
        TimeProvider timeProvider
    )
        : IRequestHandler<GetConstituenciesQuery, Result<IEnumerable<GetConstituenciesResponse>>>
    {
        public async Task<Result<IEnumerable<GetConstituenciesResponse>>> Handle(GetConstituenciesQuery request, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var dateNow = timeProvider.GetUtcNow();

            var rootConstituencies = await context.Constituencies
                .Include(c => c.Subconstituency)
                .ThenInclude(s => s.Subconstituency)
                .Include(c => c.PollingStations)  
                .Where(c => c.ParentId == null)
                .ToListAsync(cancellationToken);

            var response = rootConstituencies.Select(c => GetConstituenciesResponse.From(c, dateNow));
             
            return Result<IEnumerable<GetConstituenciesResponse>>.From(response);
        }
    }
}
