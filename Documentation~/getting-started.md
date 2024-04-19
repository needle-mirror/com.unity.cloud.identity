# Get started

This getting started guide outlines the basics of setting up a Unity Editor Project with Unity Cloud Identity. Before you begin, make sure you meet the [prerequisites](prerequisites.md).

## Install the package

To install Unity Cloud Identity on a new or existing Unity Editor Project, install the Identity package using the [installation instructions](installation.md).

### Unity Cloud Application Namespace

Application that uses Unity Cloud packages require an application namespace. This namespace enables custom URI scheme association with the OS for Deep Linking and login operations.

For more information, see the **Unity Cloud Application Namespace** section in the [Common package documentation](https://docs.unity3d.com/Packages/com.unity.cloud.common@latest).

## Apple Privacy Manifest
As per Apple's [Privacy updates for App Store submissions](https://developer.apple.com/news/?id=r1henawx), this package includes a [Privacy Manifest](https://developer.apple.com/documentation/bundleresources/privacy_manifest_files) file outlining the relevant privacy declarations.

>[!NOTE]
>Though the Unity Cloud Identity package has declared that User ID is not used for [tracking](https://developer.apple.com/app-store/app-privacy-details/#user-tracking), should your application change the login page for a third-party domain, or use SSO to login from a third-party domain, you should re-evaluate this answer for your own manifests.

## Troubleshooting

Refer to the [troubleshooting](troubleshooting.md#getting-started-issues) section if you have trouble getting started with your Unity Editor Project.
