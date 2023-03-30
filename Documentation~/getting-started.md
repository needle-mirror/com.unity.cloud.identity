# Get started

This getting started guide outlines the basics of setting up a project with Identity.

## Install the package

To install Identity on a new or existing Unity project, install the Identity package using the [installation instructions](installation.md).

## Register an application in the Unity Cloud platform

Unity Cloud projects require an application identifier when you build the application. The application identifier identifies your application in the Unity Cloud services and also enables the custom URI scheme association with the OS that's used in Deep Linking and login operations.

### Create an application identifier

To create an application identifier, follow these steps:

1. Log into the [Unity Cloud Portal](https://dt.unity.com).
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

    You might have multiple organizations in your response if your Unity account belongs to multiple organizations. Select a target organization to register your application to and copy its ORG_ID.
4. Use `POST /api/applications > [Try it out]`.
5. Provide the ORG_ID that you previously fetched and then select a `Name` and `DisplayName` (refer to the following descriptions for more information):
    * `Name`: A unique alphanumeric application name that's lowercase and between 4 and 10 characters.
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

### Set up the application identifier

To set up the application identifier, follow these steps:

1. Open your application project in the Unity Editor.
2. Go to **Edit > Project Settings > Unity Cloud > App Registration**.
3. Enter your application identifier in the **App Id** field.
![Entering the application identifier in Project Settings](images/gettingstarted-appid.png)

4. Select **Refresh** to update the application data in the Unity Cloud Portal.
Your project is now setup.

## Supported platforms

- Unity Editor
- Windows Standalone
- [WebGL](use-case-testing-webgl-localhost.md)
- Android
- Linux
- MacOS 
- iOS: Requires an Xcode project build and a valid development build certificate to achieve binding for the custom URI scheme at the OS level.
