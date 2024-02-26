using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// Implementation of <see cref="IProject"/> interface.
    /// </summary>
    internal class Project : IProject
    {
        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IServiceHttpClient m_ServiceHttpClient;

        readonly IEntityRoleProvider m_EntityRoleProvider;

        readonly GetRequestResponseCache<RangeResultsJson<ProjectMemberInfoJson>> m_GetRequestResponseCache;

        internal Project(ProjectJson projectJson, IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver, IEntityRoleProvider entityRoleProvider)
        {
            m_ServiceHostResolver = serviceHostResolver;
            m_ServiceHttpClient = serviceHttpClient;

            Descriptor = new ProjectDescriptor(new OrganizationId(projectJson.OrganizationGenesisId), new ProjectId(projectJson.GenesisId));
            Name = projectJson.Name;
            IconUrl = projectJson.IconUrl;
            CreatedAt = projectJson.CreatedAt;
            UpdatedAt = projectJson.UpdatedAt;
            ArchivedAt = projectJson.ArchivedAt;

            EnabledInAssetManager = projectJson.EnabledInAssetManager;

            m_EntityRoleProvider = entityRoleProvider;

            m_GetRequestResponseCache = new GetRequestResponseCache<RangeResultsJson<ProjectMemberInfoJson>>(60);
        }

        /// <inheritdoc/>
        public ProjectDescriptor Descriptor { get; }

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
        public bool EnabledInAssetManager { get; }

        /// <inheritdoc/>
        public async Task<IEnumerable<Role>> ListRolesAsync()
        {
            return await m_EntityRoleProvider.ListEntityRolesAsync(Descriptor.ProjectId.ToString(), EntityType.Project);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Permission>> ListPermissionsAsync()
        {
            return await m_EntityRoleProvider.ListEntityPermissionsAsync(Descriptor.ProjectId.ToString(), EntityType.Project);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IMemberInfo> ListMembersAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var rangeRequest = new RangeRequest<ProjectMemberInfoJson>(GetProjectMembers, 1000);
            var requestBasePath = $"/api/unity/legacy/v1/projects/{Descriptor.ProjectId}/users";
            var results = rangeRequest.Execute(requestBasePath, range, cancellationToken);
            await foreach (var member in results)
            {
                yield return new MemberInfo(member);
            }
        }

        async Task<RangeResultsJson<ProjectMemberInfoJson>> GetProjectMembers(string rangeRequestPath, CancellationToken cancellationToken)
        {
            var url = m_ServiceHostResolver.GetResolvedRequestUri(rangeRequestPath);

            if (m_GetRequestResponseCache.TryGetRequestResponseFromCache(url, out RangeResultsJson<ProjectMemberInfoJson> value))
            {
                return value;
            }

            var response = await m_ServiceHttpClient.GetAsync(url, cancellationToken: cancellationToken);
            var deserializedResponse = await response.JsonDeserializeAsync<RangeResultsJson<ProjectMemberInfoJson>>();
            return m_GetRequestResponseCache.AddGetRequestResponseToCache(url, deserializedResponse);
        }

    }
}
