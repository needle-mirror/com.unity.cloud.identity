# Service Account Usage

A service account allows authorization for internal headless scenarios (automated scripts, tests, etc...).

## Generate a Service Account

1. As the owner or manager of a Unity Organization, you can [create a Service Account](https://services.docs.unity.com/docs/service-account-auth/#create-a-service-account), and assign it any roles for this organization and its projects (depending on your needs).
2. Once the account is created, be sure to store its credentials (`key ID` and `secret key`) **in a secure and private location**.
3. You can now refer to those credentials in your app, following the '**keyID**:**secret**' format, to authorize your calls to Unity Cloud services.

## Using the ServiceAccountAuthorizer

The `ServiceAccountAuthorizer` class implements the `IServiceAuthorizer` interface. Once instantiated, it can be injected in a `ServiceHttpClient` to
access Unity Cloud services.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/ServiceAccountsExample.cs#ServiceAuthorizer)]

### Inject the Service Account credentials in your Unity Cloud application

You can inject the Service Account credentials by either using command line arguments when launching the application or by defining 
an environment variable at the OS level.

* Injecting credentials from a command line argument passed to the Unity Cloud application.

    ```csharp
        ./MyApp.exe -UNITY_SERVICE_ACCOUNT_CREDENTIALS [keyID:secret]
    ```

* Injecting credentials from a `UNITY_SERVICE_ACCOUNT_CREDENTIALS` environment variable set before running the Unity Cloud application.
