# Unity Editor Authenticator

The `UnityEditorAuthenticator` class is an `IAuthenticator` implementation to only use in the context of Unity Editor scripting.

It relies on the running Unity Editor's user session to provide `IAuthenticationStateProvider.AuthenticationStateChanged` event and all `IOrganizationRepository` and `IUserInfoProvider` methods to fetch Unity Organizations and Unity Projects of the logged in user and its assigned roles and permissions in them.

## UnityEditorAuthenticator default usage

Here is an example of a default instantiation of the `UnityEditorAuthenticator` in a `UnityEditor.EditorWindow` derived class to fetch the name of the logged in user:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UnityEditorAuthenticatorExample.cs#EditorWindow)]

## Use a UnityEditorAuthenticator with a custom Access Token provider usage

Here is an example of instantiation of the `UnityEditorAuthenticator` that uses a custom `IUnityEditorAccessTokenProvider` implementation:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UnityEditorAuthenticatorExample.cs#EditorWindowCustomTokenProvider)]

## Use a UnityEditorAuthenticator with CLI Unity Editor launch arguments --username --password

>[!IMPORTANT]
>Never expose or commit your personal credentials in code. The recommended best practice is to use proper secret injection from your automation platform to add credentials to your CLI command.

Here is an example of instantiation of the `UnityEditorAuthenticator` that uses the `LaunchArgumentsUnityEditorAccessTokenProvider` implementation:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UnityEditorAuthenticatorExample.cs#EditorWindowLaunchArguments)]
