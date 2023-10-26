using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{

    /// <summary>
    /// The interface for an organization.
    /// </summary>
    public interface IOrganization : IRoleProvider
    {
        /// <summary>
        /// Gets the Genesis id of the organization.
        /// </summary>
        OrganizationId Id { get; }

        /// <summary>
        /// Gets the name of the organization.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the role of the user in the organization.
        /// </summary>
        string Role { get; }

        /// <summary>
        /// An awaitable Task that returns the list of <see cref="IProject"/> the user can access in the organization.
        /// </summary>
        /// <param name="range">A range of <see cref="IProject"/> to request.</param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is an async enumeration of <see cref="IProject"/>.</returns>
        public IAsyncEnumerable<IProject> ListProjectsAsync(Range range, CancellationToken cancellationToken = default);
    }

    internal class OrganizationJson
    {
        public string Id { get; set; }

        public string GenesisId { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }
    }

    /// <summary>
    /// A class implementing <see cref="IOrganization"/>.
    /// </summary>
    [Serializable]
    internal class Organization : IOrganization
    {
        readonly IProjectProvider m_ProjectProvider;
        readonly IEntityRoleProvider m_EntityRoleProvider;

        internal Organization(OrganizationJson organizationJson, IProjectProvider projectProvider, IEntityRoleProvider entityRoleProvider)
        {
            Id = new OrganizationId(organizationJson.GenesisId);
            EntityId = organizationJson.Id;
            Name = organizationJson.Name;
            Role = organizationJson.Role;

            m_ProjectProvider = projectProvider;
            m_EntityRoleProvider = entityRoleProvider;
        }

        /// <inheritdoc />
        public OrganizationId Id { get; }

        string EntityId { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Role { get; }

        /// <inheritdoc />
        public IAsyncEnumerable<IProject> ListProjectsAsync(Range range, CancellationToken cancellationToken = default)
        {
            return m_ProjectProvider.GetOrganizationProjects(Id, m_EntityRoleProvider, range, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<bool> HasRoleAsync(string roleName)
        {
            return await m_EntityRoleProvider.HasEntityRoleAsync(roleName, Id.ToString(), EntityType.Organization);
        }

        /// <inheritdoc/>
        public async Task<bool> HasPermissionAsync(string permission)
        {
            return await m_EntityRoleProvider.HasEntityPermissionAsync(permission, Id.ToString(), EntityType.Organization);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> ListRolesAsync()
        {
            return await m_EntityRoleProvider.ListEntityRolesAsync(Id.ToString(), EntityType.Organization);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> ListPermissionsAsync()
        {
            return await m_EntityRoleProvider.ListEntityPermissionsAsync(Id.ToString(), EntityType.Organization);
        }
    }
}
