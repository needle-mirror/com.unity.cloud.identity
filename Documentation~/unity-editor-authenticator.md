# Unity Editor Authenticator

The `UnityEditorAuthenticator` class is an `IAuthenticator` implementation to be used only in the context of Unity Editor scripting.

It relies on the running Unity Editor's user session to provide `IAuthenticationStateProvider.AuthenticationStateChanged` event and all `IOrganizationRepository` methods
to fetch Unity Organizations and Unity Projects of the logged in user and its assigned roles and permissions in them.

## UnityEditorAuthenticator usage

Here is an example of instanciation of the `UnityEditorAuthenticator` in a `UnityEditor.EditorWindow` derived class:

```csharp
    internal class MyEditorWindow : EditorWindow
    {
        UnityEditorAuthenticator m_UnityEditorAuthenticator;
        IAuthenticatedUserInfoProvider m_AuthenticatedUserInfoProvider => m_UnityEditorAuthenticator;
        bool m_LoggedIn;

        [MenuItem("Window/My Window")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            MyEditorWindow window = (MyEditorWindow)EditorWindow.GetWindow(typeof(MyEditorWindow));
            window.titleContent.text = "My Window";
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

            m_UnityEditorAuthenticator = new UnityEditorAuthenticator(targetClientIdTokenToUnityServicesTokenExchanger);
            m_UnityEditorAuthenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        void OnAuthenticationStateChanged(AuthenticationState authenticationState)
        {
            m_LoggedIn = m_UnityEditorAuthenticator.AuthenticationState.Equals(AuthenticationState.LoggedIn);
            Repaint();
        }

        void OnGUI()
        {
            if (m_LoggedIn)
            {
                GUILayout.Label($"You are Logged in as {m_AuthenticatedUserInfoProvider.GetUserInfo(AuthenticatedUserInfoClaims.Name)}.");
            }
            else
            {
                GUILayout.Label($"You are Logged out.");
            }
        }
    }
```
