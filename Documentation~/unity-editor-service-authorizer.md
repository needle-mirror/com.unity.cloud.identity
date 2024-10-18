# Unity Editor Service Authorizer

The `UnityEditorServiceAuthorizer` class is an implementation of the `IServiceAuthorizer` interface, and you can use it only in the context of Unity Editor scripting.

The class inherits from the `UnityEditor.ScriptableSingleton<T>` class to ensure seamless integration into the Unity Editor lifecycle. This integration provides support for domain reloads and serialization of internal property states, such as `AuthenticationState` and any acquired Unity Services token.

Use the `UnityEditorServiceAuthorizer` class for these purposes:
* Trigger the `IAuthenticationStateProvider.AuthenticationStateChanged` event using the active user session in the Unity Editor.
* Fetch the Unity organizations and Unity projects that are associated with the current user, along with their assigned roles and permissions in these entities. To do this, use the methods that this class exposes from the `IOrganizationRepository` and `IUserInfoProvider` interfaces.

[Learn](entities-rbac.md) to use the `UnityEditorServiceAuthorizer.instance` static property as an `IOrganizationRepository` interface and retrieve organizations, projects, members, and Role-based access control (RBAC) information available for the current logged-in user.

You can inject `UnityEditorServiceAuthorizer.instance` as an `IServiceAuthorizer` in a `Common.ServiceHttpClient` class instance to provide the bearer authorization header in HTTP requests to Unity Cloud service endpoints.

## UnityEditorServiceAuthorizer usage

The example below shows how you can use the `UnityEditorServiceAuthorizer` class in a `UnityEditor.EditorWindow` derived class to fetch the name of the logged-in user:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UnityEditorServiceAuthorizerExample.cs#EditorWindow)]
