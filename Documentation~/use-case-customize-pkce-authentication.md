# Customize the PKCE authentication flow

This section explains how to inject your own `IPkceConfigurationProvider` implementation in the `CompositeAuthenticator`.

You can customize the returned `PkceConfiguration` to fulfill security requirements by injecting your own implementation.

## Prerequisites

To use this sample, do the following:

1. Install the [Identity package](installation.md)
2. Follow the [Get started guide](getting-started.md)
3. Follow the [Best practice | Dependency injection guide](best-practices-dependency-injection.md)
4. [Integrate authentication in your scene](use-case-integrating-authentication-in-your-scene.md) to implement the interactive user login flow

## Overview

To inject an `IPkceConfigurationProvider` implementation in the `CompositeAuthenticator`, perform the following procedures:

1. Create a `CustomPkceConfigurationProvider` class
2. Inject your `CustomPkceConfigurationProvider` instance in the `CompositeAuthenticator` constructor parameters
3. Optional | Adjust the `PkceConfiguration`'s returned value

### Create a CustomPkceConfigurationProvider class

To create a new `CustomPkceConfigurationProvider` class, follow these steps:

1. Open your Unity project.
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
                    AllowAnonymous = false,
                    CacheRefreshToken = true,
                    ClientId = "digital_twins",
                    LoginUrl = "https://api.unity.com/v1/oauth2/authorize",
                    TokenUrl = "https://dt.unity.com/api/auth/token/refresh",
                    RefreshTokenUrl = "https://dt.unity.com/api/auth/token/refresh",
                    LogoutUrl = "https://dt.unity.com/api/auth/token/revoke",
                    CustomLoginParams = ""
                };
                return await Task.FromResult(pkceConfiguration);
            }
        }
    ```

### Inject your CustomPkceConfigurationProvider instance in the CompositeAuthenticator constructor parameters

Modify the `PlatformServices` class by adding the `CustomPkceConfigurationProvider` script in the `CompositeAuthenticator` constructor parameters.

    ```csharp
        public static async Task InitializeAsync()
        {
            var playerSettings = UnityCloudPlayerSettings.Instance;
            s_HttpClient = new UnityHttpClient();

            var pkceConfigurationProvider = new CustomPkceConfigurationProvider(playerSettings);

            s_CompositeAuthenticator = new CompositeAuthenticator(s_HttpClient, playerSettings, playerSettings, pkceConfigurationProvider);

            await s_CompositeAuthenticator.InitializeAsync();
        }
    ```

### Optional | Adjust the PkceConfiguration's returned value

You can adjust the `PkceConfiguration`'s returned value after you provide your implementation of the `IPkceConfigurationProvider`. To customize the returned value to fulfill security requirements, either set the `CacheRefreshToken` value to `false` or change which PKCE configuration to use with another Identity provider.
