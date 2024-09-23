# Security

This section explains how to manage the security level of your Unity Cloud application.

## Overview

You can use the Unity Cloud Identity package to customize the standard [OAuth 2.0 authentication flow](https://www.rfc-editor.org/rfc/rfc6749) by injecting your `IPkceConfigurationProvider` implementation. Refer to [Customize the PKCE authentication flow](use-case-customize-pkce-authentication.md) for code samples.

The injected `IPkceConfigurationProvider` is used internally to return a `PkceConfiguration` that determines how the application handles authentication, such as caching the `Refresh Token` on the disk.

## Default PKCE configuration

The `PkceConfiguration.DefaultConfiguration` allows the application to cache the `Refresh Token` on the disk.

The `PkceConfiguration.DefaultConfiguration` has the following values:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/SecurityExample.cs#DefaultConfiguration)]

## CacheRefreshToken setting

>[!IMPORTANT]
>Applications with high security requirements should set the `CacheRefreshToken` to `false`.

The `CacheRefreshToken` configuration is the most critical setting for security. There are implications of setting it to `true` or `false` and inherent tradeoffs between security and the user experience. The following are the setting details:

* If `true`, the application saves an obfuscated file on the user's device that stores the value of the `Refresh Token` after a successful login operation. The user's session persists, even after the application shuts down, so the user doesn't need to login manually each time they start the application. Since the `Refresh Token` is on the disk, any software installed on the user's device with full disk access can read the `Refresh Token` and steal the user's identity given that they also know of the encryption key. This means the level of security is equal to the device's security.
* If `false`, the application doesn't save the `Refresh Token` to the disk. Each time the user launches the application, they'll have to go through the login process.

### Data obfuscation on disk

By default, an application linked to a `Unity Project Id` obfuscates data written to disk. If you edit the `Unity Project Id` linked to the application, the data written to disk cannot be read back.
If no `Unity Project Id` is linked to the application, the data written to disk can be read from any other application.
To link a `Unity Project Id` and prevent other applications from reading your application written data on disk, follow these steps:

1. Open your application project in the Unity Editor.
2. Go to **Edit** > **Project Settings** > **Services**.
3. Follow the instructions to link a **Unity Project Id**.

## Managing service account credentials

Service accounts are programmatic users with configurable role based access (RBAC) to a set of Unity Projects within a single Unity Organization. Service accounts can be configured to have only reader capabilities, or to have administrator capabilities.
Service accounts are a great tool for QA and automation as they simulate a broad range of RBAC capabilities.

### Usage of service account in application

Using service account in a released application needs to be done with caution, as service account capabilities can be elevated or deleted.
The recommended best practice is to fetch the credentials from an external service that can create short-lived service account credentials over hard coding credentials in the application itself.

>[!IMPORTANT]
>The recommended best practice is to never release to the public any app that contains hard-coded service account credentials. Service account credentials should be managed as secrets in QA and automation cycles. Always assign the minimal RBAC required on a limited set of Unity Project.

## Security concerns when hosting a third-party WebGL application

A WebGL application can access sensitive information stored in client-side cookies and local storage of the host domain. Make sure you only host WebGL application from trusted third-parties or use risk mitigation strategies like embedding the WebGL application inside a sandboxed IFrame to prevent security holes.
