using Application.Common.Enums;
using Application.Common.Pagination;
using Application.Features.Common;
using Application.Features.RegistrationRequests.Common;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.RegistrationRequests.GetRegistrationRequestsForAdmin
{
    [WithPermission([nameof(AppPermission.GetRegistrationRequestsForAdmin)])]
    public class GetRegistrationRequestsForAdminQuery
        : IRequest<Result<PagedList<GetRegistrationRequestsForAdminResponse>>>,
            IPagedQuery,
            IRegistrationRequestQuery
    {
        public string? Sort { get; set; }
        public string? Order { get; set; }
        public int? PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
    }

    public class GetRegistrationRequestsForAdminQueryValidator
        : AbstractValidator<GetRegistrationRequestsForAdminQuery>
    {
        public GetRegistrationRequestsForAdminQueryValidator() { }
    }

    public class GetRegistrationRequestsForAdminQueryHandler(
        ReadOnlyDbContext context,
        TimeProvider timeProvider
    )
        : BaseRegistrationRequestsHandler<GetRegistrationRequestsForAdminQuery>(context, timeProvider),
            IRequestHandler<
                GetRegistrationRequestsForAdminQuery,
                Result<PagedList<GetRegistrationRequestsForAdminResponse>>
            >
    {
        public async Task<Result<PagedList<GetRegistrationRequestsForAdminResponse>>> Handle(
            GetRegistrationRequestsForAdminQuery request,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();

            try
            {
                var query = _context
                    .RegistrationRequests.Include(r => r.Citizen)
                    .ApplySearch(request.Search)
                    .ApplySort(request.Sort, request.Order);

                int pageIndex = request.PageIndex ?? 0;

                var (data, totalCount) = await GetDataAsync(
                    query,
                    pageIndex,
                    request.PageSize,
                    cancellationToken
                );

                var items = data.Select(r => new GetRegistrationRequestsForAdminResponse
                    {
                        Id = r.Id,
                        Reference = r.Reference,
                        SubmissionDate = r.SubmissionDate,
                        Status = r.Status,
                        ConstituencyId = r.ConstituencyId,
                        ConstituencyName = r.ConstituencyName,
                        Comment = r.Comment,
                        Citizen = r.Citizen,
                        CanBeDeleted = r.CanBeDeleted,

                        IdentityDocumentOrCertificateId = r.IdentityDocumentOrCertificateId,
                        IdentityDocumentOrCertificateName = r.IdentityDocumentOrCertificateName,
                        PhotoId = r.PhotoId,
                        PhotoName = r.PhotoName,
                        AuthorName = r.AuthorName,
                    })
                    .ToList();

                var result = new PagedList<GetRegistrationRequestsForAdminResponse>(items, totalCount);

                await AddFileNameAsync(result.Items, cancellationToken);

                return Result<PagedList<GetRegistrationRequestsForAdminResponse>>.From(result);
            }
            catch (OperationCanceledException ex)
            {
                activity?.SetException(ex);
                throw;
            }
            catch (Exception ex)
            {
                activity?.SetException(ex);
                return Result<PagedList<GetRegistrationRequestsForAdminResponse>>.From();
            }
        }
    }
}
