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
            var pkceConfiguration = CreateConfiguration();

            if (m_AppNameProvider != null)
            {
                pkceConfiguration.AppName = m_AppNameProvider.GetAppName();
            }

            return await Task.FromResult(pkceConfiguration);
        }

        PkceConfiguration CreateConfiguration()
        {
            var serviceDomainHost =  GetServiceDomainHost();
            var serviceEnvironment = m_ServiceHostResolver?.GetResolvedEnvironment();

            var genesisSubdomain = serviceEnvironment switch
            {
                ServiceEnvironment.Staging => "api-staging",
                ServiceEnvironment.Test => "api-staging",
                _ => "api",
            };

            return new PkceConfiguration
            {
                AppName = "default",
                AllowAnonymous = false,
                CacheRefreshToken = true,
                ClientId = new ClientId("digital_twins"),
                ProxyLoginRedirectRoute = $"{serviceDomainHost}/app-linking/v1/login/redirect/",
                ProxyLoginCompletedRoute = $"{serviceDomainHost}/app-linking/v1/login/completed/",
                ProxySignOutCompletedRoute = $"{serviceDomainHost}/app-linking/v1/signout/completed/",
                LoginUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/authorize",
                TokenUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/token",
                RefreshTokenUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/token",
                LogoutUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/revoke",
                SignOutUrl = $"https://{genesisSubdomain}.unity.com/v1/oauth2/end-session?post_logout_redirect_uri=",
                UserInfoUrl = $"https://{genesisSubdomain}.unity.com/v1/users/current/openid",
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
