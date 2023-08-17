# Generate your Personal Access Token (PAT)

To generate your PAT, follow these steps:

1. Log into the [Digital Twin Dashboard](https://dashboard.unity3d.com/digital-twins/).
2. Select **Developer Hub** > **Personal Access Tokens**.
3. Select the **+Add New** button to create a new PAT.
4. Copy the PAT to your clipboard and save it to a secure location.

> **Note:** If a PAT is compromised, delete it and generate a new one.

## Inject the Personal Access Token in your application

The `CommandLineAccessTokenProvider` tries to find a PAT in the following way:

* From a command line argument passed to the application.

    ```csharp
        ./MyApp.exe -UNITY_CLOUD_PERSONAL_ACCESS_TOKEN [MyAccessToken]
    ```

The `PersonalAccessTokenProvider` tries to find a PAT in the following way:

* From a `UNITY_CLOUD_PERSONAL_ACCESS_TOKEN` environment variable set before running the application.
