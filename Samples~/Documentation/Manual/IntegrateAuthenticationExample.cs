using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.AppLinking.Runtime;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Cloud.Identity.Documentation
{
    // Referenced:
    // - /Documentation~/use-case-integrating-authentication-in-your-scene.md
    namespace IntegrateAuthenticationExample
    {
        #region PlatformServices
    public static class PlatformServices
    {
        static ICompositeAuthenticator s_CompositeAuthenticator;

        public static ICompositeAuthenticator CompositeAuthenticator => s_CompositeAuthenticator;

        public static async Task InitializeAsync()
        {
            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;

            var serviceConnector = ServiceConnectorFactory.Create(platformSupport, httpClient, playerSettings, playerSettings);

            s_CompositeAuthenticator = serviceConnector.CompositeAuthenticator;

            await s_CompositeAuthenticator.InitializeAsync();
        }

        public static void Shutdown()
        {
            (s_CompositeAuthenticator as IDisposable)?.Dispose();
            s_CompositeAuthenticator = null;
        }

     }
        #endregion

        #region LoginManager
        public class LoginManager : MonoBehaviour
        {
            [SerializeField]
            Button m_LoginButton;

            [SerializeField]
            Button m_LogoutButton;

            ICompositeAuthenticator m_CompositeAuthenticator;

            void Awake()
            {
                m_CompositeAuthenticator = PlatformServices.CompositeAuthenticator;
                m_CompositeAuthenticator.AuthenticationStateChanged += ApplyAuthenticationState;

                m_LoginButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OnLoginButtonClick));
                m_LogoutButton.onClick.AddListener(new UnityEngine.Events.UnityAction(OnLogoutButtonClick));
            }

            void Start()
            {
                ApplyAuthenticationState(m_CompositeAuthenticator.AuthenticationState);
            }

            void OnDestroy()
            {
                m_CompositeAuthenticator.AuthenticationStateChanged -= ApplyAuthenticationState;

                m_LoginButton.onClick.RemoveAllListeners();
                m_LogoutButton.onClick.RemoveAllListeners();
            }

            void ApplyAuthenticationState(AuthenticationState state)
            {
                switch (state)
                {
                    case AuthenticationState.LoggedOut:
                        m_LoginButton.interactable = m_CompositeAuthenticator.RequiresGUI;
                        m_LogoutButton.interactable = false;
                        break;
                    case AuthenticationState.LoggedIn:
                        m_LoginButton.interactable = false;
                        m_LogoutButton.interactable = m_CompositeAuthenticator.RequiresGUI;
                        break;
                    case AuthenticationState.AwaitingInitialization:
                    case AuthenticationState.AwaitingLogin:
                    case AuthenticationState.AwaitingLogout:
                        m_LoginButton.interactable = false;
                        m_LogoutButton.interactable = false;
                        break;
                }
            }

            async void OnLoginButtonClick()
            {
                await m_CompositeAuthenticator.LoginAsync();
            }

            async void OnLogoutButtonClick()
            {
                await m_CompositeAuthenticator.LogoutAsync();
            }
        }
        #endregion

    }
}
