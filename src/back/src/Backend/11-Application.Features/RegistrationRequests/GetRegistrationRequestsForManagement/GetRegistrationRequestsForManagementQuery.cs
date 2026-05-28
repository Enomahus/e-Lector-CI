using Application.Common.Enums;
using Application.Common.Pagination;
using Application.Features.Common;
using Application.Features.Common.Citizen;
using Application.Features.RegistrationRequests.Common;
using Application.Features.RegistrationRequests.GetRegistrationRequests;
using Application.Interfaces.Services;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.RegistrationRequests.GetRegistrationRequestsForManagement
{
    [WithPermission([nameof(AppPermission.GetRegistrationRequestsForManagement)])]
    public class GetRegistrationRequestsForManagementQuery
        : IRequest<Result<PagedList<GetRegistrationRequestsForManagementResponse>>>,
            IPagedQuery,
            IRegistrationRequestQuery
    {
        public string? Sort { get; set; }
        public string? Order { get; set; }
        public int? PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
    }

    public class GetRegistrationRequestsForManagementQueryValidator
        : AbstractValidator<GetRegistrationRequestsForManagementQuery>
    {
        public GetRegistrationRequestsForManagementQueryValidator() { }
    }

    public class GetRegistrationRequestsForManagementQueryHandler(
        ReadOnlyDbContext context,
        TimeProvider timeProvider,
        ICurrentUserService currentUserService
    )
        : BaseRegistrationRequestsHandler<GetRegistrationRequestsForManagementQuery>(context, timeProvider),
            IRequestHandler<
                GetRegistrationRequestsForManagementQuery,
                Result<PagedList<GetRegistrationRequestsForManagementResponse>>
            >
    {
        public async Task<Result<PagedList<GetRegistrationRequestsForManagementResponse>>> Handle(
            GetRegistrationRequestsForManagementQuery request,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var currentUserId = currentUserService.UserId;

            try
            {
                // Filtre par les circonscriptions assignées à l'agent courant
                var query = context
                    .RegistrationRequests.Include(r => r.Citizen)
                    .Where(r =>
                        context.UserConstituencies.Any(uc =>
                            uc.UserId == currentUserId && uc.ConstituencyId == r.ConstituencyId
                        )
                    )
                    .ApplySearch(request.Search)
                    .ApplySort(request.Sort, request.Order);

                var data = await GetDataAsync(query, cancellationToken);

                int pageIndex = request.PageIndex ?? 0;

                var result = await data.ToPagedListAsync(
                    pageIndex,
                    request.PageSize,
                    r => new GetRegistrationRequestsForManagementResponse
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
                //    r => new GetRegistrationRequestsForManagementResponse
                //    {
                //        Id = r.Id,
                //        Reference = r.Reference,
                //        SubmissionDate = r.SubmissionDate,
                //        Status = r.Status,
                //        ConstituencyId = r.ConstituencyId,
                //        ConstituencyName = r.Constituency.Wording,
                //        Comment = r.ReasonForRejection,
                //        Citizen = CitizenModel.FromDao(r.Citizen),
                //        CanBeDeleted = r.Status == RegistrationStatus.ToBeProcessed,
                //        AuthorName = ((r.Author.FirstName ?? "") + " " + (r.Author.LastName ?? "")).Trim(),
                //    },
                //    cancellationToken
                //);

                return Result<PagedList<GetRegistrationRequestsForManagementResponse>>.From(result);
            }
            catch (OperationCanceledException ex)
            {
                activity?.SetException(ex);
                throw;
            }
            catch (Exception ex)
            {
                activity?.SetException(ex);
                return Result<PagedList<GetRegistrationRequestsForManagementResponse>>.From();
            }
        }
    }
}
