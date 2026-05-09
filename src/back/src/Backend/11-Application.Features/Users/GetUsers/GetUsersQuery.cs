using Application.Common.Enums;
using Application.Features.Common;
using Application.Interfaces.Services;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.Users.GetUsers
{
    [WithPermission([nameof(AppPermission.GetUsers)])]
    public class GetUsersQuery : IRequest<Result<PagedList<GetUsersResponse>>>
    {
        public long? ConstituencyId { get; set; }
        public string? Sort { get; set; }
        public string? Order { get; set; }
        public int? PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
    }

    public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
    {
        public GetUsersQueryValidator() { }
    }

    public class GetUsersQueryHandler(
        WritableDbContext context,
        TimeProvider timeProvider,
        ICurrentUserService currentUserService
    )
        : IRequestHandler<GetUsersQuery, Result<PagedList<GetUsersResponse>>>
    {
        public async Task<Result<PagedList<GetUsersResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var now = timeProvider.GetUtcNow();
            var currentUserId = currentUserService.UserId;

            try
            {
                var query = context.Users.AsQueryable();

                // Filtre par circonscription
                if (request.ConstituencyId.HasValue)
                    query = query.Where(u => u.UserConstituencies.Any(uc => uc.ConstituencyId == request.ConstituencyId));

                // Recherche globale
                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.ToLower();
                    query = query.Where(u =>
                        (u.LastName != null && u.LastName.ToLower().Contains(search)) ||
                        (u.FirstName != null && u.FirstName.ToLower().Contains(search)) ||
                        (u.Email != null && u.Email.ToLower().Contains(search)) ||
                        (u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(search))
                    );
                }

                var totalCount = await query.CountAsync(cancellationToken);

                // Tri (whitelist des colonnes autorisées)
                var allowedSortColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "lastName", "firstName", "email", "createdAt", "isActive"
                };

                string sortColumn = allowedSortColumns.Contains(request.Sort ?? "") ? request.Sort!.ToLower() : "lastname";
                bool descending = string.Equals(request.Order, "desc", StringComparison.OrdinalIgnoreCase);

                query = (sortColumn, descending) switch
                {
                    ("firstname",  false) => query.OrderBy(u => u.FirstName),
                    ("firstname",  true)  => query.OrderByDescending(u => u.FirstName),
                    ("email",      false) => query.OrderBy(u => u.Email),
                    ("email",      true)  => query.OrderByDescending(u => u.Email),
                    ("createdat",  false) => query.OrderBy(u => u.CreatedAt),
                    ("createdat",  true)  => query.OrderByDescending(u => u.CreatedAt),
                    ("isactive",   false) => query.OrderBy(u => u.DisabledDate),
                    ("isactive",   true)  => query.OrderByDescending(u => u.DisabledDate),
                    (_,            false) => query.OrderBy(u => u.LastName),
                    (_,            true)  => query.OrderByDescending(u => u.LastName),
                };

                // Pagination
                int pageIndex = request.PageIndex ?? 0;
                var users = await query
                    .Skip(pageIndex * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new GetUsersResponse()
                    {
                        UserId = x.Id,
                        LastName = x.LastName,
                        FirstName = x.FirstName,
                        Email = x.Email,
                        Phone = x.PhoneNumber,
                        EmployeeNumber = x.EmployeeNumber,
                        UserType = x.UserType,
                        CanBeDeleted = x.Id != currentUserId && x.CreatedRegistrationRequests.Count == 0,
                        CanBeToggled = x.Id != currentUserId,
                        CreatedAt = x.CreatedAt,
                        IsActive = x.DisabledDate == null || x.DisabledDate > now,
                        Constituency = x.UserConstituencies.Count > 0
                            ? x.UserConstituencies.First().Constituency.Wording : null,
                        Roles = x.UserRoles.Select(r => r.Role.Name ?? ""),
                        AuthProvider = x.AuthProvider,
                    })
                    .ToListAsync(cancellationToken);

                return Result<PagedList<GetUsersResponse>>.From(
                    new PagedList<GetUsersResponse>(users, totalCount)
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
                return Result<PagedList<GetUsersResponse>>.From();
            }
        }
    }
}
