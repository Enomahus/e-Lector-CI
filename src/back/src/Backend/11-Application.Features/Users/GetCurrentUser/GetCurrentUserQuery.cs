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
using System;
using System.Collections.Generic;
using System.Text;
using Tools.Logging;

namespace Application.Features.Users.GetCurrentUser
{
    public class GetCurrentUserQuery: IRequest<Result<GetCurrentUserResponse>>
    {
    }

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
        public async Task<Result<GetCurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start();

            var userId = currentUserService.UserId;

            var currentUser = await context
                .Users.Include(u => u.UserConstituencies)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            var permissions = await currentUserPermissionsProvider.GetCurrentUserPermissionsAsync(cancellationToken);

            if(currentUser is null)
            {
                throw new NotFoundException(nameof(UserDao), userId);
            }

            var result = GetCurrentUserResponse.FromDao(currentUser,
                [.. permissions.Select(p => Enum.Parse<AppPermission>(p))],
                timeProvider.GetUtcNow()
                );

            return Result<GetCurrentUserResponse>.From(result);
        }
    }
}
