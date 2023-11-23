# Security

This section explains how to manage the security level of your Unity Cloud application.

## Overview

You can use the Unity Cloud Identity package to customize the standard [OAuth 2.0 authentication flow](https://www.rfc-editor.org/rfc/rfc6749) by injecting your `IPkceConfigurationProvider` implementation. Refer to [Customize the PKCE authentication flow](use-case-customize-pkce-authentication.md) for code samples.

The injected `IPkceConfigurationProvider` is used internally to return a `PkceConfiguration` that determines how the application handles authentication, such as allowing guest users or caching the `Refresh Token` on the disk.

## Default PKCE configuration

The `PkceConfiguration.DefaultConfiguration` prevents guest users access to any resource and allows the application to cache the `Refresh Token` on the disk.

The `PkceConfiguration.DefaultConfiguration` has the following values:

```csharp
    {
        AppName = "default",
        CacheRefreshToken = true,
        ClientId = new ClientId("digital_twins"),
        ProxyLoginRedirectRoute = "https://services.api.unity.com/app-linking/v1/login/redirect/",
        ProxyLoginCompletedRoute = "https://services.api.unity.com/app-linking/v1/login/completed/",
        ProxySignOutCompletedRoute = "https://services.api.unity.com/app-linking/v1/signout/completed/",
        LoginUrl = $"https://api.unity.com/v1/oauth2/authorize",
        TokenUrl = $"https://api.unity.com/v1/oauth2/token",
        RefreshTokenUrl = $"https://api.unity.com/v1/oauth2/token",
        LogoutUrl = $"https://api.unity.com/v1/oauth2/revoke",
        SignOutUrl = $"https://api.unity.com/v1/oauth2/end-session?post_logout_redirect_uri=",
        UserInfoUrl = $"https://api.unity.com/v1/users/current/openid",
        CustomLoginParams = ""
    }
```

## CacheRefreshToken setting

>[!IMPORTANT]
>Applications with high security requirements should set the `CacheRefreshToken` to `false`.

The `CacheRefreshToken` configuration is the most critical setting for security. There are implications of setting it to `true` or `false` and inherent tradeoffs between security and the user experience. The following are the setting details:

* If `true`, the application saves an obfuscated file on the user's device that stores the value of the `Refresh Token` after a successful login operation. The user's session persists, even after the application shuts down, so the user doesn't need to login manually each time they start the application. Since the `Refresh Token` is on the disk, any software installed on the user's device with full disk access can read the `Refresh Token` and steal the user's identity given that they also know of the encryption key. This means the level of security is equal to the device's security.
* If `false`, the application doesn't save the `Refresh Token` to the disk. Each time the user launches the application, they'll have to go through the login process.

## Using a service account

It is recommended never to release to the public any app that uses hardcoded service accounts.
