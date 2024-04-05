using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.Cloud.Identity.Documentation
{
    // Referenced:
    // - /Documentation~/unity-editor-authenticator.md
    #region EditorWindow
    internal class MyUnityEditorAuthenticatorExample : EditorWindow
    {
        UnityEditorAuthenticator m_UnityEditorAuthenticator;
        IUserInfoProvider m_UserInfoProvider => m_UnityEditorAuthenticator;
        IUserInfo m_UserInfo;

        // Uncomment next line to Add menu item to the Window menu
        // [MenuItem("Window/Unity Editor Authenticator Example")]
        static void Init()
        {
            MyUnityEditorAuthenticatorExample window = (MyUnityEditorAuthenticatorExample)GetWindow(typeof(MyUnityEditorAuthenticatorExample));
            window.titleContent.text = "Unity Editor Authenticator Example";
            window.Show();
        }

        async void OnEnable()
        {
            InitPlatformServices();
            await m_UnityEditorAuthenticator.InitializeAsync();
        }

        void InitPlatformServices()
        {
            m_UnityEditorAuthenticator = new UnityEditorAuthenticator();
            m_UnityEditorAuthenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
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
            if (m_UnityEditorAuthenticator.AuthenticationState.Equals(AuthenticationState.LoggedIn))
            {
                GUILayout.Label(m_UserInfo != null ? $"You are Logged in as {m_UserInfo.Name}." : "You are Logged in.");
            }
            else
            {
                GUILayout.Label(
                    m_UnityEditorAuthenticator.AuthenticationState.Equals(AuthenticationState.AwaitingInitialization)
                        ? $"Please wait, initializing..."
                        : $"You are Logged out.");
            }
        }
    }
    #endregion

    #region EditorWindowCustomTokenProvider

    internal class CustomAccessTokenProvider : IUnityEditorAccessTokenProvider
    {
        public Task<string> GetAccessTokenAsync()
        {
            return Task.FromResult("REPLACE-ME-WITH-VALID-UNITY-TOKEN");
        }
    }

    internal class MyUnityEditorAuthenticatorCustomTokenProviderExample : EditorWindow
    {
        UnityEditorAuthenticator m_UnityEditorAuthenticator;
        IUserInfoProvider m_UserInfoProvider => m_UnityEditorAuthenticator;
        IUserInfo m_UserInfo;

        bool m_InvalidCustomToken;

        // Uncomment next line to Add menu item to the Window menu
        // [MenuItem("Window/Unity Editor Authenticator with Custom Token Provider Example")]
        static void Init()
        {
            MyUnityEditorAuthenticatorCustomTokenProviderExample window = (MyUnityEditorAuthenticatorCustomTokenProviderExample)GetWindow(typeof(MyUnityEditorAuthenticatorCustomTokenProviderExample));
            window.titleContent.text = "Unity Editor Authenticator with Custom Token Provider Example";
            window.Show();
        }

        async void OnEnable()
        {
            InitPlatformServices();
            await m_UnityEditorAuthenticator.InitializeAsync();
        }

        void InitPlatformServices()
        {
            // Create a UnityEditorAuthenticator instance that uses a Custom IUnityEditorAccessTokenProvider implementation.
            m_UnityEditorAuthenticator = new UnityEditorAuthenticator(null, new CustomAccessTokenProvider());
            m_UnityEditorAuthenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
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
            try
            {

                m_UserInfo = await m_UserInfoProvider.GetUserInfoAsync();
            }
            catch (AuthenticationFailedException ex)
            {
                m_InvalidCustomToken = true;
                Debug.Log($"Cannot fetch user information using provided access token: {ex}");
            }

        }

        void OnGUI()
        {
            if (m_UnityEditorAuthenticator.AuthenticationState.Equals(AuthenticationState.LoggedIn))
            {
                if (m_InvalidCustomToken)
                {
                    GUILayout.Label("Invalid custom access token provided.");
                }
                else
                {
                    GUILayout.Label(m_UserInfo != null ? $"You are Logged in as {m_UserInfo.Name}." : "You are Logged in.");
                }
            }
            else
            {
                GUILayout.Label(
                    m_UnityEditorAuthenticator.AuthenticationState.Equals(AuthenticationState.AwaitingInitialization)
                        ? $"Please wait, initializing..."
                        : $"You are Logged out.");
            }
        }
    }
    #endregion

     #region EditorWindowLaunchArguments

    internal class MyUnityEditorAuthenticatorLaunchArgumentsExample : EditorWindow
    {
        UnityEditorAuthenticator m_UnityEditorAuthenticator;
        IUserInfoProvider m_UserInfoProvider => m_UnityEditorAuthenticator;
        IUserInfo m_UserInfo;

        bool m_InvalidCustomToken;

        // Uncomment next line to Add menu item to the Window menu
        // [MenuItem("Window/Unity Editor Authenticator from launch arguments Example")]
        static void Init()
        {
            MyUnityEditorAuthenticatorLaunchArgumentsExample window = (MyUnityEditorAuthenticatorLaunchArgumentsExample)GetWindow(typeof(MyUnityEditorAuthenticatorLaunchArgumentsExample));
            window.titleContent.text = "Unity Editor Authenticator with Custom Token Provider Example";
            window.Show();
        }

        async void OnEnable()
        {
            InitPlatformServices();
            await m_UnityEditorAuthenticator.InitializeAsync();
        }

        void InitPlatformServices()
        {
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
            var httpClient = new UnityHttpClient();
            var targetClientIdTokenToUnityServicesTokenExchanger = new TargetClientIdTokenToUnityServicesTokenExchanger(httpClient, serviceHostResolver);

            // Create a UnityEditorAuthenticator instance that uses the LaunchArgumentsUnityEditorAccessTokenProvider implementation.
            m_UnityEditorAuthenticator = new UnityEditorAuthenticator(targetClientIdTokenToUnityServicesTokenExchanger, new LaunchArgumentsUnityEditorAccessTokenProvider(serviceHostResolver));
            m_UnityEditorAuthenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
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
            try
            {

                m_UserInfo = await m_UserInfoProvider.GetUserInfoAsync();
            }
            catch (AuthenticationFailedException ex)
            {
                m_InvalidCustomToken = true;
                Debug.Log($"Cannot fetch user information using provided access token: {ex}");
            }

        }

        void OnGUI()
        {
            if (m_UnityEditorAuthenticator.AuthenticationState.Equals(AuthenticationState.LoggedIn))
            {
                if (m_InvalidCustomToken)
                {
                    GUILayout.Label("Invalid custom access token provided.");
                }
                else
                {
                    GUILayout.Label(m_UserInfo != null ? $"You are Logged in as {m_UserInfo.Name}." : "You are Logged in.");
                }
            }
            else
            {
                GUILayout.Label(
                    m_UnityEditorAuthenticator.AuthenticationState.Equals(AuthenticationState.AwaitingInitialization)
                        ? $"Please wait, initializing..."
                        : $"You are Logged out.");
            }
        }
    }
    #endregion
}
