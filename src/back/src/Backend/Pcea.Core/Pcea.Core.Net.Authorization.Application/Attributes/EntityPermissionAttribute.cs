using System;
using System.Collections.Generic;
using System.Text;

namespace Pcea.Core.Net.Authorization.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class EntityPermissionAttribute : Attribute, IEntityPermissionsHolder
    {
        /// <summary>
        /// A user which should have at least one of those permissions on the entity with
        /// permission to be able to access the feature.
        /// </summary>
        /// <value></value>
        public IEnumerable<string> UserPermissions { get; } = [];

        public IEnumerable<string> EntityPermissions { get; } = [];

        public EntityPermissionAttribute() { }

        public EntityPermissionAttribute(string[] entityPermissions, string[]? userPermissions = null)
        {
            EntityPermissions = entityPermissions;
            UserPermissions = userPermissions ?? [];
        }
    }
}
