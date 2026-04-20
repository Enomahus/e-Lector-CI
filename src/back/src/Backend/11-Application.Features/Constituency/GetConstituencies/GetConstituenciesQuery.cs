using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tools.Logging;

namespace Application.Features.Constituency.GetConstituencies
{
    public class GetConstituenciesQuery : IRequest<IEnumerable<GetConstituenciesResponse>>
    {

    }

    public class GetConstituenciesQueryValidator :  AbstractValidator<GetConstituenciesQuery>
    {
        public GetConstituenciesQueryValidator() { }
    }

    public class GetConstituenciesQueryHandler(ReadOnlyDbContext context)
        : IRequestHandler<GetConstituenciesQuery, IEnumerable<GetConstituenciesResponse>>
    {
        public async Task<IEnumerable<GetConstituenciesResponse>> Handle(GetConstituenciesQuery request, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            //var dateNow = timeProvider.GetUtcNow();

            var constituencies = await context.Constituencies
                .Include(c => c.Subconstituency)
                .Include(c => c.PollingStations)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var lookup = constituencies.ToDictionary(x => x.Id,
                x => new GetConstituenciesResponse(x.Id, x.Code, x.Wording, x.Level,[]));

            var rootNodes = new List<GetConstituenciesResponse>();

            foreach (var node in constituencies) 
            {
                var data = lookup[node.Id];
                if(node.ParentId.HasValue && lookup.TryGetValue(node.ParentId.Value, out var parent))
                {
                    parent.SubConstituencies.Add(data);
                }
                else
                {
                    rootNodes.Add(data);
                }
            }

            return rootNodes;
        }
    }
}
