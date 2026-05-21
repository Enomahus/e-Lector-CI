using Application.Common.Enums;
using Application.Common.Pagination;
using Application.Features.Common;
using Application.Features.Common.Citizen;
using Application.Features.RegistrationRequests.Common;
using Application.Interfaces.Services;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.RegistrationRequests.GetRegistrationRequests
{
    [WithPermission([nameof(AppPermission.GetRegistrationRequests)])]
    public class GetRegistrationRequestsQuery
        : IRequest<Result<PagedList<GetRegistrationRequestsResponse>>>,
            IPagedQuery
    {
        public string? Sort { get; set; }
        public string? Order { get; set; }
        public int? PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
    }

    public class GetRegistrationRequestsQueryValidator : AbstractValidator<GetRegistrationRequestsQuery>
    {
        public GetRegistrationRequestsQueryValidator() { }
    }

    public class GetRegistrationRequestsQueryHandler(
        ReadOnlyDbContext context,
        ICurrentUserService currentUserService
    ) : IRequestHandler<GetRegistrationRequestsQuery, Result<PagedList<GetRegistrationRequestsResponse>>>
    {
        public async Task<Result<PagedList<GetRegistrationRequestsResponse>>> Handle(
            GetRegistrationRequestsQuery request,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var currentUserId = currentUserService.UserId;

            try
            {
                var query = context
                    .RegistrationRequests.Include(r => r.Citizen)
                    .Where(r => r.AuthorId == currentUserId)
                    .ApplySearch(request.Search)
                    .ApplySort(request.Sort, request.Order);

                int pageIndex = request.PageIndex ?? 0;

                var result = await query.ToPagedListAsync(
                    pageIndex,
                    request.PageSize,
                    r => new GetRegistrationRequestsResponse
                    {
                        Id = r.Id,
                        Reference = r.Reference,
                        SubmissionDate = r.SubmissionDate,
                        Status = r.Status,
                        ConstituencyId = r.ConstituencyId,
                        ConstituencyName = r.Constituency.Wording,
                        Comment = r.ReasonForRejection,
                        Citizen = CitizenModel.FromDao(r.Citizen),
                        CanBeDeleted =
                            currentUserId == r.AuthorId && r.Status == RegistrationStatus.ToBeProcessed,
                    },
                    cancellationToken
                );

                return Result<PagedList<GetRegistrationRequestsResponse>>.From(result);
            }
            catch (OperationCanceledException ex)
            {
                activity?.SetException(ex);
                throw;
            }
            catch (Exception ex)
            {
                activity?.SetException(ex);
                return Result<PagedList<GetRegistrationRequestsResponse>>.From();
            }
        }
    }
}
