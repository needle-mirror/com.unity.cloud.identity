using System.Text;
using System.Threading.Tasks;
using Unity.Cloud.AppLinking.Runtime;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity.Runtime;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable S1144 // Remove the unused private method
#pragma warning disable S1481 // Remove the unused local variable

namespace Unity.Cloud.Identity.Documentation
{
    // Referenced:
    // - /Documentation~/use-case-getting-user-information.md
    namespace UserInfoProviderExample
    {
        public static class PlatformServices
        {
            public static readonly ICompositeAuthenticator CompositeAuthenticator;
        }

        public class PlatformServicesExample
        {

            #region PlatformServices
            readonly IAuthenticator m_CompositeAuthenticator;
            IServiceAuthorizer m_ServiceAuthorizer => m_CompositeAuthenticator;
            IUserInfoProvider m_UserInfoProvider => m_CompositeAuthenticator;
            IAuthenticationStateProvider m_AuthenticationStateProvider => m_CompositeAuthenticator;
            IOrganizationRepository m_OrganizationRepository => m_CompositeAuthenticator;
            #endregion PlatformServices

            public PlatformServicesExample()
            {
                var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
                var httpClient = new UnityHttpClient();
                var playerSettings = UnityCloudPlayerSettings.Instance;

                var serviceConnector = ServiceConnectorFactory.Create(platformSupport, httpClient, playerSettings, playerSettings);

                m_CompositeAuthenticator = serviceConnector.CompositeAuthenticator;
            }

            void UsePlatformServicesExample()
            {
                var platformServicesExample = new PlatformServicesExample();
                var isCompositeAuthenticatorNull = m_CompositeAuthenticator == null;
                var isUserInfoProviderNull = m_UserInfoProvider == null;
                var isAuthenticationStateProviderNull = m_AuthenticationStateProvider == null;
                var isServiceAuthorizerNull = m_ServiceAuthorizer == null;
                var isOrganizationRepositoryNull = m_OrganizationRepository == null;
            }

        }

        public class UserNameUpdater : MonoBehaviour
        {
            #region UserInfoProvider
            [SerializeField]
            Text m_UserInfoText;

            ICompositeAuthenticator m_CompositeAuthenticator;
            IAuthenticationStateProvider m_AuthenticationStateProvider => m_CompositeAuthenticator;
            IUserInfoProvider m_UserInfoProvider => m_CompositeAuthenticator;
            IUserInfo m_UserInfo;
            #endregion

            #region AwakeStartDestroy
            void Awake()
            {
                m_CompositeAuthenticator = PlatformServices.CompositeAuthenticator;
                m_AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
            }

            async Task Start()
            {
                await ApplyAuthenticationState(m_AuthenticationStateProvider.AuthenticationState);
            }

            void OnDestroy()
            {
                m_AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            }
            #endregion


            #region GetUserInfo
            async void OnAuthenticationStateChanged(AuthenticationState state)
            {
                await ApplyAuthenticationState(state);
            }

            async Task ApplyAuthenticationState(AuthenticationState state)
            {
                switch (state)
                {
                    case AuthenticationState.AwaitingInitialization:
                    case AuthenticationState.AwaitingLogout:
                    case AuthenticationState.LoggedOut:
                        m_UserInfoText.text = "...";
                        break;
                    case AuthenticationState.AwaitingLogin:
                        m_UserInfoText.text = "Awaiting completion of a user initiated manual login operation...";
                        break;
                    case AuthenticationState.LoggedIn:
                        m_UserInfo = await m_UserInfoProvider.GetUserInfoAsync();
                        BuildUserInfoText();
                        break;
                }
            }

            void BuildUserInfoText()
            {
                var sb = new StringBuilder();
                sb.Append(m_UserInfo.Name);
                sb.Append(m_CompositeAuthenticator.RequiresGUI
                    ? " is logged in with an access token issued after a successful user initiated login operation."
                    : " is logged in with an access token read from the browser local storage.");
                m_UserInfoText.text = sb.ToString();
            }
            #endregion
        }

    }
}

#pragma warning restore S1481
#pragma warning restore S1144
