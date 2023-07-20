using System;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Cloud.Identity.Runtime;
using Unity.Cloud.Common.Runtime;

namespace Unity.Cloud.Identity.Documentation
{
    #region PkceAuthenticator

    public class PkceAuthenticatorExample : MonoBehaviour
    {
        IAuthenticationStateProvider m_AuthenticationStateProvider => m_PkceAuthenticator;
        IAuthenticator m_PkceAuthenticator;

        void Awake()
        {
            var httpClient = new UnityHttpClient();
            var authenticationPlatformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
            var playerSettings = UnityCloudPlayerSettings.Instance;

            var pkceAuthenticatorSettingsBuilder = new PkceAuthenticatorSettingsBuilder(authenticationPlatformSupport, serviceHostResolver);
            pkceAuthenticatorSettingsBuilder.AddDefaultConfigurationProviderAndRequestHandler(httpClient, playerSettings)
                                            .AddDefaultAccessTokenExchanger(httpClient);

            var pkceAuthenticatorSettings = pkceAuthenticatorSettingsBuilder.Build();

            m_PkceAuthenticator = new PkceAuthenticator(pkceAuthenticatorSettings);

            m_AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        async Task Start()
        {
            await m_PkceAuthenticator.InitializeAsync();

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
                    // Initial await time to retrieve an access token from cache, if allowed and present.
                    break;
                case AuthenticationState.AwaitingLogin:
                    // User initiated the logging operation in the browser
                    break;
                case AuthenticationState.LoggedIn:
                    // The access token provided by the PKCE token endpoint is ready to use
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
