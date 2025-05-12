# Use case: Private Cloud and Fully Qualified Domain Name

This section provides instructions on configuring your scene to integrate an authentication layer for communication with Unity Cloud services that are hosted on a private cloud using a Fully Qualified Domain Name (FQDN).

## Before you start

* Follow the [Installation instructions](installation.md).
* Follow the [Get started instructions](getting-started.md).
* Follow the [Best practices: dependency injection guide](best-practices-dependency-injection.md).

## About private hosting of Unity Cloud services

Unity Cloud services can be provided from a FQDN following a contractual agreement with Unity. In the Unity Cloud Identity package, you can choose to support all providers of Unity Cloud services, or support only a single provider. This use case explains how to support a single FQDN provider of Unity Cloud services programmatically.

## How do I...?

### Instantiate an ICompositeAuthenticator for an FQDN provider in PlatformServices

To instantiate an `ICompositeAuthenticator` interface in the `PlatformServices` class, follow these steps:

1. Create one variable to hold the FQDN and one variable to hold the full URL to an accessible OpenID Connect (OIDC) configuration file.
1. Create an `IAuthenticationPlaformSupport` and an `IHttpClient` interface, then get the references to the `IAppIdProvider` and `IAppNamespaceProvider` interfaces from the `UnityCloudPlayerSettings.Instance` singleton.
1. Inject them in the `ServiceConnectorFactory.CreateForFullyQualifiedDomainName` method to build an `ICompositeAuthenticator` that supports exclusively the defined FQDN.
1. Add the following references to the created instance of the `ICompositeAuthenticator`:
   1. A private reference
   1. A public reference
1. Initialize the `ICompositeAuthenticator` and any other services in the `InitializeAsync` method.
1. Shutdown the `ICompositeAuthenticator` and any other services in the `Shutdown` method.

[!code-cs [PlatformServices](../Samples/Documentation/Manual/FullyQualifiedDomainNameExample.cs#PlatformServices)]
