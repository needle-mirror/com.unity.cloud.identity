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
        IServiceHostResolver m_ServiceHostResolver;

        /// <summary>
        /// Builds a `PkceConfigurationProvider` handles the access to a <see cref="PkceConfiguration"/>.
        /// </summary>
        /// <param name="serviceHostResolver">The service host resolver for the service Url.</param>
        /// <param name="appNameProvider">An optional <see cref="IAppNameProvider"/> to build the unique uri scheme used to bind the app to the browser response in a login operation.</param>
        public PkceConfigurationProvider(IServiceHostResolver serviceHostResolver, IAppNameProvider appNameProvider)
        {
            m_ServiceHostResolver = serviceHostResolver;
            m_AppNameProvider = appNameProvider;
        }

        /// <summary>
        /// Builds a `PkceConfigurationProvider` handles the access to a <see cref="PkceConfiguration"/>.
        /// </summary>
        /// <param name="httpClient">An <see cref="IHttpClient"/> to make http requests.</param>
        /// <param name="accessTokenProvider">An <see cref="IAccessTokenProvider"/> to inject the authenticated access token in http requests.</param>
        /// <param name="appIdProvider">An <see cref="IAppIdProvider"/> to inject the app identifier in cloud endpoint requests.</param>
        /// <param name="appNameProvider">An optional <see cref="IAppNameProvider"/> to build the unique uri scheme used to bind the app to the browser response in a login operation.</param>
        /// <param name="serviceHostResolver">The service host resolver for the service Url.</param>
        public PkceConfigurationProvider(IHttpClient httpClient, IAccessTokenProvider accessTokenProvider, IServiceHostResolver serviceHostResolver, IAppIdProvider appIdProvider, IAppNameProvider appNameProvider = null)
        {
            m_ServiceHttpClient = new ServiceHttpClient(httpClient, accessTokenProvider, appIdProvider).WithApiSourceHeadersFromAssembly(Assembly.GetExecutingAssembly());
            m_ServiceHostResolver = serviceHostResolver;
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
            var pkceConfiguration = CreateConfiguration();

            if (m_AppNameProvider != null)
            {
                pkceConfiguration.AppName = m_AppNameProvider.GetAppName();
            }

            return await Task.FromResult(pkceConfiguration);
        }

        PkceConfiguration CreateConfiguration()
        {
            var serviceEnvironment = m_ServiceHostResolver?.GetResolvedEnvironment();
            var serviceDomainProvider = m_ServiceHostResolver?.GetResolvedDomainProvider();

            var serviceDomainHost = GetServiceDomainHost();

            // Azure specifically points to genesis-staging when on test/stg. All others point to genesis-prod
            var genesisSubdomain = (serviceEnvironment, serviceProvider: serviceDomainProvider) switch
            {
                (ServiceEnvironment.Staging, ServiceDomainProvider.Azure) => "api-staging",
                (ServiceEnvironment.Test, ServiceDomainProvider.Azure) => "api-staging",
                _ => "api",
            };

            return new PkceConfiguration
            {
                AppName = "default",
                AllowAnonymous = false,
                CacheRefreshToken = true,
                ClientId = "digital_twins",
                ProxyLoginRedirectRoute = $"{serviceDomainHost}/login/redirect/",
                ProxyLoginCompletedRoute = $"{serviceDomainHost}/login/completed/",
                ProxySignOutCompletedRoute = $"{serviceDomainHost}/signout/completed/",
                LoginUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/authorize",
                TokenUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/token",
                RefreshTokenUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/token",
                LogoutUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/revoke",
                SignOutUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/end-session?post_logout_redirect_uri=",
                CustomLoginParams = ""
            };
        }

        string GetServiceDomainHost()
        {
            var serviceAddress = m_ServiceHostResolver?.GetResolvedAddress();
            if (serviceAddress != null)
            {
                var serviceAddressUri = new Uri(serviceAddress);
                return serviceAddressUri.Host;
            }

            return string.Empty;
        }
    }
}
