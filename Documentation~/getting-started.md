# Get started

This getting started guide outlines the basics of setting up a project with Unity Identity.

## Install the package

To install Unity Identity on a new or existing Unity project, install the Identity package using the [installation instructions](installation.md).

## Register an application in the Unity Cloud platform

Unity Cloud projects require an application identifier when you build the application. The application identifier identifies your application in the Unity Cloud services and also enables the custom URI scheme association with the OS that's used in Unity Deep Linking and login operations.

### Create an application identifier

To create an application identifier, follow these steps:

1. Log into the [Digital Twin Dashboard](https://dashboard.unity3d.com/digital-twins/).
2. Go to the [Identity swagger page](https://dt.unity.com/swagger/identity/index.html).
3. Use `GET /api/auth/userinfo > [Try it out] > Execute` to expose information about your Unity account. The response should look like the following:

```json
    {
        "Id": "USER_ID",
        "Name": "USER_NAME",
        "Email": "USER_MAIL",
        "Organizations": [
            {
                "Id": "ORG_ID",
                "Name": "ORG_NAME",
                "AllowCreateNewProject": true,
                "IsPrimaryOrg": false,
                "AllowRequestLicense": true,
                "Role": "USER_ROLE"
            }
        ],
        // ...
    }
```

You might have multiple organizations in your response, if your unity account belongs to multiple organizations.
Select a target organization to register your app to, and copy its `ORG_ID`.

4. Use `POST /api/applications > [Try it out]`.
5. Provide the `ORG_ID` that you previously fetched and then select a `Name` and `DisplayName` (refer to the following descriptions for more information):
    * `Name`: A unique alphanumeric app name that's lowercase and between 4 and 10 characters.
    * `DisplayName`: An arbitrary display name.

6. Select `Execute`. The following is an example of the response:

```json
    {
        "Id": "string",
        "Name": "string",
        "DisplayName": "string"
    }
```

> **Note:** The URLs must be slightly adapted if you want to generate an API token on a different service environment than production.

7. Copy the `Id` value in your clipboard. You will need this value later to set up your Unity project.

### Set up the application identifier manually

To set up the application identifier manually, follow these steps:

1. Open your application project in the Unity Editor.
2. Go to **Edit > Project Settings > Unity Cloud > App Information**.
3. In the **Enter Application ID** field, enter your application identifier.
![Entering the application identifier in Project Settings](images/manual-appid-entry.png)
4. To update the application data, click **Select**.

### Select, edit, and delete an existing application

If the `com.unity.cloud.identity` package is properly installed and you are logged in the Editor with the corresponding account, you can access existing applications.

Once logged in, follow these steps:

1. Open your application project in the Unity Editor.
2. Go to **Edit > Project Settings > Unity Cloud > App Information**.
3. Select your Organization from the **Organization** dropdown list. The list of your existing registered applications appears.
![List of existing registered applications](images/registered-applications.png)

You can select, edit, or delete an existing application from this list:

* To select an application, click the **Select** button. This action updates the application data for the project.
* To edit an application, click the **Edit** button. This action opens a window that lets you edit the `App Name` and `App ID` values.
![Editing the application name and ID](images/edit-application.png)
* To delete an application, click the **Delete** button. This action opens a window to confirm the deletion. Once deleted, you cannot recover the application.
![Deleting the application](images/delete-application.png)

### Register a new application

If the `com.unity.cloud.identity` package is properly installed and you are logged in the Editor with the corresponding account, you can access existing applications.

Once logged in, follow these steps:

1. Open your application project in the Unity Editor.
2. Go to **Edit > Project Settings > Unity Cloud > App Information**.
3. Select your Organization from the **Organization** dropdown list. Below the list of registered applications, the option to register a new app appears.
![Registering an application](images/register-application.png)
1. To register a new application, enter the desired app name and app display name in the **App Name** and **App Display Name** fields. 
> **Note**: The app name must be unique, alphanumeric, in lowercase, and between 4 to 10 characters long.
1. Click **Select** to register the new application. When successfully registered, the local application data will also be updated.

<br/> Your project is now set up.

## Manage the package stripping level

To avoid runtime errors when building with this package, follow these steps:

1. In your Unity project window, go to **Edit** > **Project settings**. <br/> The Project setting window opens.
2. Select the **Player** option.
3. Scroll to the **Additional Compiler Arguments** section.
4. Set the **Managed stripping level** option to:

- **Disabled**
<br/> or
- **Minimal** (if the **Disabled** option isn't available)

![Managed Stripping Level dropdown](images/stripping-level.png)

## Supported platforms

- Unity Editor
- Windows Standalone
- [WebGL](use-case-testing-webgl-localhost.md)
- Android
- Linux
- MacOS 
- iOS: Requires an Xcode project build and a valid development build certificate to achieve binding for the custom URI scheme at the OS level.
