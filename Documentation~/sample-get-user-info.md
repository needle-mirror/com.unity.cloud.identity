# Sample: Get User Information sample

You can use the Identity samples to retrieve a valid access token for a user and manage UI states. The [Get User Information](use-case-getting-user-information.md) sample outlines how to use an access token to retrieve information about the user.

## Before you start

To use the sample, you must have the following:

* Installed [Identity](installation.md) package
* A valid [Unity ID](https://id.unity.com/)

### Main components for the Identity package samples

This section describes the scripts shared in all Identity package samples.

#### Platform Services script

The `PlatformServices` class uses the `CompositeAuthenticatorSettingsBuilder` to build a list of default `IAuthenticator` implementations to cover concurrent authentication flow and assign it to a `CompositeAuthenticator`.

The `PlatformServices` class has two accompanying classes called `PlatformServicesInitialization` and `PlatformServicesShutdown` that call the initialization and shutdown methods through Unity's standard `Monobehaviour` methods `Awake()`, `Start()` and `OnDestroy()`.

To open the Platform Services script, go to the `Assets/Samples/Unity Cloud Identity/<package-version>/Shared/Scripts/PlatformServices.cs` file.

#### UI Controller script

The `UIController` class manages the visibility and interactability of GameObjects based on the `AuthenticationState` and the `RequiresGUI` properties of the `ICompositeAuthenticator`.

The `UIController` gets a reference to an `ICompositeAuthenticator` and a `IAuthenticationStateProvider` and exposes an `AuthenticationStateChanged` event. The `UIController` uses the `AuthenticationStateChanged` event to update the visibility and interactability of GameObjects.

To open the UI Controller script, go to the `Assets/Samples/Unity Cloud Identity/<package-version>/Shared/Scripts/UIController/UIController.cs` file.

#### Active User Controller script

The `ActiveUserController` class manages the authentication of a user.

The `ActiveUserController` gets a reference to an `ICompositeAuthenticator`, which calls the login and logout methods and exposes an `AuthenticationStateChanged` event. The `ActiveUserController` uses the `AuthenticationStateChanged` event to update the state of the UI.

To open the Active User Controller script, go to the `Assets/Samples/Unity Cloud Identity/<package-version>/Shared/Scripts/UIController/ActiveUserController.cs` file.

## Install the sample

To install the Get User Information sample, follow these steps:

1. In your Unity Project, go to **Window** > **Package Manager** > **Unity Cloud Identity**.
2. Expand the **Samples** section.
3. On the right of the Get User Information sample, select **Import**.
   After the import process completes, you can view the imported assets under the `Assets/Samples/Unity Cloud Identity` folder.

## Run the sample

To run the sample, follow the [Run the Authentication sample](#run-the-authentication-sample) procedure but replace step 2 with the following: Go to `Assets/Samples/Unity Cloud Identity/<package-version>/GetUserInformation/Scenes/GetUserInfoSample.unity` and run the scene.

After you log into the sample, it displays your username along with information about how you received the access token.

### Open the User Name Updater script

The `UserNameUpdater` class handles an `IAuthenticationStateProvider` and `IUserInfoProvider`, which are initialized through the `PlatformServices` class. The `IUserInfoProvider` can retrieve the `UserInfo` for a logged in user, from which the username is retrieved. The UI then updates with the username.

To open the user name updater script, go to the `Assets/Samples/Unity Cloud Identity/<package-version>/Get User Information/Scripts/UserNameUpdater.cs` file.

## Troubleshooting

Refer to the [troubleshooting](troubleshooting.md#samples-issues) section for help with the Get User Information sample.