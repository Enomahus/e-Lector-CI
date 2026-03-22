namespace Pcea.Core.Net.Authorization.Application.Attributes;

/// <summary>
/// An object holding required permissions.
/// </example>
/// </summary>
public interface IPermissionsHolder
{
    /// <summary>
    /// List of required permissions which should be set on user
    /// </summary>
    /// <value></value>
    public IEnumerable<string> UserPermissions { get; }
}
