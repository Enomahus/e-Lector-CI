using Application.Common.Enums;
using Application.Common.Pagination;
using Application.Features.Common;
using Application.Features.Users.Common;
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
    public class GetUsersQuery : IRequest<Result<PagedList<GetUsersResponse>>>, IPagedQuery
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
    ) : IRequestHandler<GetUsersQuery, Result<PagedList<GetUsersResponse>>>
    {
        public async Task<Result<PagedList<GetUsersResponse>>> Handle(
            GetUsersQuery request,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var now = timeProvider.GetUtcNow();
            var currentUserId = currentUserService.UserId;

            try
            {
                var query = context
                    .Users.AsNoTracking()
                    .ApplySearch(request.Search)
                    .ApplySort(request.Sort, request.Order);

                // Filtre par circonscription
                if (request.ConstituencyId.HasValue)
                    query = query.Where(u =>
                        u.UserConstituencies.Any(uc => uc.ConstituencyId == request.ConstituencyId)
                    );

                int pageIndex = request.PageIndex ?? 0;

                var (data, totalCount) = await query
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
                        CanBeDeleted = x.Id != currentUserId && x.CreatedRegistrationRequests.Count == 0,
                        CanBeToggled = x.Id != currentUserId,
                        CreatedAt = x.CreatedAt,
                        IsActive = x.DisabledDate == null || x.DisabledDate > now,
                        Constituency =
                            x.UserConstituencies.Count > 0
                                ? x.UserConstituencies.First().Constituency.Wording
                                : null,
                        Roles = x.UserRoles.Select(r => r.Role.Name ?? ""),
                        AuthProvider = x.AuthProvider,
                    })
                    .ToListAsync(cancellationToken)
                    .ContinueWith(t => (Data: t.Result, TotalCount: query.Count()), cancellationToken);

                return Result<PagedList<GetUsersResponse>>.From(
                    new PagedList<GetUsersResponse>(data, totalCount)
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
