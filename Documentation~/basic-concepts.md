# Basic concepts

The Unity Cloud Identity package's purpose is to mediate the retrieval of an access token that identifies your Unity Cloud application user when calling Unity Cloud services.

Unity Identity supports the following flows to retrieve an access token:

|  Flow                  | Description                                                                                      |
|----------------------- |------------------------------------------------------------------------------------------------- |
| Interactive login flow | A flow where the user must manually fill a login form through a UI in a browser.                 |                      |
| Service account flow | A flow that uses service account credentials injection to override the interactive flow. |
| Composite flow         | A prioritized list of authentication flows that decides, based on the execution context, which authentication flow to activate for the application session lifecycle. This flow offers flexibility for application that are built and delivered across multiple platforms (PC/MacOS/iOS/Android), and execution contexts (CICD Automation, Tests runner) that want to leverage both the interactive flow and the service account flow.   |

## CompositeAuthenticator main class

The `CompositeAuthenticator` is the main class that supports the **composite flow**.

With the help of the `CompositeAuthenticatorSettingsBuilder`, you build a `CompositeAuthenticatorSettings` instance that holds, in a prioritized order, all `IAuthenticator` instances that are expected to be used in the application.

In its initialize phase, the `CompositeAuthenticator` iterates over each `IAuthenticator` added to the `CompositeAuthenticatorSettings` and calls its `HasValidPreconditions()` method. The first `IAuthenticator` to return true is activated for the rest of the application session lifecycle.

This section lists the main `IAuthenticator` classes for each flow along with their corresponding pre-conditions.

### Interactive login flow

The interactive login flow requires user interaction with a login and a logout button. In Unity Identity, only the `PkceAuthenticator` class supports the interactive login flow.

The `PkceAuthenticator` implements the 0Auth 2.0 PKCE standard flow to retrieve an access token and involves using the default OS browser as the middle-man to authenticate the user.

It has no required pre-condition.

### Service Account flow

This non-interactive flow is supported by the `ServiceAccountAuthenticator` class and either uses the service account credentials with basic authentication to directly reach the Unity Cloud services or exchanges those credentials for an expiring token to use with bearer authentication.

The name of the expected environment variable that holds service account credentials is `UNITY_SERVICE_ACCOUNT_CREDENTIALS`. The format of the provided string value is `username`:`password` using the `:` character as a separator between the service account username and password.

The `ServiceAccountAuthenticator` class meets the required pre-conditions when service account credentials are injected in the running process using environment variables.

