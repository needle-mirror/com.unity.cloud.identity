using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Unity.Cloud.Common;
using UnityEngine;

namespace Unity.Cloud.Identity.Runtime
{

    /// <summary>
    /// This class handles iOS platform-specific features in the authentication flow.
    /// </summary>
    public class IosPkcePlatformSupport : BasePkcePlatformSupport
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void LaunchSafariWebViewUrl(string url);
        [DllImport("__Internal")]
        static extern void DismissSafariWebView();
        [DllImport("__Internal")]
        static extern void RequestAppTrackingAuthorization();
#endif
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<IosPkcePlatformSupport>();

        /// <inheritdoc/>
        public IosPkcePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNameProvider appNameProvider, IAppNamespaceProvider appNamespaceProvider, string cacheStorePath, string activationUrl = null)
            : base(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNameProvider, appNamespaceProvider, cacheStorePath, activationUrl)
        {
#if UNITY_IOS && !UNITY_EDITOR
            RequestAppTrackingAuthorization();
#endif
        }

        /// <summary>
        /// Creates an awaitable Task that opens an url in a browser and completes when response is intercepted, validated and returns a <see cref="UrlRedirectResult"/>.
        /// </summary>
        /// <param name="url">The url to open. It must trigger a redirection to the Uri referenced by <see cref="BasePkcePlatformSupport.GetRedirectUri"/>.</param>
        /// <param name="awaitedQueryArguments">The list of query arguments to validate when receiving the awaited callback url.</param>
        /// <returns>
        /// A Task that results in a <see cref="UrlRedirectResult"/> when completed.
        /// </returns>
        public override async Task<UrlRedirectResult> OpenUrlAndWaitForRedirectAsync(string url, List<string> awaitedQueryArguments = null)
        {
            m_LoginUrl = url;
            s_Logger.LogInfo($"Awaiting redirect on url: {url}");
#if UNITY_IOS && !UNITY_EDITOR
            LaunchSafariWebViewUrl(url);
#endif
            await Task.Delay(50);
            var result = await UrlRedirectionInterceptor.AwaitRedirectAsync(awaitedQueryArguments);

#if UNITY_IOS && !UNITY_EDITOR
            DismissSafariWebView();
#endif

            return result;
        }
    }
}
