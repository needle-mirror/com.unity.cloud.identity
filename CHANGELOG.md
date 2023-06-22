# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.16.1] - 2023-06-22

### Added
- `IAccessTokenExchanger` interface.
- `UnityServicesToken`, `ExchangeGenesisTokenRequest`, `TargetClientIdToken`, `ExchangeGenesisAccessTokenResponse`, `ExchangeTargetClientIdTokenResponse` class.
- `DeviceTokenToUnityServicesTokenExchanger` and `TargetClientIdTokenToUnityServicesTokenExchanger` implementation of the `IAccessTokenExchanger` interface.
- `UnityEditorAuthenticator` class to return a `UnityServicesToken` from the user logged in the Unity Editor.

### Changed
- `PkceAuthenticator` returns an access token from a `UnityServicesToken` in its `IAccessTokenProvider` implementation.
- `PkceAuthenticator` constructors not supporting the injection of an `IAccessTokenExchanger` implementation class marked as obsolete.
- `CompositeAuthenticatorSettingsBuilder.AddDefaultPkceAuthenticator` marked as obsolete in favor of a new override that accepts only an `IAppNameProvider` argument.
- `PkceConfigurationProvider` constructor marked as absolete in favor of a new constructor that accepts `ServiceHostConfiguration` and `IAppNameProvider` arguments.

## [0.16.0] - 2023-05-25

### Added
- Proxy redirect routes exposed in `PkceConfiguration`.

### Changed
- Device tokens are now saved per `ServiceEnvironment`.
- [Breaking] `ServiceHostConfiguration` is now required to build a `PkceConfigurationProvider` and `CompositeAuthenticatorSettingsBuilder`.
- [Breaking] `PkceConfigurationProvider` will build a `PkceConfiguration` dynamically based on the `ServiceHostConfiguration` provided.
- `CompositeAuthenticator` in samples now takes `ServiceHostConfiguration` as a parameter.

### Removed
- [Breaking] `DefaultConfiguration` has been removed from `PkceConfiguration`. `PkceConfigurationProvider` will build a default configuration based on a provided `ServiceHostConfiguration`.

## [0.15.3] - 2023-05-11

### Added
- Exposed an event in `PkceAuthenticator.cs` which is raised whenever the device token is refreshed.

### Changed
- Upgrade to Moq 2.0.0-pre.2

## [0.15.2] - 2023-04-27

### Added
- Added a documentation section on `Managed Stripping Level` for build settings.

### Fixed
- Reload browser page issue on WebGL.

## [0.15.1] - 2023-04-13

### Added
- `SignOutUrl` property in `PkceConfiguration`.
- `IAuthenticationPlatformSupport.GetRedirectUri` method accepts an optional string parameter to support any redirection route.

### Changed
- `LogoutAsync` method in `IUrlRedirectionAuthenticator` interface and its derived implementations accepts an optional boolean value to clear the browser cache.

## [0.15.0] - 2023-03-30

### Added
- Added Api version info to package

### Changed
- [Breaking] Change identifier types from Guid to SceneId, WorkspaceId, DatasetId, OrganizationId and VersionId.

### Fixed
- Disable code using Moq when the package is not present.

## [0.14.0] - 2023-03-16

### Changed
- `Authentication` and `GetUserInfo` samples UI refactor.
- [Breaking] All mentions of "Digital Twins" and "DT" renamed to their "Unity Cloud" equivalent, or removed altogether

## [0.13.0] - 2023-03-02

### Removed
- [Breaking] the `CompositeAuthenticator` constructor that accepts a list of `IAuthenticator`.

### Added
- New `CompositeAuthenticatorSettings` and `CompositeAuthenticatorSettingsBuilder`.
- New constructor for `CompositeAuthenticator` accepting a `CompositeAuthenticatorSettings` instance.

## [0.12.1] - 2023-02-24

### Changed
- Update to latest common dependency

## [0.12.0] - 2023-02-16

### Changed
- `EditorPkcePlatformSupport` awaits login response from the browser at a randomly attributed port, instead of the fixed 3000 port.
- [Breaking] `ICompositeAuthenticator.Interactive` property renamed to `ICompositeAuthenticator.RequiresGUI`.
- `BrowserAuthenticatedAccessTokenProvider` constructor requires a new `Dictionary<string, string>` to support different host locations.
- [Breaking] `IAuthenticator.HasValidPreconditions` method renamed to `IAuthenticator.HasValidPreconditionsAsync` and returns a `Task<bool>`.
- [Breaking] `IAuthenticationPlatformSupport.OpenUrlAndWaitForRedirection` can now throw a `TimeoutException`.
- Updated tests for new exceptions thrown by url-redirection.

### Added
- New `CompositeAuthenticatorSettings` and `CompositeAuthenticatorSettingsBuilder`.
- New constructor for `CompositeAuthenticator` accepting a `CompositeAuthenticatorSettings` instance.
- Support PKCE authentication flow for `https://` hosted WebGL builds.

## [0.11.0] - 2023-02-02

### Changed
- [Breaking] `IUrlRedirectionPlatformSupport` renamed to `IAuthenticationPlatformSupport`.
- `PersonalAccessTokenProvider` and  `CommandLineAccessTokenProvider` no longer blocks the activationUrl consumption flow.
- [Breaking] `PersonalAccessTokenProvider` expects a mandatory `IAuthenticationPlatformSupport` argument.
- [Breaking] `IUrlRedirectionInterceptor` removed from `PkceAuthenticator` constructors.
- [Breaking] `IPkcePlatformSupport` renamed to `IUrlRedirectionPlatformSupport`.
- [Breaking] `IInteractiveAuthenticator` renamed to `IUrlRedirectionAuthenticator`.
- [Breaking] `CompositeAuthenticator` inherits from new `ICompositeAuthenticator` interface.
- [Breaking] `CompositeAuthenticator` constructor has unique mandatory `List<IAuthenticator>` argument.
- [Breaking] `PersonalAccessTokenProvider`,  `CommandLineAccessTokenProvider`, `BrowserAuthenticatedAccessTokenProvider` inherits from `IAuthenticator` interface.
- [Breaking] `ICompositeAuthenticator` and `IAuthenticator` interfaces refactored.

### Removed
- [Breaking] `PreAuthenticatedHostAccessTokenProvider` class.

### Added
- `ICompositeAuthenticator` interface.

## [0.10.0] - 2023-01-19

### Removed
- [Breaking] `EditorActivateFromUrl` runtime class.

### Changed
- [Breaking] `BasePkcePlatformSupport` uses new `AesStringObfuscator` from common package to encrypt and decrypt the refresh token.

## [0.9.1] - 2022-12-08

### Changed
- Manage App Tracking permissions on iOS when logging in captive Safari controller.
- Removed warnings in package

## [0.9.0] - 2022-11-24

### Added
- Support login cancellation from url.
- GetCancellationUri method in IPkcePlatformSupport and all derived platform specific implementations.
- CancelLogin method in IAuthenticator, PkceAuthenticator and CompositeAuthenticator.
- IAuthenticator's AuthenticationState property and AuthenticationStateChange event moved to a separate new IAuthenticationStateProvider.
- All derived IAccessTokenProvider classes implements additional new IAuthenticationStateProvider.
- [Breaking] InitializeAsync public method in PreAuthenticatedHostAccessTokenProvider, BrowserAuthenticatedAccessTokenProvider, CommandLineAccessTokenProvider and PersonalAccessTokenProvider.
- AuthenticationState.AwaitingInitialization enum value.
- BasePkcePlatformSupport.

### Removed
- Internal UnitySynchronizationContextGrabber class.
- [Breaking] LaunchArgumentsParser has moved to the com.unity.digital-twins.common package.
- LinuxPkcePlatformSupport, OsxPlatformSupport and AndroidPlatformSupport.
- PlatformSupportFactory.GetActivatePlatformSupport method.

### Changed
- Fix QueryArgumentHandler<float> registration issue in QueryArgumentsProcessor.Register().
- Manual documentation updates
- [Breaking] Fix PkceAuthenticator constructor override missing required IHttpClient value.
- Renamed the samples' `Common` directory to `Shared`
- Updated the naming convention for the sample configuration and removed the doc-link field

## [0.8.0] - 2022-10-05

### Added
- Support activation from url in playmode using EditorActivateFromUrl monobehaviour.
- EditorActivateFromUrl component to mock deep link

### Changed
- [Breaking] These interfaces does not inherit from IDisposable anymore: IAuthenticator, IInteractiveAuthenticator, IPkceConfigurationProvider, IPkcePlatformSupport, IUserInfoProvider.
- Manual documentation and samples improvements

## [0.7.0] - 2022-09-22

### Changed
- [Breaking] Removed DeepLinkActivated event in IAuthenticator and IActivatePlatformSupport.
- [Breaking] Added a required IUrlRedirectionInterceptor argument in PkceAuthenticator constructors.
- README.md, LICENSE.md.
- Manual documentation improvements
- [Breaking] Support caching and resuming of an activationUrl in WebglActivatePlatformSupport and PkceAuthenticator.

### Fixed
- WebGL login redirection
- OSX standalone build

### Removed
- ICacheStore, FileCacheStore, WebGLCacheStore and BrowserHostInterop.
- AsyncUrlRedirectAwaiter, IUrlRedirectAwaiter, IUrlRedirectionInterceptor, UriSchemeRedirection, UrlRedirectionInterceptor, UrlRedirectResult and UrlRedirectStatus.
- PreBuildValidation, BuildUtils, AppLinksHelper, WindowsBuildPostProcess, OSXPlistParser, InfoPlistPostProcessBuild, XCodePostProcessBuild, AndroidBuildPostProcess.

## [0.6.0] - 2022-09-15

### Added
- BrowserAuthenticatedAccessTokenProvider.

### Changed
- [Breaking] Removed a PkceAuthenticator constructor overload.
- [Breaking] Added a CloudConfiguration to the UserInfoProvider constructor.

## [0.5.0] - 2022-09-08

### Added
- IInteractiveAuthenticator, IPreAuthenticatedAccessTokenProvider, IPkceConfigurationProvider and IUserInfoProvider interface.
- Added manual documentation to package
- Added samples and samples documentation to package

### Changed
- Use dt.unity.com domain hosted proxy page for login in browser.
- [Breaking] UserInfoProvider constructor has new single IServiceHttpClient argument.
- [Breaking] Renamed GetPkceConfiguration method to GetPkceConfigurationAsync in IPkceConfigurationProvider.
- [Breaking] PkceAuthenticator constructor now requires IPkcePlatformSupport, IAppIdProvider and IAppNameProvider.
- [Breaking] Renamed all directory matching namespaces.
- [Breaking] AppConfiguration constructor arguments replaced with IPkceConfigurationProvider in IAuthenticator implementations.
- [Breaking] AppConfiguration renamed to PkceConfiguration.
- [Breaking] Changed parameter UNITY_DT_PERSONAL_ACCESS_TOKEN and UNITY_DT_ACCESS_TOKEN to DT_PERSONAL_ACCESS_TOKEN and DT_ACCESS_TOKEN

### Removed
- [Breaking] AccessTokenProviderFactory, UserInfoProviderFactory.

## [0.4.0] - 2022-08-26

- Use CloudConfiguration static method to retrieve base host address for Cloud endpoints and support DT_CLOUD override from environment variable.
- `IdentityPlayerSettingsProvider` now attempts to resource-load existing settings rather than searching at one specific path.

## [0.3.0] - 2022-08-05

### Added
- Prefix to custom uri scheme.
- Support for WebGL deep link consumption.

### Changed
- [Breaking] DeepLinkActivated method signature modified in IActivatePlaformSupport.

## [0.2.0] - 2022-07-18

Add Personal Access Token and pre-authenticated host support.

## [0.1.1] - 2022-06-08

Fix bug with ActivationUrl found in release 0.1.0.

## [0.1.0] - 2022-06-07

Initial package on Artifactory. Changes are not listed here yet.

## [0.0.1] - 2022-06-01

Initial Release