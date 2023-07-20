using System;
using Unity.Cloud.Common.Runtime;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Cloud.Identity.Runtime;
using System.Collections.Generic;

namespace Unity.Cloud.Identity.Documentation
{
    #region AuthenticationState

    public class AuthenticationStateExample : MonoBehaviour
    {
        // Register the CompositeAuthenticator as an IAuthenticationStateProvider
        IAuthenticationStateProvider m_AuthenticationStateProvider => m_CompositeAuthenticator;
        CompositeAuthenticator m_CompositeAuthenticator;

        void Awake()
        {
            // Build a CompositeAuthenticator supporting different runtime authentication flow
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var httpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, PlatformSupportFactory.GetAuthenticationPlatformSupport(), serviceHostResolver)
                .AddDefaultBrowserAuthenticatedAccessTokenProvider()
                .AddDefaultPersonalAccessTokenProvider()
                .AddDefaultPkceAuthenticator(playerSettings)
                .Build();

            m_CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

            // Listen from AuthenticationStateChanged event
            m_AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        async Task Start()
        {
            // Initialize the CompositeAuthenticator to get its initial AuthenticationState
            await m_CompositeAuthenticator.InitializeAsync();

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
                    // Awaiting initial AuthenticationState at startup.
                    break;
                case AuthenticationState.AwaitingLogin:
                    // A manual login is pending completion
                    break;
                case AuthenticationState.LoggedIn:
                    // An access token is ready to use on Cloud Services
                    break;
                case AuthenticationState.AwaitingLogout:
                    // A manual logout is pending completion
                    break;
                case AuthenticationState.LoggedOut:
                    // No access token available
                    break;
            }
        }
    }

    #endregion
}
