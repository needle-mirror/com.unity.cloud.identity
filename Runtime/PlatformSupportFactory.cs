using System;
using UnityEngine;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;

namespace Unity.Cloud.Identity.Runtime
{

    /// <summary>
    /// A static factory that handles instantiation of platform-specific IActivatePlatformSupport and IAuthenticationPlatformSupport.
    /// </summary>
    public static class PlatformSupportFactory
    {
        /// <summary>
        /// A static factory that handles instantiation of a platform-specific <see cref="IAuthenticationPlatformSupport"/>.
        /// </summary>
        /// <param name="urlRedirectionInterceptor">
        /// An optional IUrlRedirectionInterceptor instance.
        /// If not set, the  <see cref="IAuthenticationPlatformSupport"/> instance generated will use default internal implementation.
        /// </param>
        /// <returns>
        /// A platform-specific <see cref="IAuthenticationPlatformSupport"/> instance.
        /// </returns>
        /// <exception cref="NotImplementedException">Throws a NotImplementedException if current execution platform cannot be determined.</exception>
        public static IAuthenticationPlatformSupport GetAuthenticationPlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor = null, IAppIdProvider appIdProvider = null, IAppNameProvider appNameProvider = null, string cacheStorePath = null)
        {
            urlRedirectionInterceptor ??= UrlRedirectionInterceptor.GetInstance();

            appIdProvider ??= UnityCloudPlayerSettings.Instance;
            appNameProvider ??= UnityCloudPlayerSettings.Instance;
            cacheStorePath ??= Application.persistentDataPath;

            IUrlProcessor urlProcessor = new UrlProcessor();

#if UNITY_EDITOR
            return new EditorPkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNameProvider, cacheStorePath);
#elif UNITY_STANDALONE_WIN
            return new WindowsPkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNameProvider, cacheStorePath);
#elif UNITY_STANDALONE_OSX
            return new BasePkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNameProvider, cacheStorePath);
#elif UNITY_STANDALONE_LINUX
            return new BasePkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNameProvider, cacheStorePath);
#elif UNITY_IOS
            return new IosPkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNameProvider, cacheStorePath);
#elif UNITY_ANDROID
            return new BasePkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNameProvider, cacheStorePath);
#elif UNITY_WEBGL
            return new WebglPkcePlatformSupport(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNameProvider, cacheStorePath, Application.absoluteURL);
#else
            throw new NotImplementedException("No PKCE platform support found for the current platform.");
#endif
        }
    }
}
