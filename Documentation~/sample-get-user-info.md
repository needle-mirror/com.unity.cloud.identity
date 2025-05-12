# Sample: Get User Information sample

Use the Unity Cloud Identity samples to retrieve a valid access token for a user and manage UI states. The [Get User Information](use-case-getting-user-information.md) sample outlines how to use an access token to retrieve information about the user.

## Before you start

Before you use the sample, make sure you have the following:

* An installed [Identity](installation.md) package.
* A valid [Unity ID account](https://id.unity.com/).

## Install the sample

To install the sample, follow these steps:

1. In the Unity Editor, go to **Window** > **Package Manager** > **Unity Cloud Identity**.
2. Expand the **Samples** section.
3. Select **Import** next to `Get user Information`.

   ![Screenshot of the Package Manager's Identity samples](images/import-samples.png)

 4. After the import process completes, you can view the imported assets under the `Assets/Samples/Unity Cloud Identity` folder.

## Run the sample

To run the sample, follow these steps:

1. Go to `Assets/Samples/Unity Cloud Identity/<package-version>/GetUserInformation/Scenes/GetUserInfoSample.unity` and run the scene.
1. In the Game view, select **Login** if you are logged out.
1. Log into the browser window that launches with your Unity ID account.
1. Return to the sample scene to confirm that you are logged in.

    >[!NOTE]
    >Your device stores a refresh token until you log out. This token lets the Identity service automatically attempt to log you in when you relaunch the sample.

6. Your username is displayed along with information about how you received the access token, which organizations, projects you have access to and what legacy role you have on a selected organization.

### Main components for the Unity Cloud Identity package samples

This section describes the scripts shared in all Identity package samples.

#### Platform Services script

The `PlatformServices` class uses the `CompositeAuthenticatorSettingsBuilder` class to build a list of default `IAuthenticator` implementations to cover the concurrent authentication flow and assign it to a `CompositeAuthenticator` class.
Use the following classes with the `PlatformServices` class:

* `PlatformServicesInitialization` 
* `PlatformServicesShutdown`

These classes use the following standard Unity `Monobehaviour` methods to run the initialization and shutdown methods:

* `Awake()`
* `Start()`
* `OnDestroy()`

To open the Platform Services script, go to the `Assets/Samples/Unity Cloud Identity/<package-version>/Shared/Scripts/PlatformServices.cs` file.

#### UI Controller script

The `UIController` class manages the visibility and interactability of GameObjects based on the `AuthenticationState` and the `RequiresGUI` properties of the `ICompositeAuthenticator` interface.

The `UIController` gets a reference to an `ICompositeAuthenticator` and a `IAuthenticationStateProvider` interface and exposes an `AuthenticationStateChanged` event. The `UIController` uses the `AuthenticationStateChanged` event to update the visibility and interactability of GameObjects.

To open the UI Controller script, go to the `Assets/Samples/Unity Cloud Identity/<package-version>/Shared/Scripts/UIController/UIController.cs` file.

#### Active User Controller script

The `ActiveUserController` class manages the authentication of a user.

The `ActiveUserController` gets a reference to an `ICompositeAuthenticator` interface, which calls the login and logout methods and exposes an `AuthenticationStateChanged` event. The `ActiveUserController` uses the `AuthenticationStateChanged` event to update the state of the UI.

To open the Active User Controller script, go to the `Assets/Samples/Unity Cloud Identity/<package-version>/Shared/Scripts/UIController/ActiveUserController.cs` file.

### The User Name Updater script

The `UserNameUpdater` class handles an `IAuthenticationStateProvider` interface and an `IAuthenticatedUserInfoProvider` interface, which are initialized through the `PlatformServices` class. The `IAuthenticatedUserInfoProvider` retrieves the `UserInfo` for a signed in user, from which the username is retrieved. The UI then updates with the username.

To open the User Name Updater script, go to the `Assets/Samples/Unity Cloud Identity/<package-version>/GetUserInformation/Scripts/UserNameUpdater.cs` file.

## Troubleshooting

In case of issues with the Get User Information sample, refer to the [troubleshooting](troubleshooting.md#sample-issues).
