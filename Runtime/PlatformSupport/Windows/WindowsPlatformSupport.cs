using System;
using Unity.Cloud.Common;
using UnityEngine;

namespace Unity.Cloud.Identity.Runtime
{
    /// <summary>
    /// This class contains Windows standalone platform-specific logic to handle app activation from an url or key value pairs.
    /// </summary>
    public class WindowsActivatePlatformSupport : BasePkcePlatformSupport
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<WindowsActivatePlatformSupport>();

        /// <summary>
        /// Creates a WindowsActivatePlatformSupport that handles app activation from an url or key value pairs.
        /// </summary>
        /// <param name="urlRedirectionInterceptor">An <see cref="IUrlRedirectionInterceptor"/> that manages url redirection interception.</param>
        public WindowsActivatePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNameProvider appNameProvider, string cacheStorePath, string activationUrl = null)
            : base(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNameProvider, cacheStorePath, activationUrl)
        {
            // Check deep link on startup
            var launchArgumentsParser = new LaunchArgumentsParser();

            if (Uri.TryCreate(launchArgumentsParser.ActivationUrl, UriKind.Absolute, out Uri _))
            {
                s_Logger.LogInfo($"App was activated from url: {launchArgumentsParser.ActivationUrl}");
                ActivationUrl = launchArgumentsParser.ActivationUrl;
            }

            // Could hold query params from ActivationURL or launch arguments
            ActivationKeyValue = launchArgumentsParser.ActivationKeyValues;
        }
    }

    /// <summary>
    /// This class handles Windows standalone platform-specific features in the authentication flow.
    /// </summary>
    public class WindowsPkcePlatformSupport : WindowsActivatePlatformSupport
    {
        /// <summary>
        /// Get a string value override for the default random state used in the authentication flow.
        /// </summary>
        /// <returns>
        /// A string value corresponding to the main window pointer of the app.
        /// </returns>
        public override string GetAppStateOverride() => UrlRedirectionInterceptor.GetRedirectProcessId();

        /// <summary>
        /// Creates a WindowsPkcePlatformSupport instance using an IUrlRedirectionInterceptor.
        /// </summary>
        /// <param name="urlRedirectionInterceptor">The IUrlRedirectionInterceptor that will intercept the authentication response sent after completing a login operation in browser.</param>
        public WindowsPkcePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNameProvider appNameProvider, string cacheStorePath, string activationUrl = null)
            : base(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNameProvider, cacheStorePath, activationUrl)
        {
        }
    }
}
