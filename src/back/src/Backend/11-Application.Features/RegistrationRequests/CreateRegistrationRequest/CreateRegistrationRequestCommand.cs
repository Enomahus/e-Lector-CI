using Application.Audit;
using Application.Common.Enums;
using Application.Exceptions;
using Application.Exceptions.Auth;
using Application.Features.RegistrationRequests.Common;
using Application.Interfaces.Services;
using Application.Models;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Pcea.Core.Net.AuditTrail.Attributes;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Tools.Logging;

namespace Application.Features.RegistrationRequests.CreateRegistrationRequest
{
    [AuditParameters(
        Category = nameof(AuditCategory.RegistrationRequest),
        Action = nameof(AuditAction.RegistrationRequestCreated)
     )]
    [WithPermission(nameof(AppPermission.CreateRegistrationRequest))]
    public class CreateRegistrationRequestCommand : RegistrationRequestCommandBase,  IRequest<Result<Guid>>
    {
    }

    public class CreateRegistrationRequestCommandValidator 
        : RegistrationRequestValidationBase<CreateRegistrationRequestCommand>
    {
        public CreateRegistrationRequestCommandValidator(
            ReadOnlyDbContext context, TimeProvider timeProvider) : base(context, timeProvider) { }
    }

    public class CreateRegistrationRequestCommandHandler(
        ICurrentUserService currentUserService,
        WritableDbContext context,
        TimeProvider timeProvider,
        IReferenceGeneratorService referenceGeneratorService
    ) : IRequestHandler<CreateRegistrationRequestCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateRegistrationRequestCommand command, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start();
            var dateNow = timeProvider.GetUtcNow();

            Guid? currentUserId = null;
            var constituencyId = command.RegistrationRequest!.ConstituencyId;
            UserDao? currentUser = null;

            currentUserId = currentUserService.UserId;
            currentUser =
                await context.Users
                    .Include(u => u.UserConstituencies)
                    .ThenInclude(uc => uc.Constituency)
                    .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken)
                ?? throw new NotFoundException(nameof(UserDao), currentUserId);

            if (currentUser.UserConstituencies.First().ConstituencyId != command.RegistrationRequest!.ConstituencyId)
            {
                throw new UserAccessException();
            }

            RegistrationRequestDao? registrationRequest = null;
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteInTransactionAsync(
                async() =>
                {
                    registrationRequest = command.RegistrationRequest.ToDao(constituencyId!.Value);
                    
                    registrationRequest.Reference = await referenceGeneratorService.GenerateRequestReferenceAsync(context, timeProvider, cancellationToken);
                    registrationRequest.AuthorId = currentUserId;
                    registrationRequest.Status = RegistrationStatus.ToBeProcessed;
                    registrationRequest.LastUpdaterId = registrationRequest.AuthorId;
                    registrationRequest.SoumissionDate = dateNow;

                    await context.RegistrationRequests.AddAsync(registrationRequest, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);

                    activity.AddParameter(registrationRequest, r => r.Id);
                },
                () => Task.FromResult(true)
            );

            return AuditResult<Guid>.From(registrationRequest!.Reference, registrationRequest!.Id);
        }
    }
}
