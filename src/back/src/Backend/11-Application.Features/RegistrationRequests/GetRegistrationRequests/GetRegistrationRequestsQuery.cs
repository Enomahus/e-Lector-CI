using Application.Common.Enums;
using Application.Features.Common;
using Application.Features.Common.Citizen;
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
    public class GetRegistrationRequestsQuery : IRequest<Result<PagedList<GetRegistrationRequestsResponse>>>
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
                    .AsQueryable();

                // Recherche globale
                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.ToLower();
                    query = query.Where(r =>
                        (r.Reference != null && r.Reference.ToLower().Contains(search))
                        || (r.Citizen.FirstName != null && r.Citizen.FirstName.ToLower().Contains(search))
                        || (r.Citizen.LastName != null && r.Citizen.LastName.ToLower().Contains(search))
                        || (
                            r.Constituency.Wording != null
                            && r.Constituency.Wording.ToLower().Contains(search)
                        )
                    );
                }

                var totalCount = await query.CountAsync(cancellationToken);

                // Tri (whitelist des colonnes autorisées)
                var allowedSortColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "reference",
                    "submissionDate",
                    "status",
                    "constituencyName",
                    "citizenLastName",
                    "citizenFirstName",
                };

                string sortColumn = allowedSortColumns.Contains(request.Sort ?? "")
                    ? request.Sort!.ToLower()
                    : "submissiondate";
                bool descending = string.Equals(request.Order, "desc", StringComparison.OrdinalIgnoreCase);

                query = (sortColumn, descending) switch
                {
                    ("reference", false) => query.OrderBy(r => r.Reference),
                    ("reference", true) => query.OrderByDescending(r => r.Reference),
                    ("status", false) => query.OrderBy(r => r.Status),
                    ("status", true) => query.OrderByDescending(r => r.Status),
                    ("constituencyname", false) => query.OrderBy(r => r.Constituency.Wording),
                    ("constituencyname", true) => query.OrderByDescending(r => r.Constituency.Wording),
                    ("citizenlastname", false) => query.OrderBy(r => r.Citizen.LastName),
                    ("citizenlastname", true) => query.OrderByDescending(r => r.Citizen.LastName),
                    ("citizenfirstname", false) => query.OrderBy(r => r.Citizen.FirstName),
                    ("citizenfirstname", true) => query.OrderByDescending(r => r.Citizen.FirstName),
                    (_, false) => query.OrderBy(r => r.SubmissionDate),
                    (_, true) => query.OrderByDescending(r => r.SubmissionDate),
                };

                // Pagination
                int pageIndex = request.PageIndex ?? 0;
                var registrationRequests = await query
                    .Skip(pageIndex * request.PageSize)
                    .Take(request.PageSize)
                    .Select(r => new GetRegistrationRequestsResponse
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
                            currentUserId == r.AuthorId && (r.Status == RegistrationStatus.ToBeProcessed),
                    })
                    .ToListAsync(cancellationToken);

                return Result<PagedList<GetRegistrationRequestsResponse>>.From(
                    new PagedList<GetRegistrationRequestsResponse>(registrationRequests, totalCount)
                );
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
