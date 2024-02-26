# Use case: Customize the PKCE authentication flow

This section explains how to inject your own `IPkceConfigurationProvider` implementation in the `CompositeAuthenticator`.

You can customize the returned `PkceConfiguration` to fulfill security requirements by injecting your own implementation.

## Before you start

To use this sample, you must do the following:

1. Install the [Identity package](installation.md)
2. Follow the [Get started guide](getting-started.md)
3. Follow the [Best practice | Dependency injection guide](best-practices-dependency-injection.md)
4. [Integrate authentication in your scene](use-case-integrating-authentication-in-your-scene.md) to implement the interactive user login flow

## How do I...?

### Inject a CustomPkceConfiguration in the PkceAuthenticator using the PkceAuthenticatorSettingsBuilder

Modify the `PlatformServices` class by adding a new instance of the `CustomPkceConfigurationProvider` in the `PkceAuthenticatorSettingsBuilder` and use the
resulting `customPkceAuthenticatorSettings` to instantiate a `PkceAuthenticator`.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCase/CustomizePkceAuthenticatorExample.cs#CustomPkceAuthenticator)]

### Optional | Adjust the PkceConfiguration's returned value

You can adjust the `PkceConfiguration`'s returned value after you provide your implementation of the `IPkceConfigurationProvider`. To customize the returned value to fulfill security requirements, either set the `CacheRefreshToken` value to `false` or change which PKCE configuration to use with another Identity provider.
