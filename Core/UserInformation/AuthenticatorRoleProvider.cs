using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// A class to hold all Entity type string values.
    /// </summary>
    internal static class EntityType
    {
        public static readonly string Organization = "organization";
        public static readonly string Project = "project";
    }

    /// <summary>
    /// The interface to validate and list roles assigned to a user.
    /// </summary>
    public interface IRoleProvider
    {
        /// <summary>
        /// Validate if a role is assigned to a user.
        /// </summary>
        /// <param name="roleName">The name of the role to look for.</param>
        /// <returns>A task that once completed returns if a role is assigned to a user.</returns>
        Task<bool> HasRoleAsync(string roleName);

        /// <summary>
        /// Validate if a permission is assigned to a user.
        /// </summary>
        /// <param name="permission">The permission to look for.</param>
        /// <returns>A task that once completed returns if a permission is assigned to a user.</returns>
        Task<bool> HasPermissionAsync(string permission);

        /// <summary>
        /// List roles assigned to a user.
        /// </summary>
        /// <returns>A task that once completed returns the list of roles assigned to a user.</returns>
        Task<IEnumerable<string>> ListRolesAsync();

        /// <summary>
        /// List permissions assigned to a user.
        /// </summary>
        /// <returns>A task that once completed returns the list of permissions assigned to a user.</returns>
        Task<IEnumerable<string>> ListPermissionsAsync();
    }

    /// <summary>
    /// The interface to validate a role assigned to a user on an entity.
    /// </summary>
    internal interface IEntityRoleProvider
    {
        /// <summary>
        /// A Task to return if a role is assigned to a user on an entity.
        /// </summary>
        /// <param name="roleName">The name of the role to look for.</param>
        /// <param name="entityId">The string id of the entity.</param>
        /// <param name="entityType">The type of the entity.</param>
        Task<bool> HasEntityRoleAsync(string roleName, string entityId, string entityType);

        /// <summary>
        /// A Task to return if a permission is assigned to a user on an entity.
        /// </summary>
        /// <param name="permission">The name of the permission to look for.</param>
        /// <param name="entityId">The string id of the entity.</param>
        /// <param name="entityType">The type of the entity.</param>
        Task<bool> HasEntityPermissionAsync(string permission, string entityId, string entityType);

        /// <summary>
        /// A Task to return the list of roles assigned to a user on an entity.
        /// </summary>
        /// <param name="entityId">The string id of the entity.</param>
        /// <param name="entityType">The type of the entity.</param>
        /// <returns>The list of roles assigned to a user on an entity.</returns>
        Task<IEnumerable<string>> ListEntityRolesAsync(string entityId, string entityType);

        /// <summary>
        /// A Task to return the list of permissions assigned to a user on an entity.
        /// </summary>
        /// <param name="entityId">The string id of the entity.</param>
        /// <param name="entityType">The type of the entity.</param>
        /// <returns>The list of permissions assigned to a user on an entity.</returns>
        Task<IEnumerable<string>> ListEntityPermissionsAsync(string entityId, string entityType);
    }

    [Serializable]
    internal class EntityJson
    {
        public string EntityId  { get; set; }

        public string EntityType  { get; set; }

        public int OriginId  { get; set; }

        public IEnumerable<PolicyJson> Policies  { get; set; }
    }

    [Serializable]
    internal class PolicyJson
    {
        public string Id  { get; set; }

        public string RoleId  { get; set; }

        public string RoleName  { get; set; }

        public string[] RolePermissions  { get; set; }
    }

    internal class AuthenticatorRoleProvider : IEntityRoleProvider
    {
        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IServiceHttpClient m_ServiceHttpClient;
        readonly string m_UserId;

        // InMemory caching of requests result
        readonly Dictionary<string, (DateTime, IEnumerable<EntityJson>)> m_ServiceCallResultSnapshot = new ();
        // Time in seconds before the request is allowed to reach the service endpoint again.
        readonly int m_ServiceCallResultSnapshotTimeLimitInSeconds = 10;

        public AuthenticatorRoleProvider(string userId, IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            m_ServiceHostResolver = serviceHostResolver;
            m_ServiceHttpClient = serviceHttpClient;
            m_UserId = userId;
        }

        async Task<IEnumerable<EntityJson>> GetUserEntityRoles(string entityId, string entityType)
        {
            var url = m_ServiceHostResolver.GetResolvedRequestUri($"/api/access/legacy/v1/users/{m_UserId}/entities?entityType={entityType}&entityId={entityId}&filterByEntityType[]={entityType}");

            // Use cached result before making a new call to service
            if (m_ServiceCallResultSnapshot.ContainsKey(url) && (DateTime.Now - m_ServiceCallResultSnapshot[url].Item1).Seconds < m_ServiceCallResultSnapshotTimeLimitInSeconds)
            {
                return m_ServiceCallResultSnapshot[url].Item2;
            }

            // First time, or time to refresh if cached result has expired
            var response = await m_ServiceHttpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var returnValue = JsonSerialization.Deserialize<IEnumerable<EntityJson>>(responseContent);

            // Memorize, then return value
            m_ServiceCallResultSnapshot[url] = (DateTime.Now, returnValue);
            return m_ServiceCallResultSnapshot[url].Item2;
        }

        public async Task<bool> HasEntityRoleAsync(string roleName, string entityId, string entityType)
        {
            var entityListJson = await GetUserEntityRoles(entityId, entityType);
            return entityListJson
                .Any(entity => entity.Policies.Any(policy => policy.RoleName.Equals(roleName)));
        }

        public async Task<bool> HasEntityPermissionAsync(string permission, string entityId, string entityType)
        {
            var entityListJson = await GetUserEntityRoles(entityId, entityType);
            return entityListJson
                .Any(entity => entity.Policies.Any(policy => policy.RolePermissions.Contains(permission)));
        }

        public async Task<IEnumerable<string>> ListEntityRolesAsync(string entityId, string entityType)
        {
            var entityListJson = await GetUserEntityRoles(entityId, entityType);
            var roles = new List<string>();
            foreach (var entity in entityListJson)
                roles.AddRange(entity.Policies.Select(policy => policy.RoleName));

            return roles;
        }

        public async Task<IEnumerable<string>> ListEntityPermissionsAsync(string entityId, string entityType)
        {
            var entityListJson = await GetUserEntityRoles(entityId, entityType);
            var permissions = new List<string>();
            foreach (var entity in entityListJson)
                permissions.AddRange(entity.Policies.SelectMany(policy => policy.RolePermissions));

            return permissions;
        }
    }
}
