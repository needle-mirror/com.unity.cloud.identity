# Unity organizations, projects, roles and permissions

Unity Cloud Identity package can list Unity entities (Unity Organizations as `IOrganization` and Unity Projects as `IProject`) that are available to the logged in user through implementations of the `IOrganizationRepository` interface, like the `CompositeAuthenticator` and the `PkceAuthenticator`. 

Roles and permissions assigned to a user can be listed or validated using awaitable methods from `IOrganization` and `IProject` implementations.

>[!NOTE]
>Different roles and permissions can be assigned to a user on a Unity Organization and on a Unity Project. See the list of [available roles and permissions](https://services.docs.unity.com/docs/service-account-auth/#available-roles).

## Fetching Unity Organizations for a user

Once the `IAuthenticationStateProvider.AuthenticationStateChanged` event is triggered with a value of `AuthenticationState.LoggedIn` you can call the `IOrganizationRepository.ListOrganizationsAsync()`
method to return the list of Unity Organizations accessible to the logged in user.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/EntitiesRbacExample.cs#ListOrganizations)]

## Fetching Unity Projects for an IOrganization

The `IOrganization` interface exposes a `ListProjectsAsync()` method to fetch a range of Unity Project in an awaitable `IAsyncEnumerable<IProject>` object.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/EntitiesRbacExample.cs#ListOrganizationProjects)]

## List and validate roles or permissions for Unity entities

Both `IOrganization` and `IProject` implements the `IRoleProvider` interface. They both expose awaitable methods to list and validate roles and permissions assigned to the user.

You can use roles and permissions information to provide a better user experience by adjusting the UI element available and displayed to the user.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/EntitiesRbacExample.cs#ListOrganizationRoles)]

[!code-cs [behaviour-script](../Samples/Documentation/Manual/EntitiesRbacExample.cs#ListProjectPermissions)]

>[!NOTE]
>When a user has the `IOrganization.Role` value of `owner` or `manager`, it is considered as having all available roles and permissions over all `IProject` of this `IOrganization`.
Calling ListRolesAsync() or ListPermissionsAsync() method of these `IProject` returns an empty list, and calling HasRoleAsync() or HasPermissionAsync() method of these `IProject` returns false.
Consider validating first the `IOrganization.Role` value of the logged in user before calling `IRoleProvider` methods on any `IProject` belonging to this `IOrganization`.
