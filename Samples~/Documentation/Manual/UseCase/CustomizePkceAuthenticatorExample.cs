using System.Threading.Tasks;
using Unity.Cloud.AppLinking.Runtime;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity.Runtime;

namespace Unity.Cloud.Identity.Documentation
{
    // Referenced:
    // - /Documentation~/use-case-customize-pkce-authentication.md
    namespace CustomizePkceAuthenticatorExample
    {
        #region CustomPkceAuthenticator
        public static class PlatformServices
        {
            static IAuthenticator s_PkceAuthenticator;
            public static IAuthenticator PkceAuthenticator => s_PkceAuthenticator;

            internal class CustomPkceConfigurationProvider : IPkceConfigurationProvider
            {
                readonly PkceConfiguration m_PkceConfiguration = new ()
                {
                    CacheRefreshToken = false,
                    ClientId = new ClientId("digital_twins"),
                    ProxyLoginRedirectRoute = "https://services.api.unity.com/app-linking/v1/login/redirect/",
                    ProxyLoginCompletedRoute = "https://services.api.unity.com/app-linking/v1/login/completed/",
                    ProxySignOutCompletedRoute = "https://services.api.unity.com/app-linking/v1/signout/completed/",
                    LoginUrl = "https://api.unity.com/v1/oauth2/authorize",
                    TokenUrl = "https://api.unity.com/v1/oauth2/token",
                    RefreshTokenUrl = "https://api.unity.com/v1/oauth2/token",
                    LogoutUrl = "https://api.unity.com/v1/oauth2/revoke",
                    SignOutUrl = "https://api.unity.com/v1/oauth2/end-session?post_logout_redirect_uri=",
                    UserInfoUrl = "https://api.unity.com/v1/users/current/openid",
                    CustomLoginParams = ""
                };
                public async Task<PkceConfiguration> GetPkceConfigurationAsync()
                {
                    return await Task.FromResult(m_PkceConfiguration);
                }
            }

            public static void Create()
            {
                var httpClient = new UnityHttpClient();
                var playerSettings = UnityCloudPlayerSettings.Instance;
                var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
                var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

                // Create new instance of CustomPkceConfigurationProvider
                var customPkceConfigurationProvider = new CustomPkceConfigurationProvider();

                var customPkceAuthenticatorSettings = new PkceAuthenticatorSettingsBuilder(platformSupport, serviceHostResolver)
                    .AddDefaultConfigurationProviderAndRequestHandler(httpClient, playerSettings)
                    .AddAppIdProvider(playerSettings)
                    // Inject customPkceConfigurationProvider in customPkceAuthenticatorSettings
                    .AddConfigurationProvider(customPkceConfigurationProvider)
                    .AddDefaultAccessTokenExchanger(httpClient)
                    .Build();

                // create an instance of PkceAuthenticator from customPkceAuthenticatorSettings
                s_PkceAuthenticator = new PkceAuthenticator(customPkceAuthenticatorSettings);
            }

            public static async Task InitializeAsync()
            {
                await s_PkceAuthenticator.InitializeAsync();
            }

            public static void ShutDownServices()
            {
                s_PkceAuthenticator = null;
            }
        }
        #endregion CustomPkceAuthenticator
    }
}







