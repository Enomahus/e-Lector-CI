using Application.Audit;
using Application.Common.Enums;
using Application.Exceptions.Auth;
using Application.Features.RegistrationRequests.Common;
using Application.Interfaces.Services;
using Application.Models;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.AuditTrail.Attributes;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Pcea.Core.Net.Authorization.Application.Interfaces.Services;
using Tools.Logging;

namespace Application.Features.RegistrationRequests.UpdateRegistrationRequest
{
    [AuditParameters(
        Category = nameof(AuditCategory.RegistrationRequest),
        Action = nameof(AuditAction.RegistrationRequestUpdated)
    )]
    [WithPermission(
        nameof(AppPermission.UpdateRegistrationRequest),
        nameof(AppPermission.UpdateRegistrationRequestDraft)
    )]
    public class UpdateRegistrationRequestCommand : RegistrationRequestCommandBase, IRequest<Result<Guid>>
    {
        public Guid? Id { get; set; }
    }

    public class UpdateRegistrationRequestCommandValidator
        : RegistrationRequestValidationBase<UpdateRegistrationRequestCommand>
    {
        public UpdateRegistrationRequestCommandValidator(ReadOnlyDbContext context, TimeProvider timeProvider)
            : base(context, timeProvider) { }
    }

    public class UpdateRegistrationRequestCommandHandler(
        ICurrentUserService currentUserService,
        WritableDbContext context,
        RegistrationRequestService registrationRequestService,
        ICurrentUserPermissionsProvider currentUserPermissions
    ) : IRequestHandler<UpdateRegistrationRequestCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(
            UpdateRegistrationRequestCommand command,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start().AddParameter(command, c => c.Id);

            var currentUserId = currentUserService.UserId;
            var currentUser =
                await context
                    .Users.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken)
                ?? throw new UserAccessException();

            var permissions = await currentUserPermissions.GetCurrentUserPermissionsAsync(cancellationToken);
            var userIsSuperAdmin = permissions.Contains(AppPermission.SuperAdmin.ToString());

            var registrationRequestDao = await registrationRequestService.PrepareDaoForUpdate(
                command,
                cancellationToken
            );

            context.RegistrationRequests.Update(registrationRequestDao);
            await context.SaveChangesAsync(cancellationToken);

            return AuditResult<Guid>.From(registrationRequestDao.Reference, registrationRequestDao.Id);
        }
    }
}
