# Basic concepts

The Unity Identity's authentication layer purpose is to mediate the retrieval of an access token that identifies your application user when calling Unity Cloud services.

Unity Identity supports the following flows to retrieve an access token:

|  Flow                  | Description                                                                                      |
|----------------------- |------------------------------------------------------------------------------------------------- |
| Interactive login flow | A flow where the user must manually fill a login form through a UI in a browser.                 |
| Automated flow         | A flow recommended for automated tools, but that comes handy to fast-track local development on other Unity Cloud feature that rely on Unity Identity. </br> The user generates a personal access token (PAT) from the Digital Twin Dashboard and injects it into the application via command line or environment variables to avoid interaction with a UI.                           |
| Pre-authenticated flow | A flow that can be used by web-hosted platforms (WebGL) where the host already has a valid access token that can be fetched by the application. |
| Composite flow         | A prioritized list of authentication flows that decides, based the execution context, which authentication flow to activate for the application session lifecycle. This flow offers flexibility for application built and delivered across multiple platforms (PC/MacOS/iOS/Android/WebGL) and execution contexts (CICD Automation, Tests runner).   |

## CompositeAuthenticator main class

The `CompositeAuthenticator` is the main class supporting the <b>composite flow</b>.

With the help of the `CompositeAuthenticatorSettingsBuilder`, you build a `CompositeAuthenticatorSettings` instance that holds, in a prioritized order, all `IAuthenticator` instances that are expected to be used in the application.

In its initialize phase, the `CompositeAuthenticator` iterates over each `IAuthenticator` added to the `CompositeAuthenticatorSettings` and calls its `HasValidPreconditions()` method. The first `IAuthenticator` to return true is activated for the rest of the application session lifecycle.

This section lists the main `IAuthenticator` classes for each flow and their corresponding pre-conditions.

### Interactive login flow

The interactive login flow requires user interaction with a login and a logout button. In Unity Identity, only the `PkceAuthenticator` class supports the interactive login flow.

The `PkceAuthenticator` implements the 0Auth 2.0 PKCE standard flow to retrieve an access token and involves using the default OS browser as the middle-man to authenticate the user.

It has no required pre-condition.

### Automated flow

This flow is supported by the `PersonalAccessTokenProvider` and `CommandLineAccessTokenProvider` classes and works through a PAT that you generate on the Digital Twin Dashboard.

Both classes pre-conditions are the detection of a PAT, either injected from command line or discovered in environment variables.

### Pre-authenticated flow

This non-interactive flow is for workflows where authentication takes place before launching the application: for example, when an application is deployed on the WebGL platform and hosted on a web page that already requires authentication.
This flow is supported by the `BrowserAuthenticatedAccessTokenProvider` class and retrieves an access token value from the local storage of the running browser.

This class pre-conditions are the combined detection of an hosted execution context, and the detection of an expected key name and non-null value in the local storage of the running browser.
