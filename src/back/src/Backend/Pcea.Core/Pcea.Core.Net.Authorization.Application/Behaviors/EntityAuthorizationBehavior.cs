using MediatR;
using Pcea.Core.Net.Authorization.Application.Attributes;
using Pcea.Core.Net.Authorization.Application.Exceptions;
using Pcea.Core.Net.Authorization.Application.Interfaces.Services;
using Pcea.Core.Net.Authorization.Interfaces.Handlers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pcea.Core.Net.Authorization.Application.Behaviors
{
    public abstract class EntityAuthorizationBehavior<TRequest, TResponse, T_EntityId>(
        ICurrentUserEntityPermissionsProvider<T_EntityId> entityPermissionsProvider,
        ICurrentUserPermissionsProvider currentUserPermissionsProvider,
        IAuthorizationHandler handler
    ) : Behavior<TRequest, TResponse> where TRequest: notnull
    {
        protected readonly ICurrentUserEntityPermissionsProvider<T_EntityId> _permissionsProvider =
        entityPermissionsProvider;
        protected readonly IAuthorizationHandler _authorizationHandler = handler;


        protected override async Task<TResponse> HandleRequest(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken
        )
        {
            var requiredPermissionsHolders = await GetRequiredPermissionsHoldersAsync(
                request,
                cancellationToken
            );

            if (!requiredPermissionsHolders.Any())
            {
                return await next();
            }

            if (!await _permissionsProvider.IsCurrentUserAuthenticatedAsync(cancellationToken))
            {
                throw new UserAccessException(
                    null,
                    new Dictionary<string, object?>() { { "No user authenticated", null } }
                );
            }
            var entityId = await GetRequestEntityIdAsync(
                request,
                requiredPermissionsHolders,
                cancellationToken
            );
            var entityPermissionCodes =
                await _permissionsProvider.GetCurrentUserPermissionsOnEntityAsync(
                    entityId,
                    cancellationToken
                );
            var userPermissionCodes =
                await currentUserPermissionsProvider.GetCurrentUserPermissionsAsync(cancellationToken);

            foreach (var permissionHolder in requiredPermissionsHolders)
            {
                // Check for user permissions
                Models.AuthorizationResult result = new() { IsAuthorized = true };
                if (permissionHolder.UserPermissions.Any())
                {
                    result = await CheckPermissionsAsync(
                        permissionHolder.UserPermissions,
                        userPermissionCodes,
                        cancellationToken
                    );
                }
                if (
                    permissionHolder.EntityPermissions.Any()
                    && (!permissionHolder.UserPermissions.Any() || !result.IsAuthorized)
                )
                {
                    result = await CheckPermissionsAsync(
                        permissionHolder.EntityPermissions,
                        entityPermissionCodes,
                        cancellationToken
                    );
                }
                if (!result.IsAuthorized)
                {
                    result.AdditionalData.Add("Entity Id", entityId);
                    throw new UserAccessException(null, result.AdditionalData);
                }
            }

            // User is authorized / authorization not required
            return await next();
        }

        protected virtual async Task<Models.AuthorizationResult> CheckPermissionsAsync(
            IEnumerable<string> requiredPermissionsCodes,
            IEnumerable<string> permissionsCodes,
            CancellationToken cancellationToken
        )
        {
            await _authorizationHandler.BuildAsync(
                requiredPermissionsCodes,
                permissionsCodes,
                cancellationToken
            );
            return await _authorizationHandler.HandleAsync(cancellationToken);
        }

        protected abstract Task<T_EntityId> GetRequestEntityIdAsync(
            TRequest request,
            IEnumerable<IEntityPermissionsHolder> permissionAttributes,
            CancellationToken cancellationToken
        );

        protected abstract Task<
            IEnumerable<IEntityPermissionsHolder>
        > GetRequiredPermissionsHoldersAsync(TRequest request, CancellationToken cancellationToken);
    }
}
