# Unity organizations, projects, roles and permissions

Unity Cloud Identity package can list Unity entities (Unity Organizations as `IOrganization` and Unity Projects as `IProject`) that are available to the logged in user through implementations of the `IOrganizationRepository` interface, like the `CompositeAuthenticator` and the `PkceAuthenticator`. 

Roles and permissions assigned to a user can be listed or validated using awaitable methods from `IOrganization` and `IProject` implementations.

>[!NOTE]
>Different roles and permissions can be assigned to a user on a Unity Organization and on a Unity Project. See the list of [available roles and permissions](https://services.docs.unity.com/docs/service-account-auth/#available-roles).

## Fetching Unity Organizations for a user

Once the `IAuthenticationStateProvider.AuthenticationStateChanged` event is triggered with a value of `AuthenticationState.LoggedIn` you can call the `IOrganizationRepository.ListOrganizationsAsync()`
method to return the list of Unity Organizations accessible to the logged in user.

```csharp
    public class MyMonoBehaviour : MonoBehaviour
    {
        ICompositeAuthenticator m_CompositeAuthenticator;
        IOrganizationRepository m_OrganizationRepository => m_CompositeAuthenticator;
        IEnumerable<IOrganization> m_Organizations;
        
        void Start()
        {
           
            if (m_CompositeAuthenticator == null)
            {
                m_CompositeAuthenticator = PlatformServices.CompositeAuthenticator;
                m_CompositeAuthenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;

                // Update UI with current state
                _ = ApplyAuthenticationState(m_CompositeAuthenticator.AuthenticationState);
            }
        }

        void OnDestroy()
        {
            m_CompositeAuthenticator.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
        {
            _ = ApplyAuthenticationState(newAuthenticationState);
        }

        async Task ApplyAuthenticationState(AuthenticationState state)
        {
            switch (state)
            {
                case AuthenticationState.AwaitingInitialization:
                case AuthenticationState.AwaitingLogin:
                case AuthenticationState.AwaitingLogout:
                    break;
                case AuthenticationState.LoggedIn:
                    m_Organizations = await m_OrganizationRepository.ListOrganizationsAsync();
                    break;
                case AuthenticationState.LoggedOut:
                    break;
            }
        }

    }
```

## Fetching Unity Projects for an IOrganization

The `IOrganization` interface exposes a `ListProjectsAsync()` method to fetch a range of Unity Project in an awaitable `IAsyncEnumerable<IProject>` object.

```csharp
    void FetchOrganizationProjects(IOrganization organization)
    {
        var projects = organization.ListProjectsAsync(Range.All);
        var projectsList = await ToList(projects);
    }
```

## List and validate roles or permissions for Unity entities

Both `IOrganization` and `IProject` implements the `IRoleProvider` interface. They both expose awaitable methods to list and validate roles and permissions assigned to the user.

You can use roles and permissions information to provide a better user experience by adjusting the UI element available and displayed to the user.

```csharp
    // Validate if a user has the "owner" role on an IOrganization
    var hasOwnerRole = await SelectedOrganization.HasRoleAsync("owner");
    // List user assigned roles in an IOrganization
    var userRoles = await SelectedOrganization.ListRolesAsync();

    // Validate if a user has the "amc.assets.create" permission on an IProject
    var hasAssetsCreateRole = await SelectedProject.HasPermissionAsync("amc.assets.create");
    // List user assigned permissions in an IProject
    var userPermissions = await SelectedOProject.ListPermissionsAsync();
```

>[!NOTE]
>When a user has the `IOrganization.Role` value of `owner` or `manager`, it is considered as having all available roles and permissions over all `IProject` of this `IOrganization`.
Calling ListRolesAsync() or ListPermissionsAsync() method of these `IProject` returns an empty list, and calling HasRoleAsync() or HasPermissionAsync() method of these `IProject` returns false.
Consider validating first the `IOrganization.Role` value of the logged in user before calling `IRoleProvider` methods on any `IProject` belonging to this `IOrganization`.
