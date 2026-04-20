using Application.Common.Enums;
using Application.Exceptions;
using Application.Exceptions.Auth;
using Application.Interfaces.Services;
using Application.Models;
using Application.Models.Errors;
using FluentValidation;
using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.SQLServer.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Pcea.Core.Net.Authorization.Application.Interfaces.Services;
using Tools.Logging;

namespace Application.Features.RegistrationRequests.GetRegistrationRequest
{
    [WithPermission(nameof(AppPermission.GetRegistrationRequest))]
    public class GetRegistrationRequestQuery(Guid id) : IRequest<Result<GetRegistrationRequestResponse>>
    {
        public Guid Id { get; set; } = id;
    }

    public class GetRegistrationRequestQueryValidator : AbstractValidator<GetRegistrationRequestQuery>
    {
        public GetRegistrationRequestQueryValidator()
        {
            RuleFor(r => r.Id).NotEmpty()
                .WithMessage(ValidationErrorCode.Required.ToString());
        }
    }

    public class GetRegistrationRequestQueryHandler(
        ReadOnlyDbContext context, TimeProvider timeProvider,
        ICurrentUserService currentUserService,
        ICurrentUserPermissionsProvider currentUserPermissions
    )
        : IRequestHandler<GetRegistrationRequestQuery, Result<GetRegistrationRequestResponse>>
    {
        public async Task<Result<GetRegistrationRequestResponse>> Handle(GetRegistrationRequestQuery request, CancellationToken cancellationToken)
        {
            using var activity = ActivitySourceLog.CQRS.Start().AddParameter(request, r => r.Id);
            var dateNow = timeProvider.GetUtcNow();

            var currentUserId = currentUserService.UserId;
            var currentUser =
                await context.Users.Include(u => u.UserConstituencies)
                .ThenInclude(c => c.Constituency)
                .FirstOrDefaultAsync(
                    user => user.Id == currentUserId,
                    cancellationToken
                ) ?? throw new UserAccessException();
            var permissions = await currentUserPermissions.GetCurrentUserPermissionsAsync(cancellationToken);
            var userIsSuperAdmin = permissions.Contains(AppPermission.SuperAdmin.ToString());


            var registrationRequest = await context.RegistrationRequests
                .Include(r => r.Constituency)
                .Include(r => r.Citizen.Father)
                .Include(r => r.Citizen.Mother)
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(RegistrationRequestDao), request.Id);

            if(!userIsSuperAdmin && currentUser.UserConstituencies.Any(u => u.ConstituencyId != registrationRequest.ConstituencyId)
                || currentUser.Id != registrationRequest.AuthorId)
            {
                throw new UserAccessException();
            }

            return Result<GetRegistrationRequestResponse>.From(
                GetRegistrationRequestResponse.FromDao(registrationRequest, currentUser, dateNow)
                );

        }
    }
}
