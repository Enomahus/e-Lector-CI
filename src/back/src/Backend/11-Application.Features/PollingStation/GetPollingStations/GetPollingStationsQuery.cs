using Application.Common.Enums;
using Application.Common.Pagination;
using Application.Features.Common;
using Application.Features.PollingStation.Common;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.PollingStation.GetPollingStations
{
    [WithPermission([nameof(AppPermission.GetPollingStations)])]
    public class GetPollingStationsQuery
        : IRequest<Result<PagedList<GetPollingStationsResponse>>>,
            IPagedQuery
    {
        public string? Sort { get; set; }
        public string? Order { get; set; }
        public int? PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
    }

    public class GetPollingStationsQueryValidator : AbstractValidator<GetPollingStationsQuery>
    {
        public GetPollingStationsQueryValidator() { }
    }

    public class GetPollingStationsQueryHandler(ReadOnlyDbContext context, TimeProvider timeProvider)
        : IRequestHandler<GetPollingStationsQuery, Result<PagedList<GetPollingStationsResponse>>>
    {
        public async Task<Result<PagedList<GetPollingStationsResponse>>> Handle(
            GetPollingStationsQuery request,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var now = timeProvider.GetUtcNow();

            try
            {
                var query = context
                    .PollingStations.ApplySearch(request.Search)
                    .ApplySort(request.Sort, request.Order);

                int pageIndex = request.PageIndex ?? 0;

                var result = await query.ToPagedListAsync(
                    pageIndex,
                    request.PageSize,
                    ps => new GetPollingStationsResponse(
                        (long?)ps.Constituency.Parent.Parent.Parent.Parent.Id,
                        ps.Constituency.Parent.Parent.Parent.Parent.Code,
                        ps.Constituency.Parent.Parent.Parent.Parent.Wording,
                        (long?)ps.Constituency.Parent.Parent.Parent.Id,
                        ps.Constituency.Parent.Parent.Parent.Code,
                        ps.Constituency.Parent.Parent.Parent.Wording,
                        (long?)ps.Constituency.Parent.Parent.Id,
                        ps.Constituency.Parent.Parent.Code,
                        ps.Constituency.Parent.Parent.Wording,
                        (long?)ps.Constituency.Parent.Id,
                        ps.Constituency.Parent.Code,
                        ps.Constituency.Parent.Wording,
                        (long?)ps.Constituency.Id,
                        ps.Constituency.Code,
                        ps.Constituency.Wording,
                        ps.Id,
                        ps.StationNumber,
                        ps.DisabledDate == null || ps.DisabledDate > now,
                        ps.DisabledDate
                    ),
                    cancellationToken
                );

                return Result<PagedList<GetPollingStationsResponse>>.From(result);
            }
            catch (OperationCanceledException ex)
            {
                activity?.SetException(ex);
                throw;
            }
            catch (Exception ex)
            {
                activity?.SetException(ex);
                return Result<PagedList<GetPollingStationsResponse>>.From();
            }
        }
    }
}
