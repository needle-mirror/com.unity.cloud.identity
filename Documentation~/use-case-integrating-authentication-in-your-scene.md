# Use case: Integrate authentication in your scene

This section explains how to set up your scene to integrate an authentication layer to communicate with Unity Cloud services.

## Before you start

* Follow the [Installation instructions](installation.md).
* Follow the [Get started instructions](getting-started.md).
* Follow the [Best practices: dependency injection guide](best-practices-dependency-injection.md).

## How do I...?

### Instantiate a CompositeAuthenticator in PlatformServices

To instantiate a `CompositeAuthenticator` in the `PlatformServices` class, follow these steps:

<ol>
    <li>Use the <code>CompositeAuthenticatorSettingsBuilder</code> to build a prioritized list of supported <code>IAuthenticator</code>.</li>
    <li>Create an instance of the <code>CompositeAuthenticator</code> using the <code>CompositeAuthenticatorSettings</code> as argument.</li>
    <li> Add the following references of the created instance of the <code>CompositeAuthenticator</code>: </li>
        <ol type="a">
            <li> A private read-only reference </li>
            <li> A public reference as <code>ICompositeAuthenticator</code>, <code>IAuthenticationStateProvider</code> and <code>IAccessTokenProvider</code></li>
        </ol>
    <li> Initialize the <code>CompositeAuthenticator</code> and any other services in the <code>InitializeAsync</code> method. </li>
    <li> Shutdown the <code>CompositeAuthenticator</code> and any other services in the <code>Shutdown</code> method. </li>
</ol>

[!code-cs [PlatformServices](../Samples/Documentation/Manual/IntegrateAuthenticationExample.cs#PlatformServices)]

### Tie the CompositeAuthenticator with UI

If your application needs to support the <b>interactive login flow</b>, create a login UI and link it to the `IUrlRedirectionAuthenticator` methods to achieve interactive login and logout.

Please note that if the `CompositeAuthenticatorSettings` added <b>automated flow</b> or <b>preauthenticated flow</b> `IAuthenticator`, it's not guaranteed that the activated flow is interactive. This means some parts of the UI need to be hidden or disabled from code, depending on the situation. Validate this by checking the value of the `ICompositeAuthenticator.RequiresGUI` property when managing UI element.

<ol>
    <li> Create <b>Login</b> and <b>Logout</b> buttons in your scene.</li>

![Creating buttons in the scene](images/usecase1-buttons.png)
   <li>Create a <b>LoginManager</b> GameObject and attach a new <b>LoginManager</b> MonoBehaviour to it. This script links your UI with Identity's authentication engine.</li>
   <li>Update the <code>LoginManager</code> script so it references your buttons and an <code>IAuthenticationStateProvider</code>.</li>
   
   <li> Update the <code>Awake</code> method to do the following:
        <ol type="a">
                <li> Retrieve the <code>CompositeAuthenticator</code> reference from <code>PlatformServices</code>.</li>
            <li> Subscribe to the <code>AuthenticationStateChanged</code> event. </li>
            <li> Subscribe to the buttons' <code>onClick</code> events. </li>
            <li> Call <code>ApplyAuthenticationState</code> to update the UI when the scene loads. </li>
        </ol></li>
    <li> Make sure the subscriptions are cleaned up in <code>OnDestroy</code>. </li>
    <li> Implement the <code>ApplyAuthenticationState</code> method to update your buttons based on the current authentication state. Make your buttons interactive in specific circumstances, otherwise you risk errors and exceptions. To determine if you should display your buttons, rely on the <code>AuthenticationState</code> and the <code>RequiresGUI</code> property that determines whether the UI is relevant in the selected authentication flow. </li>
    <li> Define the behaviors for your buttons by calling the appropriate methods. </li>

<blockquote> <b>Note</b>: Only an interactive login flow (like the one exposed by the `IUrlRedirectionAuthenticator` interface) requires these methods. If the flow isn't interactive, calling `LoginAsync` or `LogoutAsync` throws exceptions.</blockquote>

<li> Test your scene to see how the buttons update along with the authentication state of the user. </li>
</ol>

[!code-cs [LoginManager](../Samples/Documentation/Manual/IntegrateAuthenticationExample.cs#LoginManager)]

### Leverage the authorization headers to communicate with other Unity Cloud services

After you set up your login flow, communication with other Unity Cloud services is possible. All Unity Cloud services expect an instance of `IServiceAuthorizer` to apply authorization information to HTTP requests, which the `PlatformServices` provides.

For an example of a service that expects an `IServiceAuthorizer`, refer to the [Get user information guide](use-case-getting-user-information.md).
