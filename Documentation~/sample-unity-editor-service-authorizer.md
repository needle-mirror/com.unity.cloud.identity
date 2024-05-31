# Sample: UnityEditorServiceAuthorizer sample

The UnityEditorServiceAuthorizer sample outlines how to use the `UnityEditorServiceAuthorizer.instance` in an `EditorWindow` to retrieve information about the current user logged in the Unity Editor.

See [Unity Editor Service Authorizer](unity-editor-service-authorizer.md) for more information about this class.

## Before you start

To use the sample, you must have the following:

* Installed [Identity](installation.md) package
* A valid [Unity ID](https://id.unity.com/)

## Install the sample

To install the sample, follow these steps:

1. In your Unity Editor Project, go to **Window** > **Package Manager** > **Unity Cloud Identity**.
2. Expand the **Samples** section.
3. On the right of the UnityEditorServiceAuthorizer sample, select **Import**.

   ![Screenshot of the Package Manager's Identity samples](images/import-samples.png)

   After the import process completes, you can view the imported assets under the `Assets/Samples/Unity Cloud Identity` folder.

## Run the sample

To run the sample, follow these steps:

1. In your Unity Editor Project, go to **Unity Cloud** > **Samples** and click on the **UnityEditorServiceAuthorizer Sample** menu item.
2. Your username is displayed in a Label GUILayout element inside a custom EditorWindow.

#### UnityCloudIdentityWindow script

The `UnityCloudIdentityWindow` class uses the `UnityEditorServiceAuthorizer.instance` as an `IUserInfoProvider` to retrieve information about the current user logged in the Unity Editor.

The `AuthenticationStateChanged` event of the `UnityEditorServiceAuthorizer.instance` is registered in the `OnEnable` method and unregistered in the `OnDisable` method. In the `AuthenticationStateChanged` event handler, when the user is logged in, the `IUserInfoProvider.GetUserInfoAsync()` method is called to retrieve the username of the current logged in user in the Unity Editor.

>[!NOTE]
>The `AuthenticationState` is updated when the Unity Editor is launched and when a user logs in or out from the Unity Editor.

To open the UnityCloudIdentityWindow script, go to the `Assets/Samples/Unity Cloud Identity/<package-version>/UnityEditorServiceAuthorizer/Scripts/UnityCloudIdentityWindow.cs` file.

## Troubleshooting

Refer to the [troubleshooting](troubleshooting.md#samples-issues) section for help with the UnityEditorServiceAuthorizer sample.
