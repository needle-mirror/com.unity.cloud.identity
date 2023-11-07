# Use case: Get user information

This section explains how to set up your scene to get information about your application's user.

Getting user information is an example of a Unity Cloud service that you can call after authenticating the user in your application. This service needs an Access Token to identify the user and authorize the call.

## Before you start

To use this sample, you must first [Integrate authentication in your scene](use-case-integrating-authentication-in-your-scene.md).

## How do I...?

### Create an IAuthenticatedUserInfoProvider and IServiceAuthorizer reference

As all `IAuthenticator` inherits the `IAuthenticatedUserInfoProvider` and `IServiceAuthorizer` interface, you simply need to
reference an `IAuthenticator`, like the `CompositeAuthenticator` as an `IAuthenticatedUserInfoProvider` or `IServiceAuthorizer` to get a reference.

    ```csharp
        static CompositeAuthenticator s_CompositeAuthenticator;
        public static ICompositeAuthenticator CompositeAuthenticator => s_CompositeAuthenticator;

        public static IAuthenticatedUserInfoProvider AuthenticatedUserInfoProvider => s_CompositeAuthenticator;
    ```

### Leverage the IAuthenticatedUserInfoProvider in your scene

To leverage the `IAuthenticatedUserInfoProvider` in your scene, see the following steps:

1. In your scene, create a `Text` field that displays the current authentication state (or the username if the user is logged in).
![Creating a text field in the scene](images/usecase2-usertext.png)

2. Create a `UserNameUpdater` script and attach it to the **LoginManager** GameObject. This script fills the text with the correct values based on the authentication state.

3. Update the `UserNameUpdater` class so it references your `Text` field, `IAuthenticator`, and `IAuthenticatedUserInfoProvider`.

   ```csharp
        public class UserNameUpdater : MonoBehaviour
        {
            [SerializeField]
            Text m_Text;

            IAuthenticator m_Authenticator;
            IAuthenticatedUserInfoProvider m_AuthenticatedUserInfoProvider;
        }
   ```

4. The `Awake` method should retrieve services from `PlatformServices`. You can then subscribe (and unsubscribe in `OnDestroy`) to the `AuthenticationStateChanged` event.

   ```csharp
        void Awake()
        {
            m_Authenticator = PlatformServices.Authenticator;
            m_AuthenticatedUserInfoProvider = PlatformServices.AuthenticatedUserInfoProvider;

            m_Authenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        void OnDestroy()
        {
            m_Authenticator.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }
   ```

5. The `OnAuthenticationStateChanged` method updates the text based on the current authentication state, and the `IAuthenticatedUserInfoProvider.GetUserInfo` method can be called when the user is logged in.

   ```csharp
       void OnAuthenticationStateChanged(AuthenticationState state)
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
                   m_Text.text = m_AuthenticatedUserInfoProvider.GetUserInfo(AuthenticatedUserInfoClaims.Name);
                   break;
           }
       }
   ```

6. Select **Play**. The text updates based on the authentication state.

### Inject an IServiceAuthorizer in ServiceHttpClient to access resources on Unity Cloud.

Since all `IAuthenticator` inherits the `IServiceAuthorizer` interface, you can inject a reference from any
`IAuthenticator` class implementation, like the `CompositeAuthenticator`, into the `ServiceHttpClient` constructor method as a valid `IServiceAuthorizer`.

The `ServiceHttpClient` can then be injected in any class of other Unity.Cloud unity packages to access resources on Unity Cloud.

```csharp
        var httpClient = new UnityHttpClient();
        var playerSettings = UnityCloudPlayerSettings.Instance;
        var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
        // Injecting the CompositeAuthenticator as an IServiceAuthorizer to build the ServiceHttpClient
        var serviceHttpClient = new ServiceHttpClient(httpClient, s_CompositeAuthenticator, playerSettings);
        // Injecting the serviceHttpClient to build an authorized IAssetRepository to retrieve IAsset, IDataset, ... from Unity Cloud.
        var assetRepository = AssetRepositoryFactory.Create(serviceHttpClient, serviceHostResolver);
   ```
