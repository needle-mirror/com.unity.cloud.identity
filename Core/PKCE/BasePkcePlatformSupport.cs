using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
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
        virtual public string ActivationUrl { get; protected set; }

        /// <inheritdoc/>
        virtual public Dictionary<string, string> ActivationKeyValue { get; protected set; }

        string m_CacheStorePath { get; }

        /// <inheritdoc/>
        public virtual string GetAppStateOverride() => null;

        /// <inheritdoc/>
        public virtual IKeyValueStore SecretCacheStore { get; }

        /// <inheritdoc/>
        public virtual IKeyValueStore CodeVerifierCacheStore { get; } = null;

        /// <summary>
        /// The url used to initiate a login operation in the default OS browser.
        /// </summary>
        protected string m_LoginUrl = string.Empty;

        protected readonly IUrlProcessor m_UrlProcessor;
        readonly IAppIdProvider m_AppIdProvider;
        readonly IAppNameProvider m_AppNameProvider;

        /// <summary>
        /// Creates a BasePkcePlatformSupport that handles app activation from an url or key value pairs.
        /// </summary>
        /// <param name="urlRedirectionInterceptor">An <see cref="IUrlRedirectionInterceptor"/> that manages url redirection interception.</param>
        /// <param name="activationUrl">An optional activation URL</param>
        public BasePkcePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNameProvider appNameProvider, string cacheStorePath, string activationUrl = null)
        {
            m_UrlProcessor = urlProcessor;
            m_AppIdProvider = appIdProvider;
            m_AppNameProvider = appNameProvider;
            m_CacheStorePath = cacheStorePath;
            SecretCacheStore = new FileKeyValueStore(m_CacheStorePath, new AesStringObfuscator(!string.IsNullOrEmpty(m_AppIdProvider.GetAppId()) ? m_AppIdProvider.GetAppId() : "default"));
            if (!string.IsNullOrEmpty(activationUrl) && Uri.TryCreate(activationUrl, UriKind.Absolute, out Uri _))
            {
                s_Logger.LogInfo($"App was activated from url: {activationUrl}");
                ActivationUrl = activationUrl;
            }
            // Could hold query params from ActivationURL
            ActivationKeyValue = new Dictionary<string, string>();

            UrlRedirectionInterceptor = urlRedirectionInterceptor;
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

        /// <inheritdoc/>
        public virtual void ExportServiceAuthorizerToken(string type, string token)
        {
        }

        void OpenUrlAction(string url)
        {
            if (m_UrlProcessor != null)
            {
                m_UrlProcessor.ProcessURL(url);
            }
        }

        /// <inheritdoc/>
        public virtual string GetRedirectUri(string operation = null)
        {
            var operationPath = string.IsNullOrEmpty(operation) ? string.Empty : $"/{operation}";
            return $"{UriSchemeRedirection.s_UriSchemePrefix}{m_AppNameProvider.GetAppName()}://implicit/callback{operationPath}";
        }

        /// <inheritdoc/>
        public virtual Task<string> GetRedirectUriAsync(string operation = null)
        {
            return Task.FromResult(GetRedirectUri(operation));
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
