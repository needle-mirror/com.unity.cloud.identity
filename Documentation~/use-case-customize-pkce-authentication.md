# Use case: Customize the PKCE authentication flow

This section explains how to inject your own `IPkceConfigurationProvider` implementation in the `CompositeAuthenticator`.

You can customize the returned `PkceConfiguration` to fulfill security requirements by injecting your own implementation.

## Before you start

To use this sample, you must do the following:

1. Install the [Identity package](installation.md)
2. Follow the [Get started guide](getting-started.md)
3. Follow the [Best practice | Dependency injection guide](best-practices-dependency-injection.md)
4. [Integrate authentication in your scene](use-case-integrating-authentication-in-your-scene.md) to implement the interactive user login flow

## How do I...?

### Create a CustomPkceConfigurationProvider class

To create a new `CustomPkceConfigurationProvider` class, follow these steps:

1. Open your Unity Editor Project.
2. Go to the **Assets** folder in the Project window.
3. Go to **Add (+)** > **C# Script**.
4. Rename the new script as `CustomPkceConfigurationProvider`.
5. Open the `CustomPkceConfigurationProvider` script and replace the content with the following:

    ```csharp
        using System;
        using System.Threading.Tasks;
        using Unity.Cloud.Identity;
        using Unity.Cloud.Common;

        public class CustomPkceConfigurationProvider : IPkceConfigurationProvider
        {
            readonly IAppNameProvider m_AppNameProvider;

            public CustomPkceConfigurationProvider(IAppNameProvider appNameProvider)
            {
                m_AppNameProvider = appNameProvider;
            }

            public async Task<PkceConfiguration> GetPkceConfigurationAsync()
            {
                var pkceConfiguration = new PkceConfiguration
                {
                    AppName = m_AppNameProvider.GetAppName(),
                    CacheRefreshToken = true,
                    ClientId = new ClientId("digital_twins"),
                    ProxyLoginRedirectRoute = "https://services.api.unity.com/app-linking/v1/login/redirect/",
                    ProxyLoginCompletedRoute = "https://services.api.unity.com/app-linking/v1/login/completed/",
                    ProxySignOutCompletedRoute = "https://services.api.unity.com/app-linking/v1/signout/completed/",
                    LoginUrl = $"https://api.unity.com/v1/oauth2/authorize",
                    TokenUrl = $"https://api.unity.com/v1/oauth2/token",
                    RefreshTokenUrl = $"https://api.unity.com/v1/oauth2/token",
                    LogoutUrl = $"https://api.unity.com/v1/oauth2/revoke",
                    SignOutUrl = $"https://api.unity.com/v1/oauth2/end-session?post_logout_redirect_uri=",
                    UserInfoUrl = $"https://api.unity.com/v1/users/current/openid",
                    CustomLoginParams = ""
                };
                return await Task.FromResult(pkceConfiguration);
            }
        }
    ```

### Inject the CustomPkceConfigurationProvider instance in the PkceAuthenticatorSettingsBuilder

Modify the `PlatformServices` class by adding a new instance of the `CustomPkceConfigurationProvider` in the `PkceAuthenticatorSettingsBuilder` and use the
resulting `PkceAuthenticatorSettings` to instantiate a `PkceAuthenticator`.

    ```csharp
        public static void Create()
        {
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

            // Create new instance of CustomPkceConfigurationProvider
            var pkceConfigurationProvider = new CustomPkceConfigurationProvider(playerSettings);
            
            var customPkceAuthenticatorSettings = new PkceAuthenticatorSettingsBuilder(platformSupport, serviceHostResolver)
                .AddDefaultConfigurationProviderAndRequestHandler(httpClient, playerSettings, playerSettings)
                .AddAppIdProvider(playerSettings)
                // Inject new instance of CustomPkceConfigurationProvider in custom PkceAuthenticatorSettings
                .AddConfigurationProvider(pkceConfigurationProvider)
                .AddDefaultAccessTokenExchanger(httpClient)
                .Build();

            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, serviceHostResolver, playerSettings)
                .AddDefaultBrowserAuthenticatedAccessTokenProvider(playerSettings, playerSettings)
                // Inject new instance of PkceAuthenticator created from custom PkceAuthenticatorSettings
                .AddAuthenticator(new PkceAuthenticator(customPkceAuthenticatorSettings))
                .Build();

            s_CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);
        }
    ```

### Optional | Adjust the PkceConfiguration's returned value

You can adjust the `PkceConfiguration`'s returned value after you provide your implementation of the `IPkceConfigurationProvider`. To customize the returned value to fulfill security requirements, either set the `CacheRefreshToken` value to `false` or change which PKCE configuration to use with another Identity provider.
