# Security

This section explains how to manage the security level of your Unity Cloud application.

## Overview

Use the Unity Cloud Identity package to customize the standard [OAuth 2.0 authentication flow](https://www.rfc-editor.org/rfc/rfc6749) by injecting your `IPkceConfigurationProvider` implementation. See [code samples](use-case-customize-pkce-authentication.md).

The system uses the injected `IPkceConfigurationProvider` internally to return a `PkceConfiguration` class that determines how the application handles authentication, such as caching the `Refresh Token` on the disk.

## CacheRefreshToken setting

>[!IMPORTANT]
>We recommend that applications with high security requirements inject their own `IPkceConfigurationProvider` implementation and set the `CacheRefreshToken` to `false`.

The `CacheRefreshToken` configuration is the most critical setting for security. Setting it to `true` or `false` involves implications and inherent compromises between security and the user experience. The following are the setting details:

* If `true`, the application saves an obfuscated file on your device that stores the value of the `Refresh Token` after a successful login operation. The your session persists, even after the application shuts down, so you don't need to login manually each time you start the application. Since the `Refresh Token` is on the disk, any software installed on your device with full disk access can read the `Refresh Token` and steal the your identity given that they also know of the encryption key. This means the level of security is equal to the device's security.
* If `false`, the application doesn't save the `Refresh Token` to the disk. Each time you launch the application, you must go through the login process.

### Data obfuscation on disk

By default, an application that is linked to a `Unity Project Id` obfuscates data written to disk. If you edit the `Unity Project Id` linked to the application, the data written to disk cannot be read back.
If no `Unity Project Id` is linked to the application, the data written to disk can be read from any other application.
To link a `Unity Project Id` and prevent other applications from reading data written to disk by your application disk, follow these steps:

1. Open your application project in the Unity Editor.
2. Go to **Edit** > **Project Settings** > **Services**.
3. Follow the instructions to link a **Unity Project Id**.

## Managing service account credentials

Service accounts are programmatic users with configurable role-based access (RBAC) to a set of Unity projects within a single Unity organization. You can configure service accounts with either of the following capabilities:

* Reader-only  capabilities
* Administrator capabilities
  
Service accounts are a great tool for QA and automation as they simulate a broad range of RBAC capabilities.

### Usage of service account in application

Use service account in a released application with caution, because service account capabilities can be elevated or deleted.
The recommended best practice is to fetch the credentials from an external service that can create short-lived service account credentials over hard coding credentials in the application itself.

>[!IMPORTANT]
>Never release an application with hard-coded service account credentials to the public. Manage service account credentials as secrets during QA and automation cycles. Always assign the minimal RBAC required to a limited set of Unity projects.

## Security concerns when hosting a third-party WebGL application

A WebGL application can access sensitive information stored in client-side cookies and local storage of the host domain. Make sure you host WebGL applications only from trusted third-parties or use risk mitigation strategies, like embedding the WebGL application inside a sandboxed IFrame, to prevent security holes.
