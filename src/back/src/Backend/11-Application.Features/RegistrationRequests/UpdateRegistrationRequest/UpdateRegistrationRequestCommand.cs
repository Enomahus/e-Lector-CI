using Application.Audit;
using Application.Common.Enums;
using Application.Features.RegistrationRequests.Common;
using Application.Models;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Pcea.Core.Net.AuditTrail.Attributes;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.RegistrationRequests.UpdateRegistrationRequest
{
    [AuditParameters(
        Category = nameof(AuditCategory.RegistrationRequest),
        Action = nameof(AuditAction.RegistrationRequestUpdated)
    )]
    [WithPermission(
        nameof(AppPermission.UpdateRegistrationRequest),
        nameof(AppPermission.UpdateRegistrationRequestsForManagement)
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
        WritableDbContext context,
        RegistrationRequestService registrationRequestService
    ) : IRequestHandler<UpdateRegistrationRequestCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(
            UpdateRegistrationRequestCommand command,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start().AddParameter(command, c => c.Id);

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
