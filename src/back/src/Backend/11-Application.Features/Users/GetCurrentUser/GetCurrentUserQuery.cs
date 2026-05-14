using Application.Common.Enums;
using Application.Exceptions;
using Application.Interfaces.Services;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Interfaces.Services;
using Tools.Logging;

namespace Application.Features.Users.GetCurrentUser
{
    public class GetCurrentUserQuery : IRequest<Result<GetCurrentUserResponse>> { }

    public class GetCurrentUserQueryValidator : AbstractValidator<GetCurrentUserQuery>
    {
        public GetCurrentUserQueryValidator() { }
    }

    public class GetCurrentUserQueryHandler(
        ICurrentUserService currentUserService,
        ICurrentUserPermissionsProvider currentUserPermissionsProvider,
        ReadOnlyDbContext context,
        TimeProvider timeProvider
    ) : IRequestHandler<GetCurrentUserQuery, Result<GetCurrentUserResponse>>
    {
        public async Task<Result<GetCurrentUserResponse>> Handle(
            GetCurrentUserQuery request,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();

            var userId = currentUserService.UserId;
            var dateNow = timeProvider.GetUtcNow();

            var currentUser = await context
                .Users.Include(u => u.UserConstituencies)
                .Include(u => u.UserRoles)
                    .ThenInclude(r => r.Role)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            var permissions = await currentUserPermissionsProvider.GetCurrentUserPermissionsAsync(
                cancellationToken
            );

            if (currentUser is null)
            {
                throw new NotFoundException(nameof(UserDao), userId);
            }

            return Result<GetCurrentUserResponse>.From(
                GetCurrentUserResponse.FromDao(
                    currentUser,
                    [.. permissions.Select(p => Enum.Parse<AppPermission>(p))],
                    dateNow
                )
            );
        }
    }
}
