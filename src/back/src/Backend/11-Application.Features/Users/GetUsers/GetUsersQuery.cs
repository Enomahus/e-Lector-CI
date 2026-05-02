using Application.Common.Enums;
using Application.Features.Common;
using Application.Interfaces.Services;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
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

            var users = await context
                .Users.Where(u => request.ConstituencyId == null
                    || u.UserConstituencies.Any(uc => uc.ConstituencyId == request.ConstituencyId)
                )
                .Select(x => new GetUsersResponse()
                {
                    UserId = x.Id,
                    LastName = x.LastName,
                    FirstName = x.FirstName,
                    Email = x.Email,
                    Phone = x.PhoneNumber,
                    CanBeDeleted = x.Id != currentUserId && x.CreatedRegistrationRequests.Count == 0,
                    CanBeToggled = x.Id != currentUserId,
                    CreatedAt = x.CreatedAt,
                    IsActive = x.DisabledDate == null || x.DisabledDate > now,
                    Constituency = x.UserConstituencies.Count > 0 
                        ? x.UserConstituencies.First().Constituency.Wording : null,
                    Roles = x.UserRoles.Select(r => r.Role.Name ?? ""),
                    AuthProvider = x.AuthProvider,
                })
                .ToList();

            return Result<PagedList<GetUsersResponse>>.From(users);

        }
    }
}
