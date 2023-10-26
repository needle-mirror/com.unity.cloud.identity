using System;
using Unity.Cloud.Common.Runtime;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Cloud.Identity.Runtime;
using System.Collections.Generic;

namespace Unity.Cloud.Identity.Documentation
{
    #region AuthenticationStateChanged

    public class AuthenticationStateProviderExample : MonoBehaviour
    {
        IAuthenticationStateProvider m_AuthenticationStateProvider => m_CompositeAuthenticator;
        CompositeAuthenticator m_CompositeAuthenticator;

        void Awake()
        {
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var httpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, PlatformSupportFactory.GetAuthenticationPlatformSupport(), serviceHostResolver, playerSettings)
                .AddDefaultBrowserAuthenticatedAccessTokenProvider(playerSettings, playerSettings)
                .AddDefaultPkceAuthenticator(playerSettings, playerSettings)
                .Build();

            m_CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

            m_AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        async Task Start()
        {
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
                    break;
                case AuthenticationState.AwaitingLogin:
                    break;
                case AuthenticationState.LoggedIn:
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
