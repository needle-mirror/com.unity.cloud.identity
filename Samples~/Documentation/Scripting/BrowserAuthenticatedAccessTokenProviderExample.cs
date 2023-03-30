using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
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
            var authenticationPlatformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();

            var localStorageKeyNames = new Dictionary<string, string>() { { "*", "genesis-access-token" } };

            m_BrowserAuthenticatedAccessTokenProvider = new BrowserAuthenticatedAccessTokenProvider(authenticationPlatformSupport, localStorageKeyNames);
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
