using Pcea.Core.Net.Authorization.Application.Attributes;
using Pcea.Core.Net.Authorization.Application.Interfaces.Services;
using Pcea.Core.Net.Authorization.Application.Requests;
using Pcea.Core.Net.Authorization.Interfaces.Handlers;
using System.Reflection;

namespace Pcea.Core.Net.Authorization.Application.Behaviors;


/// <summary>
/// Base class for simple request with authorization on entity
/// The request must implements <see cref="IEntityAuthorizedRequest{T_EntityId}"/> to provide entityId on which to performe the authorization.
/// You must implement it with a concrete <see cref="T_EntityId"/> definition
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <typeparam name="T_EntityId"></typeparam>
public abstract class BasicEntityAuthorizationBehavior<TRequest, TResponse, T_EntityId>(
    ICurrentUserEntityPermissionsProvider<T_EntityId> permissionsProvider,
    ICurrentUserPermissionsProvider currentUserPermissionsProvider,
    IAuthorizationHandler handler
)
: EntityAuthorizationBehavior<TRequest, TResponse, T_EntityId>(
    permissionsProvider,
    currentUserPermissionsProvider,
    handler
) where TRequest : notnull, IEntityAuthorizedRequest<T_EntityId>
{

    protected override Task<T_EntityId> GetRequestEntityIdAsync(
        TRequest request,
        IEnumerable<IEntityPermissionsHolder> permissionAttributes,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(request.AuthorizationEntityId);
    }

    protected override Task<
        IEnumerable<IEntityPermissionsHolder>
    > GetRequiredPermissionsHoldersAsync(TRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<IEntityPermissionsHolder>>(
            request.GetType().GetCustomAttributes<EntityPermissionAttribute>()
        );
    }
}
