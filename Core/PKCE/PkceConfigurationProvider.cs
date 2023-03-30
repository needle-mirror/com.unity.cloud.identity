using System;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// This <see cref="IPkceConfigurationProvider"/> implementation handles the access to a <see cref="PkceConfiguration"/>.
    /// </summary>
    public class PkceConfigurationProvider : IPkceConfigurationProvider
    {
        readonly IAppNameProvider m_AppNameProvider;
        readonly IServiceHttpClient m_ServiceHttpClient;

        /// <summary>
        /// Builds a PkceConfigurationProvider handles the access to a <see cref="PkceConfiguration"/>.
        /// </summary>
        /// <param name="httpClient">An <see cref="IHttpClient"/> to make http requests.</param>
        /// <param name="accessTokenProvider">An <see cref="IAccessTokenProvider"/> to inject the authenticated access token in http requests.</param>
        /// <param name="appIdProvider">An <see cref="IAppIdProvider"/> to inject the app identifier in cloud endpoint requests.</param>
        /// <param name="appNameProvider">An optional <see cref="IAppNameProvider"/> to build the unique uri scheme used to bind the app to the browser response in a login operation.</param>
        public PkceConfigurationProvider(IHttpClient httpClient, IAccessTokenProvider accessTokenProvider, IAppIdProvider appIdProvider, IAppNameProvider appNameProvider = null)
        {
            m_ServiceHttpClient = new ServiceHttpClient(httpClient, accessTokenProvider, appIdProvider).WithApiSourceHeadersFromAssembly(Assembly.GetExecutingAssembly());
            m_AppNameProvider = appNameProvider;
        }

        /// <summary>
        /// Creates a Task that results in a <see cref="PkceConfiguration"/> when internal update is completed.
        /// </summary>
        /// <returns>
        /// A Task that results in a <see cref="PkceConfiguration"/> when internal update is completed.
        /// </returns>
        public async Task<PkceConfiguration> GetPkceConfigurationAsync()
        {
            return await UpdatePkceConfiguration();
        }

        async Task<PkceConfiguration> UpdatePkceConfiguration()
        {
            // Eventually, we fetch the information from Cloud endpoint using m_ServiceHttpClient
            var pkceConfiguration = PkceConfiguration.DefaultConfiguration;
            if (m_AppNameProvider != null)
            {
                pkceConfiguration.AppName = m_AppNameProvider.GetAppName();
            }
            return await Task.FromResult(pkceConfiguration);
        }
    }
}
