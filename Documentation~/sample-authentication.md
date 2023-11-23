# Sample: Authentication sample

You can use the Unity Cloud Identity samples to retrieve a valid access token for a user and manage UI states. The Authentication sample outlines how to listen to Authentication state changes in your application to manage the visibility and interactabilty of GameObjects.

## Prerequisites

To use the Unity Cloud Identity samples, you require the following:

* Installed [Identity](installation.md) package
* A valid [Unity ID](https://id.unity.com/)

## Install the sample

To install the sample, follow these steps:

1. In your Unity Editor Project, go to **Window** > **Package Manager** > **Unity Cloud Identity**.
2. Expand the **Samples** section.
3. On the right of the Authentication sample, select **Import**.

   ![Screenshot of the Package Manager's Identity samples](images/import-samples.png)

   After the import process completes, you can view the imported assets under the `Assets/Samples/Unity Cloud Identity` folder.

## Run the sample

To run the sample, follow these steps:

1. In your Unity Editor Project, go to **File** > **Open Scene**.
2. Go to `Assets/Samples/Unity Cloud Identity/<package-version>/Authentication/Scenes/AuthenticationSample.unity` and run the scene.
3. In the Game view, select **Login** if you are logged out.
4. Log into the browser window that launches with your Unity ID account.
5. Return to the sample scene to confirm that you are logged in.

    >[!NOTE]
    >Your device stores a refresh token until you log out. This token lets the Identity service automatically attempt to log you in when you relaunch the sample.

6. (Optional) Relaunch the sample, without logging out, to test the automatic login.

### Main components for the Unity Cloud Identity package samples

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

### AuthenticationState Updater script

The `AuthenticationStateUpdater` class manages the authentication state of a user.

The `AuthenticationStateUpdater` gets a reference to an `IAuthenticationStateProvider` and exposes an `AuthenticationStateChanged` event. The `AuthenticationStateUpdater` uses the `AuthenticationStateChanged` event to update the state of the UI.

To open the Active User controller script, go to the `Assets/Samples/Unity Cloud Identity/<package-version>/Authentication/Scripts/AuthenticationStateUpdater.cs` file.

## Troubleshooting

Refer to the [troubleshooting](troubleshooting.md#samples-issues) section for help with the Authentication sample.
