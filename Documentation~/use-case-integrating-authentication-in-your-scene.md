# Use case: Integrate authentication in your scene

This section explains how to set up your scene to integrate an authentication layer to communicate with Unity Cloud services.

## Before you start

* Follow the [Installation instructions](installation.md).
* Follow the [Get started instructions](getting-started.md).
* Follow the [Best practices: dependency injection guide](best-practices-dependency-injection.md).

## How do I...?

### Instantiate an ICompositeAuthenticator in PlatformServices

To instantiate an `ICompositeAuthenticator` interface in the `PlatformServices` class, follow these steps:

1. Create an `IAuthenticationPlaformSupport`, and an `IHttpClient` interface, then get the references to an `IAppIdProvider` and `IAppNamespaceProvider` interfaces from the `UnityCloudPlayerSettings.Instance` singleton.
1. Inject them in the `ServiceConnectorFactory.Create` method to build an `ICompositeAuthenticator`.
1. Add the following references to the created instance of the <code>ICompositeAuthenticator</code>:
   1. A private reference
   1. A public reference
1. Initialize the `ICompositeAuthenticator` and any other services in the `InitializeAsync` method.
1. Shutdown the `ICompositeAuthenticator` and any other services in the `Shutdown` method.

[!code-cs [PlatformServices](../Samples/Documentation/Manual/IntegrateAuthenticationExample.cs#PlatformServices)]

### Tie the ICompositeAuthenticator with UI

If your application needs to support the **interactive login flow**, create a login UI and link it to the `IUrlRedirectionAuthenticator` methods to achieve interactive login and logout.

>[!NOTE]
>The `ICompositeAuthenticator` interface that was created using the `ServiceConnectorFactory.Create` method can activate the **interactive flow** or the **service account flow**. So, it's not guaranteed that the activated flow is the interactive one. This means that some parts of the UI need to be hidden or disabled from code depending on the situation. Validate this by checking the value of the `ICompositeAuthenticator.RequiresGUI` property when managing a UI element.

1. Create <b>Login</b> and <b>Logout</b> buttons in your scene.

   ![Creating buttons in the scene](images/usecase1-buttons.png)

1. Create a **LoginManager** GameObject and attach a new **LoginManager** monobehavior to it. This script links your UI with Identity's authentication engine.
1. Update the `LoginManager` script so it references your buttons and an `IAuthenticationStateProvider` interface.
1. Update the `Awake` method to do the following:
   1. Retrieve the `ICompositeAuthenticator` reference from `PlatformServices`.
   1. Subscribe to the `AuthenticationStateChanged` event. 
   1. Subscribe to the buttons' `onClick` events. 
   1. Call `ApplyAuthenticationState` to update the UI when the scene loads. 
1. Make sure the subscriptions are cleaned up in `OnDestroy`.
1. Implement the `ApplyAuthenticationState` method to update your buttons based on the current authentication state. Make your buttons interactive in specific circumstances, otherwise you risk errors and exceptions. To determine if you should display your buttons, rely on the `AuthenticationState` and the `RequiresGUI` property that determines whether the UI is relevant in the selected authentication flow.
1. Define the behaviors for your buttons by calling the appropriate methods.

   >[!NOTE]
   >Only an interactive login flow, like the one exposed by the `IUrlRedirectionAuthenticator` interface, requires these methods. If the flow isn't interactive, calling `LoginAsync` or `LogoutAsync` throws exceptions.

1. Test your scene to see how the buttons update along with the authentication state of the user.

[!code-cs [LoginManager](../Samples/Documentation/Manual/IntegrateAuthenticationExample.cs#LoginManager)]

### Leverage the authorization headers to communicate with other Unity Cloud services

After you set up your login flow, communication with other Unity Cloud services is possible. All Unity Cloud services expect an instance of `IServiceAuthorizer` to apply authorization information to HTTP requests, which the `PlatformServices` class provides.

See an [example of a service that expects an `IServiceAuthorizer`](use-case-getting-user-information.md).
