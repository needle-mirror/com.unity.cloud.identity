using System;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Cloud.Identity.Runtime;

namespace Unity.Cloud.Identity.Documentation
{
    #region PersonalAccessTokenProvider

    public class PersonalAccessTokenProviderExample : MonoBehaviour
    {
        IAuthenticationStateProvider m_AuthenticationStateProvider => m_PersonalAccessTokenProvider;
        IAuthenticator m_PersonalAccessTokenProvider;

        void Awake()
        {
            var authenticationPlatformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            m_PersonalAccessTokenProvider = new PersonalAccessTokenProvider(authenticationPlatformSupport);
            m_AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        async Task Start()
        {
            await m_PersonalAccessTokenProvider.InitializeAsync();

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
                    // Initial await time to retrieve the injected personal access token
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
