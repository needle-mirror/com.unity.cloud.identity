using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Cloud.AppLinking.Runtime;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity.Runtime;

namespace Unity.Cloud.Identity.Documentation
{
    #region BrowserAuthenticatedAccessTokenProvider

    public class BrowserAuthenticatedAccessTokenProviderExample : MonoBehaviour
    {
        IAuthenticationStateProvider m_AuthenticationStateProvider => m_BrowserAuthenticatedAccessTokenProvider;
        IAuthenticator m_BrowserAuthenticatedAccessTokenProvider;

        void Awake()
        {
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var authenticationPlatformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

            var localStorageKeyNames = new Dictionary<string, string>() { { "dashboard.unity3d.com", "genesis-access-token" } };

            var pkceAuthenticatorSettingsBuilder = new PkceAuthenticatorSettingsBuilder(authenticationPlatformSupport, serviceHostResolver);
            pkceAuthenticatorSettingsBuilder.AddDefaultConfigurationProviderAndRequestHandler(httpClient, playerSettings)
                .AddDefaultAccessTokenExchanger(httpClient);

            var pkceAuthenticatorSettings = pkceAuthenticatorSettingsBuilder.Build();

            m_BrowserAuthenticatedAccessTokenProvider = new BrowserAuthenticatedAccessTokenProvider(pkceAuthenticatorSettings, localStorageKeyNames);
            m_AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        async Task Start()
        {
            await m_BrowserAuthenticatedAccessTokenProvider.InitializeAsync();

            // After initialize, Update UI with current state
            ApplyAuthenticationState(m_AuthenticationStateProvider.AuthenticationState);
        }

        void OnDisable()
        {
            m_AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        void OnAuthenticationStateChanged(AuthenticationState state)
        {
            ApplyAuthenticationState(state);
        }

        void ApplyAuthenticationState(AuthenticationState newAuthenticationState)
        {
            switch (newAuthenticationState)
            {
                case AuthenticationState.AwaitingInitialization:
                    // Initial await time to retrieve the access token from the browser
                    break;
                case AuthenticationState.AwaitingLogin:
                    break;
                case AuthenticationState.LoggedIn:
                    // The access token provided is ready to use
                    break;
                case AuthenticationState.AwaitingLogout:
                    break;
                case AuthenticationState.LoggedOut:
                    break;
            }
        }
    }

    #endregion

}
