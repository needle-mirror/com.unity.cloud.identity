using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{

    internal class ProjectsJson
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public int Total { get; set; }

        public IEnumerable<ProjectJson> Results { get; set; }
    }

    /// <summary>
    /// An interface that exposes project information.
    /// </summary>
    public interface IProject : IRoleProvider
    {
        /// <summary>
        /// Gets the id of the project.
        /// </summary>
        ProjectDescriptor Descriptor { get; }

        /// <summary>
        /// Gets the name of the organization.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        public string IconUrl { get; }

        /// <summary>
        /// Gets the date time of creation the project.
        /// </summary>
        public DateTime? CreatedAt { get; }

        /// <summary>
        /// Gets the date time of last update the project.
        /// </summary>
        public DateTime? UpdatedAt { get; }

        /// <summary>
        /// Gets the date time of last update the project.
        /// </summary>
        public DateTime? ArchivedAt { get; }

        /// <summary>
        /// Gets the kids store compliance of the project.
        /// </summary>
        public bool? KidsStoreCompliance { get; }

        /// <summary>
        /// Gets the coppa of the project.
        /// </summary>
        public string Coppa { get; }

        /// <summary>
        /// Gets the default environment id for the project.
        /// </summary>
        public string DefaultEnvironmentId { get; }

    }

    internal class ProjectJson
    {
        public string Id { get; set; }

        public string GenesisId { get; set; }

        public string Name { get; set; }

        public string IconUrl {  get; set; }

        public DateTime? CreatedAt {  get; set; }

        public DateTime? UpdatedAt {  get; set; }

        public DateTime? ArchivedAt {  get; set; }

        public bool? KidsStoreCompliance {  get; set; }

        public string Coppa {  get; set; }

        public string OrganizationGenesisId {  get; set; }

        public string DefaultEnvironmentId {  get; set; }
    }

    /// <summary>
    /// Implementation of <see cref="IProject"/> interface.
    /// </summary>
    internal class Project : IProject
    {
        readonly IEntityRoleProvider m_EntityRoleProvider;

        internal Project(ProjectJson projectJson, IEntityRoleProvider entityRoleProvider)
        {
            Descriptor = new ProjectDescriptor(new OrganizationId(projectJson.OrganizationGenesisId), new ProjectId(projectJson.Id));
            GenesisId = projectJson.GenesisId;
            Name = projectJson.Name;
            IconUrl = projectJson.IconUrl;
            CreatedAt = projectJson.CreatedAt;
            UpdatedAt = projectJson.UpdatedAt;
            ArchivedAt = projectJson.ArchivedAt;
            KidsStoreCompliance = projectJson.KidsStoreCompliance;
            Coppa = projectJson.Coppa;
            DefaultEnvironmentId = projectJson.DefaultEnvironmentId;
            m_EntityRoleProvider = entityRoleProvider;
        }

        /// <inheritdoc/>
        public ProjectDescriptor Descriptor { get; }

        string GenesisId { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string IconUrl { get; }

        /// <inheritdoc/>
        public DateTime? CreatedAt { get; }

        /// <inheritdoc/>
        public DateTime? UpdatedAt { get; }

        /// <inheritdoc/>
        public DateTime? ArchivedAt { get; }

        /// <inheritdoc/>
        public bool? KidsStoreCompliance { get; }

        /// <inheritdoc/>
        public string Coppa { get; }

        /// <inheritdoc/>
        public string DefaultEnvironmentId { get; }

        /// <inheritdoc/>
        public async Task<bool> HasRoleAsync(string roleName)
        {
            return await m_EntityRoleProvider.HasEntityRoleAsync(roleName, Descriptor.ProjectId.ToString(), EntityType.Project);
        }

        /// <inheritdoc/>
        public async Task<bool> HasPermissionAsync(string permission)
        {
            return await m_EntityRoleProvider.HasEntityPermissionAsync(permission, Descriptor.ProjectId.ToString(), EntityType.Project);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> ListRolesAsync()
        {
            return await m_EntityRoleProvider.ListEntityRolesAsync(Descriptor.ProjectId.ToString(), EntityType.Project);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> ListPermissionsAsync()
        {
            return await m_EntityRoleProvider.ListEntityPermissionsAsync(Descriptor.ProjectId.ToString(), EntityType.Project);
        }
    }
}
