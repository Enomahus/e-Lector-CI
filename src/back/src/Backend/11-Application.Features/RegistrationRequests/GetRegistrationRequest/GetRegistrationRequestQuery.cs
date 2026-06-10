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
    [WithPermission([
        nameof(AppPermission.GetRegistrationRequest),
        nameof(AppPermission.GetRegistrationRequestForAdmin),
        nameof(AppPermission.GetRegistrationRequestForManagement),
    ])]
    public class GetRegistrationRequestQuery(Guid id) : IRequest<Result<GetRegistrationRequestResponse>>
    {
        public Guid Id { get; set; } = id;
    }

    public class GetRegistrationRequestQueryValidator : AbstractValidator<GetRegistrationRequestQuery>
    {
        public GetRegistrationRequestQueryValidator()
        {
            RuleFor(r => r.Id).NotEmpty().WithMessage(ValidationErrorCode.Required.ToString());
        }
    }

    public class GetRegistrationRequestQueryHandler(
        ReadOnlyDbContext context,
        TimeProvider timeProvider,
        ICurrentUserService currentUserService,
        ICurrentUserPermissionsProvider currentUserPermissions
    ) : IRequestHandler<GetRegistrationRequestQuery, Result<GetRegistrationRequestResponse>>
    {
        public async Task<Result<GetRegistrationRequestResponse>> Handle(
            GetRegistrationRequestQuery request,
            CancellationToken cancellationToken
        )
        {
            using var activity = ActivitySourceLog.CQRS.Start().AddParameter(request, r => r.Id);
            var dateNow = timeProvider.GetUtcNow();

            var currentUserId = currentUserService.UserId;
            var currentUser =
                await context
                    .Users.Include(u => u.UserConstituencies)
                        .ThenInclude(c => c.Constituency)
                    .FirstOrDefaultAsync(user => user.Id == currentUserId, cancellationToken)
                ?? throw new UserAccessException();
            var permissions = await currentUserPermissions.GetCurrentUserPermissionsAsync(cancellationToken);
            var userIsSuperAdmin = permissions.Contains(AppPermission.SuperAdmin.ToString());

            // Identification des rôles par les permissions clés présentes dans RolesData.cs
            var canManageAllRequests = permissions.Contains(
                AppPermission.GetRegistrationRequestsForAdmin.ToString()
            );
            var isManagementAgent =
                permissions.Contains(AppPermission.GetRegistrationRequestForManagement.ToString())
                && !canManageAllRequests; // Évite les collisions Admin/Agent

            var registrationRequest =
                await context
                    .RegistrationRequests.Include(r => r.Constituency)
                    .Include(r => r.Citizen.Father)
                    .Include(r => r.Citizen.Mother)
                    .Include(r => r.RegistrationRequestDocuments)
                    .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(RegistrationRequestDao), request.Id);

            if (!userIsSuperAdmin && !canManageAllRequests)
            {
                if (isManagementAgent)
                {
                    // RÈGLE AGENT : La demande doit appartenir à l'une des circonscriptions affectées à l'agent
                    bool hasAccessToConstituency = currentUser.UserConstituencies.Any(u =>
                        u.ConstituencyId == registrationRequest.ConstituencyId
                    );

                    if (!hasAccessToConstituency)
                    {
                        // L'agent n'est pas affecté à la circonscription de cette demande.
                        throw new UserAccessException();
                    }
                }
                else
                {
                    // RÈGLE ÉLECTEUR (Fallback) : L'utilisateur doit impérativement être l'auteur
                    if (currentUser.Id != registrationRequest.AuthorId)
                    {
                        // Un citoyen ne peut accéder qu'à ses propres demandes.
                        throw new UserAccessException();
                    }
                }
            }

            return Result<GetRegistrationRequestResponse>.From(
                GetRegistrationRequestResponse.FromDao(registrationRequest, currentUser, dateNow)
            );
        }
    }
}
