#if !UC_EXCLUDE_SAMPLES
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Unity.Cloud.Identity.Samples
{
    using System.Linq;

    public class ActiveUserController : MonoBehaviour
    {
        [SerializeField]
        Button m_LoginButton;

        [SerializeField]
        Button m_CancelLoginButton;

        [SerializeField]
        Button m_LogoutButton;

        [SerializeField]
        Button m_SignOutButton;

        [SerializeField]
        UIController m_UIController;

        [SerializeField]
        Text m_UserNameText;

        ICompositeAuthenticator m_CompositeAuthenticator;
        IUserInfoProvider m_UserInfoProvider;

        [SerializeField]
        UnityEvent m_UserUnauthorized;

        void Start()
        {
            RegisterButtons();

            if (m_CompositeAuthenticator == null)
            {
                m_CompositeAuthenticator = PlatformServices.CompositeAuthenticator;
                m_UserInfoProvider = PlatformServices.UserInfoProvider;

                m_CompositeAuthenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;

                // Update UI with current state
                _ = ApplyAuthenticationState(m_CompositeAuthenticator.AuthenticationState);
            }

        }

        void OnDestroy()
        {
            UnregisterButtons();
            m_CompositeAuthenticator.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        public void Login()
        {
            try
            {
                m_CompositeAuthenticator.LoginAsync();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException
                    or AuthenticationFailedException)
                {
                    Debug.LogError(ex.Message);
                }
                throw;
            }
        }

        public void CancelLogin()
        {
            try
            {
                m_CompositeAuthenticator.CancelLogin();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                {
                    Debug.LogError(ex.Message);
                }
                throw;
            }
        }

        public void Logout()
        {
            try
            {
                m_CompositeAuthenticator.LogoutAsync();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException
                    or AuthenticationFailedException)
                {
                    Debug.LogError(ex.Message);
                }
                throw;
            }
        }

        void SignOut()
        {
            try
            {
                m_CompositeAuthenticator.LogoutAsync(true);
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException
                    or AuthenticationFailedException)
                {
                    Debug.LogError(ex.Message);
                }
                throw;
            }
        }

        void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
        {
            _ = ApplyAuthenticationState(newAuthenticationState);
        }

        async Task ApplyAuthenticationState(AuthenticationState state)
        {
            // Clear status text on authentication change
            m_UserNameText.text = string.Empty;

            switch (state)
            {
                case AuthenticationState.AwaitingInitialization:
                case AuthenticationState.AwaitingLogin:
                case AuthenticationState.AwaitingLogout:
                    UpdateButton(m_LoginButton, false);
                    UpdateButton(m_LogoutButton, false);
                    UpdateButton(m_SignOutButton, false);
                    break;
                case AuthenticationState.LoggedIn:
                    UpdateButton(m_LoginButton, false);
                    UpdateButton(m_LogoutButton, m_CompositeAuthenticator.RequiresGUI);
                    UpdateButton(m_SignOutButton, m_CompositeAuthenticator.RequiresGUI);
                    m_UserNameText.text = await GetUserInfo();
                    break;
                case AuthenticationState.LoggedOut:
                    UpdateButton(m_LoginButton, m_CompositeAuthenticator.RequiresGUI);
                    UpdateButton(m_LogoutButton, false);
                    UpdateButton(m_SignOutButton, false);
                    m_UserNameText.text = "No User";
                    break;
            }
        }

        async Task<string> GetUserInfo()
        {
            try
            {
                var userInfo = await m_UserInfoProvider.GetUserInfoAsync();
                var userNameText = userInfo != null ? userInfo.Name : "No User";
                return userNameText;
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException
                    or UnauthorizedException
                    or ConnectionException
                    or ForbiddenException)
                {
                    Debug.LogError(ex.Message);

                    if (ex is UnauthorizedException)
                        m_UserUnauthorized?.Invoke();
                }
                throw;
            }
        }

        static void UpdateButton(Button button, bool enabled)
        {
            if (button != null)
                button.interactable = enabled;
        }

        void RegisterButtons()
        {
            if (m_LoginButton != null)
                m_LoginButton.onClick.AddListener(Login);
            if(m_CancelLoginButton != null)
                m_CancelLoginButton.onClick.AddListener(CancelLogin);
            if (m_LogoutButton != null)
                m_LogoutButton.onClick.AddListener(Logout);
            if (m_SignOutButton != null)
                m_SignOutButton.onClick.AddListener(SignOut);
        }

        void UnregisterButtons()
        {
            if (m_LoginButton != null)
                m_LoginButton.onClick.RemoveListener(Login);
            if(m_CancelLoginButton != null)
                m_CancelLoginButton.onClick.RemoveListener(CancelLogin);
            if (m_LogoutButton != null)
                m_LogoutButton.onClick.RemoveListener(Logout);
            if (m_SignOutButton != null)
                m_SignOutButton.onClick.RemoveListener(SignOut);

        }

    }
}
#endif
