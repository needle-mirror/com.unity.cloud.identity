using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// The interface for an organization repository.
    /// </summary>
    public interface IOrganizationRepository
    {
        /// <summary>
        /// Returns an `IEnumerable` of type <see cref="IOrganization"/>.
        /// </summary>
        Task<IEnumerable<IOrganization>> ListOrganizationsAsync();
    }

    /// <summary>
    /// Lists organizations from Cloud services.
    /// </summary>
    public class AuthenticatorOrganizationRepository : IOrganizationRepository
    {
        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IServiceHttpClient m_ServiceHttpClient;
        readonly IProjectProvider m_ProjectProvider;
        IEntityRoleProvider m_EntityRoleProvider;

        /// <summary>
        /// Builds an <see cref="AuthenticatorOrganizationRepository"/> class to list organizations from Cloud services.
        /// </summary>
        /// <param name="serviceHttpClient">A <see cref="IServiceHttpClient"/> implementation.</param>
        /// <param name="serviceHostResolver">A <see cref="ServicesHostConfiguration"/> instance.</param>
        public AuthenticatorOrganizationRepository(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            var unityServicesDomainResolver = new UnityServicesDomainResolver(true);
            m_ServiceHostResolver = serviceHostResolver.CreateCopyWithDomainResolverOverride(unityServicesDomainResolver);
            m_ServiceHttpClient = serviceHttpClient;
            m_ProjectProvider = new AuthenticatorProjectProvider(m_ServiceHttpClient, m_ServiceHostResolver);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IOrganization>> ListOrganizationsAsync()
        {
            var url = m_ServiceHostResolver.GetResolvedRequestUri("/api/unity/v1/users/me/organizations");
            var response = await m_ServiceHttpClient.GetAsync(url);
            var userInfoJson = await response.JsonDeserializeAsync<UnityUserInfoJson>();
            m_EntityRoleProvider = new AuthenticatorRoleProvider(userInfoJson.GenesisId, m_ServiceHttpClient, m_ServiceHostResolver);
            return userInfoJson.Organizations.Select(userOrgJson => new Organization(userOrgJson, m_ProjectProvider, m_EntityRoleProvider)).Cast<IOrganization>().ToList();
        }

    }
}
