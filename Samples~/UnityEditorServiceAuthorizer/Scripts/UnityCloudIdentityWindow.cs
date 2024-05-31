using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Unity.Cloud.Identity.Editor.Samples
{
    public class UnityCloudIdentityWindow : EditorWindow
    {
        AuthenticationState m_AuthenticationState;

        IUserInfoProvider m_UserInfoProvider => UnityEditorServiceAuthorizer.instance;

        string UserInfoName;

        // Add menu named "UnityEditorServiceAuthorizer Sample" to the Window menu
        [MenuItem("Unity Cloud/Samples/UnityEditorServiceAuthorizer Sample")]
        static void Init()
        {
            // Get existing open window or if none, make a new one
            UnityCloudIdentityWindow window = (UnityCloudIdentityWindow)EditorWindow.GetWindow(typeof(UnityCloudIdentityWindow));
            window.titleContent.text = "UnityEditorServiceAuthorizer Sample";
            window.Show();
        }

        void OnEnable()
        {
            UnityEditorServiceAuthorizer.instance.AuthenticationStateChanged += OnAuthenticationStateChanged;
            OnAuthenticationStateChanged(UnityEditorServiceAuthorizer.instance.AuthenticationState);
        }

        private void OnDisable()
        {
            UnityEditorServiceAuthorizer.instance.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        async void OnAuthenticationStateChanged(AuthenticationState authenticationState)
        {
            m_AuthenticationState = authenticationState;
            var loggedIn = authenticationState.Equals(AuthenticationState.LoggedIn);
            if (loggedIn && string.IsNullOrEmpty(UserInfoName))
                await GetUserInfoAsync();

            if (authenticationState.Equals(AuthenticationState.LoggedOut))
            {
                UserInfoName = string.Empty;
            }
            Repaint();
        }

        async Task GetUserInfoAsync()
        {
            var userInfo = await m_UserInfoProvider.GetUserInfoAsync();
            UserInfoName = userInfo.Name;
        }

        void OnGUI()
        {
            switch (m_AuthenticationState)
            {
                case AuthenticationState.AwaitingInitialization:
                    GUILayout.Label( "Initialization...");
                    break;
                case AuthenticationState.AwaitingLogin:
                    GUILayout.Label( "Awaiting login...");
                    break;
                case AuthenticationState.LoggedIn:
                    GUILayout.Label(!string.IsNullOrEmpty(UserInfoName) ? $"You are Logged in as {UserInfoName}." : "You are Logged in.");
                    break;
                case AuthenticationState.AwaitingLogout:
                case AuthenticationState.LoggedOut:
                    GUILayout.Label($"You are Logged out.");
                    break;
            }
        }
    }
}

