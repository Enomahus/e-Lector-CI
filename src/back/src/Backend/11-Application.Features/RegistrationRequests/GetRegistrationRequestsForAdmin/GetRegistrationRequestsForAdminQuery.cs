using Application.Common.Enums;
using Application.Common.Pagination;
using Application.Features.Common;
using Application.Features.Common.Citizen;
using Application.Features.RegistrationRequests.Common;
using Application.Features.RegistrationRequests.GetRegistrationRequests;
using Application.Features.RegistrationRequests.GetRegistrationRequestsForManagement;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.RegistrationRequests.GetRegistrationRequestsForAdmin
{
    [WithPermission([nameof(AppPermission.GetRegistrationRequestsFormAdmin)])]
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
        : BaseRegistrationRequestsHandler<GetRegistrationRequestsQuery>(context, timeProvider),
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
                var query = context
                    .RegistrationRequests.Include(r => r.Citizen)
                    .ApplySearch(request.Search)
                    .ApplySort(request.Sort, request.Order);

                var data = await GetDataAsync(query, cancellationToken);

                int pageIndex = request.PageIndex ?? 0;

                var result = await data.ToPagedListAsync(
                    pageIndex,
                    request.PageSize,
                    r => new GetRegistrationRequestsForAdminResponse
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
                        CertificateOfNationalityDocumentId = r.CertificateOfNationalityDocumentId,
                        CertificateOfNationalityDocumentName = r.CertificateOfNationalityDocumentName,
                        IdentityDocumentId = r.IdentityDocumentId,
                        IdentityDocumentName = r.IdentityDocumentName,
                        PhotoId = r.PhotoId,
                        PhotoName = r.PhotoName,
                        AuthorName = r.AuthorName,
                    },
                    cancellationToken
                );

                await AddFileNameAsync(result.Items, cancellationToken);

                //var result = await query.ToPagedListAsync(
                //    pageIndex,
                //    request.PageSize,
                //    r => new GetRegistrationRequestsForAdminResponse
                //    {
                //        Id = r.Id,
                //        Reference = r.Reference,
                //        SubmissionDate = r.SubmissionDate,
                //        Status = r.Status,
                //        ConstituencyId = r.ConstituencyId,
                //        ConstituencyName = r.Constituency.Wording,
                //        Comment = r.ReasonForRejection,
                //        Citizen = CitizenModel.FromDao(r.Citizen),
                //        CanBeDeleted = false,
                //        AuthorName = ((r.Author.FirstName ?? "") + " " + (r.Author.LastName ?? "")).Trim(),
                //    },
                //    cancellationToken
                //);

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
