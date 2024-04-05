using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Cloud.Identity.Samples.Authenticate
{
    /// <summary>
    /// A Monobehaviour class to fetch user information using platform services.
    /// </summary>
    public class AuthenticationStateUpdater : MonoBehaviour
    {
        [SerializeField]
        Text m_AuthenticationStateInfoText;

        IAuthenticator m_CompositeAuthenticator;
        IAuthenticationStateProvider m_AuthenticationStateProvider => m_CompositeAuthenticator;

        void Awake()
        {
            m_CompositeAuthenticator = PlatformServices.CompositeAuthenticator;
            m_AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        void Start()
        {
            // Update UI with current state
            ApplyAuthenticationState(m_AuthenticationStateProvider.AuthenticationState);
        }

        void OnDestroy()
        {
            m_AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        void OnAuthenticationStateChanged(AuthenticationState state)
        {
            ApplyAuthenticationState(state);
        }

        void ApplyAuthenticationState(AuthenticationState state)
        {
            switch (state)
            {
                case AuthenticationState.AwaitingInitialization:
                    m_AuthenticationStateInfoText.text = "AuthenticationState: AwaitingInitialization";
                    break;
                case AuthenticationState.AwaitingLogout:
                    m_AuthenticationStateInfoText.text = "AuthenticationState: AwaitingLogout";
                    break;
                case AuthenticationState.LoggedOut:
                    m_AuthenticationStateInfoText.text = "AuthenticationState: LoggedOut.\n\nNo Access Token available to connect to Unity Cloud Services.";
                    break;
                case AuthenticationState.AwaitingLogin:
                    m_AuthenticationStateInfoText.text = "AuthenticationState: AwaitingLogin.\n\nAwaiting completion of a user initiated manual login operation...";
                    break;
                case AuthenticationState.LoggedIn:
                    m_AuthenticationStateInfoText.text = "AuthenticationState: LoggedIn.\n\nAn Access Token is available to connect to Unity Cloud Services.";
                    break;
            }
        }

        /// <summary>
        /// Action to display unauthorized message to user.
        /// </summary>
        public void OnUserUnauthorized()
        {
            m_AuthenticationStateInfoText.text = "User Unauthorized:\n\nThe provided access token threw an Unauthorized exception - please ensure you are using a valid token.";
        }
    }
}
