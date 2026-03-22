using System;
using System.Collections.Generic;
using System.Text;

namespace Pcea.Core.Net.Authorization.Application.Attributes;

/// <summary>
/// An object holding required permissions.
/// Use the correct list depending on needs (can be both).
/// In usage, the permissions handler will check for those permissions with an OR logic inside a list but with an AND between the lists
/// <example>
/// [EntityPermission(EntityPermissions: [nameof(PermA), nameof(PermB)], UserPermissions: [nameof(PermC)]
/// [EntityPermission(EntityPermissions: [nameof(PermD)], UserPermissions: [nameof(PermE)]
/// will result in
/// User must have PermA OR PermB OR (PermC on entity)
/// AND
/// User must have PerD OR (PermE on entity)
/// </example>
/// </summary>
public interface IEntityPermissionsHolder : IPermissionsHolder
{
    /// <summary>
    /// List of required permissions which should be set on link between user and entity
    /// </summary>
    /// <value></value>
    public IEnumerable<string> EntityPermissions { get; }
}
