# Unity Editor Service Authorizer

The `UnityEditorServiceAuthorizer` class is an `IServiceAuthorizer` implementation to only use in the context of Unity Editor scripting.

The class is derived from the `UnityEditor.ScriptableSingleton<T>` for a seamless integration in the Editor lifecycle. It brings domain reload and serialization support for internal property state like the `AuthenticationState` and any acquired Unity Services token.

It relies on the running Unity Editor's user session to provide `IAuthenticationStateProvider.AuthenticationStateChanged` event and it exposes `IOrganizationRepository` and `IUserInfoProvider` methods to fetch Unity Organizations and Unity Projects of the logged in user and its assigned roles and permissions in them.

See [Unity entities Roles and Permissions](entities-rbac.md) to learn how to use the `UnityEditorServiceAuthorizer.instance` as an `IOrganizationRepository` and retrieve organizations, projects, members and RBAC information available to the current logged in user.

The `UnityEditorServiceAuthorizer.instance` can also be injected as an `IServiceAuthorizer` in a `Common.ServiceHttpClient` class instance to provide the bearer authorization header in HTTP requests to Unity Cloud service endpoints.

## UnityEditorServiceAuthorizer usage

Here is a sample usage of the `UnityEditorServiceAuthorizer` in a `UnityEditor.EditorWindow` derived class to fetch the name of the logged in user:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UnityEditorServiceAuthorizerExample.cs#EditorWindow)]