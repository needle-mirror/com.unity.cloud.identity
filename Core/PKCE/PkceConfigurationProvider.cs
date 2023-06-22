using System;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// Handles the access to a <see cref="PkceConfiguration"/>.
    /// </summary>
    public class PkceConfigurationProvider : IPkceConfigurationProvider
    {
        readonly IAppNameProvider m_AppNameProvider;
        readonly IServiceHttpClient m_ServiceHttpClient;
        ServiceHostConfiguration m_ServiceHostConfiguration;

        /// <summary>
        /// Builds a `PkceConfigurationProvider` handles the access to a <see cref="PkceConfiguration"/>.
        /// </summary>
        /// <param name="serviceHostConfiguration">An optional service environment configuration.</param>
        /// <param name="appNameProvider">An optional <see cref="IAppNameProvider"/> to build the unique uri scheme used to bind the app to the browser response in a login operation.</param>
        public PkceConfigurationProvider(ServiceHostConfiguration serviceHostConfiguration, IAppNameProvider appNameProvider)
        {
            m_ServiceHostConfiguration = serviceHostConfiguration;
            m_AppNameProvider = appNameProvider;
        }

        /// <summary>
        /// Builds a `PkceConfigurationProvider` handles the access to a <see cref="PkceConfiguration"/>.
        /// </summary>
        /// <param name="httpClient">An <see cref="IHttpClient"/> to make http requests.</param>
        /// <param name="accessTokenProvider">An <see cref="IAccessTokenProvider"/> to inject the authenticated access token in http requests.</param>
        /// <param name="appIdProvider">An <see cref="IAppIdProvider"/> to inject the app identifier in cloud endpoint requests.</param>
        /// <param name="appNameProvider">An optional <see cref="IAppNameProvider"/> to build the unique uri scheme used to bind the app to the browser response in a login operation.</param>
        /// <param name="serviceHostConfiguration">An optional service environment configuration.</param>
        [Obsolete("Replaced by constructor requiring only ServiceHostConfiguration and IAppNameProvider.")]
        public PkceConfigurationProvider(IHttpClient httpClient, IAccessTokenProvider accessTokenProvider, ServiceHostConfiguration serviceHostConfiguration, IAppIdProvider appIdProvider, IAppNameProvider appNameProvider = null)
        {
            m_ServiceHttpClient = new ServiceHttpClient(httpClient, accessTokenProvider, appIdProvider).WithApiSourceHeadersFromAssembly(Assembly.GetExecutingAssembly());
            m_ServiceHostConfiguration = serviceHostConfiguration;
            m_AppNameProvider = appNameProvider;
        }

        /// <summary>
        /// Creates a task that results in a <see cref="PkceConfiguration"/> when internal update is completed.
        /// </summary>
        /// <returns>
        /// A task that results in a <see cref="PkceConfiguration"/> when internal update is completed.
        /// </returns>
        public async Task<PkceConfiguration> GetPkceConfigurationAsync()
        {
            return await UpdatePkceConfiguration();
        }

        async Task<PkceConfiguration> UpdatePkceConfiguration()
        {
            // Eventually, we fetch the information from Cloud endpoint using m_ServiceHttpClient
            PkceConfiguration pkceConfiguration = CreateConfiguration(m_ServiceHostConfiguration);

            if (m_AppNameProvider != null)
            {
                pkceConfiguration.AppName = m_AppNameProvider.GetAppName();
            }

            return await Task.FromResult(pkceConfiguration);
        }

        static PkceConfiguration CreateConfiguration(ServiceHostConfiguration serviceHostConfiguration)
        {
            var serviceEnvironment = serviceHostConfiguration.ResolveEnvironment().environment;
            var serviceDomainProvider = serviceHostConfiguration.ResolveProvider();
            var serviceDomainHost = serviceHostConfiguration.GetServiceDomain();

            // Azure specifically points to genesis-staging when on test/stg. All others point to genesis-prod
            string genesisSubdomain = (serviceEnvironment, serviceProvider: serviceDomainProvider) switch
            {
                (ServiceEnvironment.Staging, ServiceDomainProvider.Azure) => "api-staging",
                (ServiceEnvironment.Test, ServiceDomainProvider.Azure) => "api-staging",
                _ => "api",
            };

            string environmentPrefix = serviceEnvironment switch
            {
                ServiceEnvironment.Staging => "stg.",
                ServiceEnvironment.Test => "test.",
                _ => string.Empty,
            };

            return new PkceConfiguration
            {
                AppName = "default",
                AllowAnonymous = false,
                CacheRefreshToken = true,
                ClientId = "digital_twins",
                ProxyLoginRedirectRoute = $"{environmentPrefix}{serviceDomainHost}/login/redirect/",
                ProxyLoginCompletedRoute = $"{environmentPrefix}{serviceDomainHost}/login/completed/",
                ProxySignOutCompletedRoute = $"{environmentPrefix}{serviceDomainHost}/signout/completed/",
                LoginUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/authorize",
                TokenUrl = $"https://{environmentPrefix}{serviceDomainHost}/api/auth/token/refresh",
                RefreshTokenUrl = $"https://{environmentPrefix}{serviceDomainHost}/api/auth/token/refresh",
                LogoutUrl = $"https://{environmentPrefix}{serviceDomainHost}/api/auth/token/revoke",
                SignOutUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/end-session?post_logout_redirect_uri=",
                CustomLoginParams = ""
            };
        }
    }
}
