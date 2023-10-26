# Use case: Get user information

This section explains how to set up your scene to get information about your application's user.

Getting user information is an example of a Unity Cloud service that you can call after authenticating the user in your application. This service needs an Access Token to identify the user and authorize the call.

## Before you start

To use this sample, you must first [Integrate authentication in your scene](use-case-integrating-authentication-in-your-scene.md).

## How do I...?

### Instantiate a UserInfoProvider in PlatformServices

To instantiate a `UserInfoProvider` in the `PlatformServices` class, see the following steps:

1. Add the following references in the `PlatformServices` class if they're not already present:
   * A public reference to `IUserInfoProvider` and `IServiceHostResolver`
   * A private reference to `ServiceHttpClient`, `UserInfoProvider` and `IServiceHostResolver`

    ```csharp
        static ServiceHttpClient s_ServiceHttpClient;
        static UserInfoProvider s_UserInfoProvider;
        static ServiceHostResolver s_ServiceHostResolver;

        public static IUserInfoProvider UserInfoProvider => s_UserInfoProvider;
        public static IServiceHostResolver ServiceHostResolver => s_ServiceHostResolver;
        
    ```

2. Initialize the services in the `InitializeAsync` method. The [Integrate authentication in your scene guide](use-case-integrating-authentication-in-your-scene.md) includes definitions of the `s_HttpClient`, `AccessTokenProvider`, and `playerSettings` variables.

    ```csharp
        public static async Task InitializeAsync()
        {
            // ...

            s_ServiceHostResolver = UnityServiceHostResolverFactory.Create();

            s_ServiceHttpClient = new ServiceHttpClient(s_HttpClient, AccessTokenProvider, playerSettings);
            s_UserInfoProvider = new UserInfoProvider(s_ServiceHttpClient, s_ServiceHostResolver);

            // ...
        }
    ```

3. Shutdown the services in the `Shutdown` method.

    ```csharp
        public static void Shutdown()
        {
            // ...
            s_ServiceHttpClient = null;
            s_UserInfoProvider = null;
            s_ServiceHostResolver = null;
            
            // ...
        }
    ```

### Leverage the UserInfoProvider in your scene

To leverage the `UserInfoProvider` in your scene, see the following steps:

1. In your scene, create a `Text` field that displays the current authentication state (or the username if the user is logged in).
![Creating a text field in the scene](images/usecase2-usertext.png)

2. Create a `UserNameUpdater` script and attach it to the **LoginManager** GameObject. This script fills the text with the correct values based on the authentication state.

3. Update the `UserNameUpdater` class so it references your `Text` field, `IAuthenticator`, and `IUserInfoProvider`.

   ```csharp
        public class UserNameUpdater : MonoBehaviour
        {
            [SerializeField]
            Text m_Text;

            IAuthenticator m_Authenticator;
            IUserInfoProvider m_UserInfoProvider;
        }
   ```

4. The `Awake` method should retrieve services from `PlatformServices`. You can then subscribe (and unsubscribe in `OnDestroy`) to the `AuthenticationStateChanged` event.

   ```csharp
        void Awake()
        {
            m_Authenticator = PlatformServices.Authenticator;
            m_UserInfoProvider = PlatformServices.UserInfoProvider;

            m_Authenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        void OnDestroy()
        {
            m_Authenticator.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }
   ```

5. The `OnAuthenticationStateChanged` method updates the text based on the current authentication state, and the `GetUserInfoAsync` method can be called when the user is logged in.

   ```csharp
       async void OnAuthenticationStateChanged(AuthenticationState state)
       {
           switch (state)
           {
               case AuthenticationState.AwaitingLogin:
               case AuthenticationState.AwaitingLogout:
                   m_Text.text = "...";
                   break;
               case AuthenticationState.LoggedOut:
                   m_Text.text = "Logged out";
                   break;
               case AuthenticationState.LoggedIn:
                   var userInfo = await m_UserInfoProvider.GetUserInfoAsync();
                   m_Text.text = userInfo.Name;
                   break;
           }
       }
   ```

6. Select **Play**. The text updates in real time based on the authentication state.
