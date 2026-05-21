using Application.Common.Enums;
using Application.Models;
using FluentValidation;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.Users.GetRoles
{
    [WithPermission(nameof(AppPermission.GetRoles))]
    public class GetRolesQuery : IRequest<Result<List<RoleModel>>> { }

    public class GetRolesQueryValidator : AbstractValidator<GetRolesQuery> { }

    public class GetRolesQueryHandler(ReadOnlyDbContext context)
        : IRequestHandler<GetRolesQuery, Result<List<RoleModel>>>
    {
        public async Task<Result<List<RoleModel>>> Handle(
            GetRolesQuery request,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start();

            var rolesDao = await context
                .Roles.Select(r => new RoleModel() { Id = r.Id, Name = r.Name })
                .ToListAsync(cancellationToken);

            return Result<List<RoleModel>>.From(rolesDao);
        }
    }
}
