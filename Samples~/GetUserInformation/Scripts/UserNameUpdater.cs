using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Cloud.Identity.Samples.GetUserInfo
{
    /// <summary>
    /// A Monobehaviour class to fetch user information using platform services.
    /// </summary>
    public class UserNameUpdater : MonoBehaviour
    {
        [SerializeField]
        Text m_UserInfoText;

        IAuthenticationStateProvider m_AuthenticationStateProvider;
        ICompositeAuthenticator m_CompositeAuthenticator;
        IUserInfoProvider m_UserInfoProvider;

        void Awake()
        {
            m_AuthenticationStateProvider = PlatformServices.AuthenticationStateProvider;
            m_CompositeAuthenticator = PlatformServices.CompositeAuthenticator;
            m_UserInfoProvider = PlatformServices.UserInfoProvider;

            m_AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        async Task Start()
        {
            // Update UI with current state
            await ApplyAuthenticationState(m_AuthenticationStateProvider.AuthenticationState);
        }


        void OnDestroy()
        {
            m_AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        void OnAuthenticationStateChanged(AuthenticationState state)
        {
            _ = ApplyAuthenticationState(state);
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
                    var userInfo = await m_UserInfoProvider.GetUserInfoAsync();
                    m_UserInfoText.text = BuildUserInfoText(userInfo);
                    break;
            }
        }

        string BuildUserInfoText(UserInfo userInfo)
        {
            var sb = new StringBuilder();
            sb.Append(userInfo.Name);
            if (m_CompositeAuthenticator.RequiresGUI)
            {
                sb.Append(" is logged in with an access token issued after a successful user initiated login operation.");
            }
            else
            {
                sb.Append(" logged in with an access token coming from an environment variable, a browser local storage or injected as a launch argument to the current process.");
            }
            return sb.ToString();
        }
    }
}
