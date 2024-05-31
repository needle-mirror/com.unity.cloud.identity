using System;
using System.Threading.Tasks;
using Unity.Cloud.Identity.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.Cloud.Identity.Documentation
{
    // Referenced:
    // - /Documentation~/unity-editor-service-authorizer.md
    #region EditorWindow
    internal class MyUnityEditorServiceAuthorizerExample : EditorWindow
    {
        AuthenticationState m_AuthenticationState;

        IUserInfoProvider m_UserInfoProvider => UnityEditorServiceAuthorizer.instance;
        IUserInfo m_UserInfo;

        // Uncomment next line to Add menu item to the Window menu
        // [MenuItem("Window/Unity Editor Service Authorizer Example")]
        static void Init()
        {
            MyUnityEditorServiceAuthorizerExample window = (MyUnityEditorServiceAuthorizerExample)GetWindow(typeof(MyUnityEditorServiceAuthorizerExample));
            window.titleContent.text = "Unity Editor Authenticator Example";
            window.Show();
        }

        void OnEnable()
        {
            UnityEditorServiceAuthorizer.instance.AuthenticationStateChanged += OnAuthenticationStateChanged;
            OnAuthenticationStateChanged(UnityEditorServiceAuthorizer.instance.AuthenticationState);
        }

        async void OnAuthenticationStateChanged(AuthenticationState authenticationState)
        {
            switch (authenticationState)
            {
                case AuthenticationState.LoggedIn:
                    await GetUserInfoAsync();
                    break;
                case AuthenticationState.AwaitingInitialization:
                case AuthenticationState.AwaitingLogin:
                case AuthenticationState.AwaitingLogout:
                case AuthenticationState.LoggedOut:
                    break;
            }
            Repaint();
        }

        async Task GetUserInfoAsync()
        {
            m_UserInfo = await m_UserInfoProvider.GetUserInfoAsync();
        }

        void OnGUI()
        {
            if (UnityEditorServiceAuthorizer.instance.AuthenticationState.Equals(AuthenticationState.LoggedIn))
            {
                GUILayout.Label(m_UserInfo != null ? $"You are Logged in as {m_UserInfo.Name}." : "You are Logged in.");
            }
            else
            {
                GUILayout.Label(
                    UnityEditorServiceAuthorizer.instance.AuthenticationState.Equals(AuthenticationState.AwaitingInitialization)
                        ? $"Please wait, initializing..."
                        : $"You are Logged out.");
            }
        }
    }
    #endregion
}
