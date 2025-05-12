# Use case: Get user information

This section explains how to set up your scene to get information about your application's user.

Getting user information is an example of a Unity Cloud service that you can call after authenticating the user in your application. This service needs an access token to identify the user and authorize the call.

## Before you start

Before you use this sample, [integrate authentication in your scene](use-case-integrating-authentication-in-your-scene.md).

## How do I...?

### Create references to IAuthenticator-inherited interfaces

The `CompositeAuthenticator` class, like all `IAuthenticator` implementations, inherits the `IServiceAuthorizer`, `IAuthenticationStateProvider`, `IUserInfoProvider`, and `IOrganizationRepository` interfaces.
Create an instance of the `CompositeAuthenticator` and use it to reference all inherited interfaces.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCase/UserInfoProviderExample.cs#PlatformServices)]

### Leverage the IUserInfoProvider in your scene

To leverage the `IUserInfoProvider` in your scene, follow these steps:

1. In your scene, create a `Text` field that displays the current authentication state (or the username if the user is logged in).
![Creating a text field in the scene](images/usecase2-usertext.png)

2. Create a `UserNameUpdater` script and attach it to the **LoginManager** GameObject. This script fills the text with the correct values based on the authentication state.

3. Update the `UserNameUpdater` class so it references your `Text` field, `IAuthenticator`, and `IUserInfoProvider`.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCase/UserInfoProviderExample.cs#UserInfoProvider)]

4. The `Awake` and `Destroy` method manages `PlatformServices` references and events. The async `Start` method applies the initial authentication state.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCase/UserInfoProviderExample.cs#AwakeStartDestroy)]

5. The `OnAuthenticationStateChanged` method updates the text based on the current authentication state, and the `IUserInfoProvider.GetUserInfoAsync` method can be called when the user is logged in.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCase/UserInfoProviderExample.cs#GetUserInfo)]

6. Select **Play**. The text updates based on the authentication state.

### Inject an IServiceAuthorizer in ServiceHttpClient to access resources on Unity Cloud

Because all `IAuthenticator` implentations inherit the `IServiceAuthorizer` interface, you can inject a reference from any
`IAuthenticator` class implementation, like the `ServiceAccountAuthenticator`, into the `ServiceHttpClient` constructor method as a valid `IServiceAuthorizer`.

You can inject the `ServiceHttpClient` then in any class of other `Unity.Cloud` Unity packages to access resources on Unity Cloud.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/ServiceAuthorizerExample.cs#ServiceAuthorizer)]
