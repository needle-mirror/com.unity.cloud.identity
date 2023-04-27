using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using UnityEngine;

namespace Unity.Cloud.Identity.Runtime
{
    /// <summary>
    /// This class contains platform shared logic to handle the authentication flow.
    /// </summary>
    public class BasePkcePlatformSupport : IAuthenticationPlatformSupport
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<BasePkcePlatformSupport>();

        /// <inheritdoc/>
        virtual public IUrlRedirectionInterceptor UrlRedirectionInterceptor { get; internal set; }

        /// <inheritdoc/>
        virtual public string ActivationUrl { get; internal set; }

        /// <inheritdoc/>
        virtual public Dictionary<string, string> ActivationKeyValue { get; internal set; }

        static readonly string k_CacheStorePath = Application.persistentDataPath;

        /// <inheritdoc/>
        public virtual string GetAppStateOverride() => null;

        /// <inheritdoc/>
        public virtual IKeyValueStore SecretCacheStore { get; } = new FileKeyValueStore(k_CacheStorePath, new AesStringObfuscator(!string.IsNullOrEmpty(UnityCloudPlayerSettings.Instance.AppId) ? UnityCloudPlayerSettings.Instance.AppId : "default"));

        /// <inheritdoc/>
        public virtual IKeyValueStore CodeVerifierCacheStore { get; } = null;

        /// <summary>
        /// The url used to initiate a login operation in the default OS browser.
        /// </summary>
        protected string m_LoginUrl = string.Empty;

        readonly Action<string> m_OpenUrlAction;

        /// <summary>
        /// Creates a BasePkcePlatformSupport that handles app activation from an url or key value pairs.
        /// </summary>
        /// <param name="urlRedirectionInterceptor">An <see cref="IUrlRedirectionInterceptor"/> that manages url redirection interception.</param>
        public BasePkcePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor)
        {
            if (Uri.TryCreate(Application.absoluteURL, UriKind.Absolute, out Uri _))
            {
                s_Logger.LogInfo($"App was activated from url: {Application.absoluteURL}");
                ActivationUrl = Application.absoluteURL;
            }
            // Could hold query params from ActivationURL
            ActivationKeyValue = new Dictionary<string, string>();

            UrlRedirectionInterceptor = urlRedirectionInterceptor;
        }

        internal BasePkcePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, Action<string> openUrlAction)
            : this(urlRedirectionInterceptor)
        {
            m_OpenUrlAction = openUrlAction;
        }

        /// <inheritdoc/>
        public virtual async Task<UrlRedirectResult> OpenUrlAndWaitForRedirectAsync(string url, List<string> awaitedQueryArguments = null)
        {
            m_LoginUrl = url;
            s_Logger.LogInfo($"Awaiting redirect on url: {url}");

            OpenUrlAction(url);
            await Task.Delay(50);
            return await UrlRedirectionInterceptor.AwaitRedirectAsync(awaitedQueryArguments);
        }

        void OpenUrlAction(string url)
        {
            if (m_OpenUrlAction != null)
            {
                m_OpenUrlAction(url);
            }
            else
            {
                Application.OpenURL(url);
            }
        }

        /// <inheritdoc/>
        public virtual string GetRedirectUri(string operation = null)
        {
            var operationPath = string.IsNullOrEmpty(operation) ? string.Empty : $"/{operation}";
            return $"{UriSchemeRedirection.s_UriSchemePrefix}{UnityCloudPlayerSettings.Instance.AppName}://implicit/callback{operationPath}";
        }

        /// <inheritdoc/>
        public virtual string GetCancellationUri()
        {
            if (string.IsNullOrEmpty(m_LoginUrl))
                throw new InvalidOperationException("No cancellation Uri available. Awaiting login operation to be initiated.");

            var loginHost = new Uri(m_LoginUrl).Host;
            return $"https://{loginHost}?code=none&state=cancelled";
        }

        /// <inheritdoc/>
        public virtual void ProcessActivationUrl(List<string> awaitedQueryArguments = null)
        {
            if (!string.IsNullOrEmpty(ActivationUrl))
            {
                UrlRedirectionInterceptor.InterceptAwaitedUrl(ActivationUrl, awaitedQueryArguments);
                // Only process once
                ActivationUrl = string.Empty;
            }
        }

        /// <summary>
        /// No <see cref="UrlRedirectResult"/> expected at app initializing time.
        /// </summary>
        /// <returns>
        /// A null value.
        /// </returns>
        public virtual UrlRedirectResult? GetRedirectionResult()
        {
            return null;
        }
    }
}
