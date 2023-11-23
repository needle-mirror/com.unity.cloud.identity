using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{

    /// <summary>
    /// An <see cref="IAuthenticator"/> implementation that expects an access token from a browser environment.
    /// </summary>
    /// <example>
    /// <code source="../../Samples/Documentation/Scripting/BrowserAuthenticatedAccessTokenProviderExample.cs" region="BrowserAuthenticatedAccessTokenProvider"/>
    /// </example>
    public class BrowserAuthenticatedAccessTokenProvider : IAuthenticator
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<BrowserAuthenticatedAccessTokenProvider>();

        /// <summary>
        /// Default list of supported domains with corresponding localStorage key names.
        /// </summary>
        public static Dictionary<string, string> DefaultLocalStorageKeyNames =>
            new()
            {
                { "dev.staging.cloud.unity.com", "genesis-access-token-staging" },
                { "staging.cloud.unity.com", "genesis-access-token-staging" },
                { "cloud.unity.com", "genesis-access-token" }
            };

        readonly string m_LocalStorageKeyName;

        AuthenticationState m_AuthenticationState = AuthenticationState.AwaitingInitialization;

        /// <inheritdoc/>
        public event Action<AuthenticationState> AuthenticationStateChanged;

        /// <inheritdoc/>
        public AuthenticationState AuthenticationState
        {
            get => m_AuthenticationState;
            private set
            {
                m_AuthenticationState = value;
                AuthenticationStateChanged?.Invoke(m_AuthenticationState);
            }
        }

        readonly IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;

        readonly IAccessTokenExchanger<string, UnityServicesToken> m_UnityServicesTokenExchanger;
        UnityServicesToken m_UnityServicesToken;

        string m_SessionBrowserAccessTokenValue;

        readonly IPkceRequestHandler m_PkceRequestHandler;
        readonly IOrganizationRepository m_OrganizationRepository;

        IAuthenticatedUserInfoProvider m_AuthenticatedUserInfoProvider;

        /// <summary>
        /// Returns an <see cref="IAuthenticator"/> implementation that expects an access token from a browser environment.
        /// </summary>
        /// <remarks>The `BrowserAuthenticatedAccessTokenProvider` tries to match running host location with location provided in <see cref="localStorageKeyNames"/>. Use a single wildcard character (*) to match any host location.</remarks>
        /// <param name="pkceAuthenticatorSettings">The <see cref="PkceAuthenticatorSettings"/> that contains all PKCE authentication classes</param>
        /// <param name="localStorageKeyNames">A dictionary with browser locations as keys and local storage key name as values.</param>
        /// <param name="organizationsRepository">An optional <see cref="IOrganizationRepository"/>.</param>
        public BrowserAuthenticatedAccessTokenProvider(PkceAuthenticatorSettings pkceAuthenticatorSettings, Dictionary<string, string> localStorageKeyNames = null, IOrganizationRepository organizationsRepository = null)
        {
            localStorageKeyNames ??= DefaultLocalStorageKeyNames;
            m_AuthenticationPlatformSupport = pkceAuthenticatorSettings.AuthenticationPlatformSupport;
            m_PkceRequestHandler = pkceAuthenticatorSettings.PkceRequestHandler;
            m_UnityServicesTokenExchanger = pkceAuthenticatorSettings.AccessTokenExchanger;

            m_LocalStorageKeyName = GetHostAccessTokenFilename(localStorageKeyNames);

            m_OrganizationRepository = organizationsRepository ?? new AuthenticatorOrganizationRepository(
                new ServiceHttpClient(pkceAuthenticatorSettings.HttpClient, this,
                    pkceAuthenticatorSettings.AppIdProvider), pkceAuthenticatorSettings.ServiceHostResolver);
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            try
            {
                m_SessionBrowserAccessTokenValue = await m_AuthenticationPlatformSupport.SecretCacheStore.ReadCacheAsync(m_LocalStorageKeyName);

                if (!string.IsNullOrEmpty(m_SessionBrowserAccessTokenValue))
                {
                    s_Logger.LogInfo("genesis Access Token provided from a browser environment.");
                   await RefreshAuthenticatedUserInfo();
                }

                AuthenticationState = string.IsNullOrEmpty(m_SessionBrowserAccessTokenValue) ? AuthenticationState.LoggedOut : AuthenticationState.LoggedIn;

                if (!string.IsNullOrEmpty(m_AuthenticationPlatformSupport.ActivationUrl))
                {
                    m_AuthenticationPlatformSupport.UrlRedirectionInterceptor.InterceptAwaitedUrl(m_AuthenticationPlatformSupport.ActivationUrl);
                }
            }
            catch (FileNotFoundException)
            {
                s_Logger.LogInfo("Token could not be found in cache.");
                AuthenticationState = AuthenticationState.LoggedOut;
            }
        }

        async Task RefreshAuthenticatedUserInfo()
        {
            m_AuthenticatedUserInfoProvider = await m_PkceRequestHandler.GetAuthenticatedUserInfoAsync(m_SessionBrowserAccessTokenValue);
            m_UnityServicesToken = await m_UnityServicesTokenExchanger.ExchangeAsync(m_SessionBrowserAccessTokenValue);
            m_AuthenticationPlatformSupport.ExportServiceAuthorizerToken("Bearer", m_UnityServicesToken.AccessToken);
        }

        /// <inheritdoc cref="IServiceAuthorizer.AddAuthorization"/>
        public async Task AddAuthorization(HttpHeaders headers)
        {
            try
            {
                var browserAccessTokenValue = await m_AuthenticationPlatformSupport.SecretCacheStore.ReadCacheAsync(m_LocalStorageKeyName);
                if (string.IsNullOrEmpty(browserAccessTokenValue))
                {
                    throw new InvalidOperationException($"Missing '{m_LocalStorageKeyName}' value from browser local storage.");
                }

                if (!browserAccessTokenValue.Equals(m_SessionBrowserAccessTokenValue))
                {
                    m_SessionBrowserAccessTokenValue = browserAccessTokenValue;
                    await RefreshAuthenticatedUserInfo();
                }

                headers.AddAuthorization(m_UnityServicesToken.AccessToken, ServiceHeaderUtils.k_BearerScheme);
            }
            catch (FileNotFoundException)
            {
                throw new InvalidOperationException($"Missing '{m_LocalStorageKeyName}' value from browser local storage.");
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IOrganization>> ListOrganizationsAsync()
        {
            return await m_OrganizationRepository.ListOrganizationsAsync();
        }

        /// <summary>
        /// Indicates if the <see cref="BrowserAuthenticatedAccessTokenProvider"/> running instance has access to an access token from the browser environment.
        /// </summary>
        /// <returns>If the <see cref="BrowserAuthenticatedAccessTokenProvider"/> running instance has access to an access token.</returns>
        public Task<bool> HasValidPreconditionsAsync()
        {
            if (!string.IsNullOrEmpty(m_LocalStorageKeyName))
            {
                return m_AuthenticationPlatformSupport.SecretCacheStore.ValidateFilenameExistsAsync(m_LocalStorageKeyName);
            }
            return Task.FromResult(false);
        }

        string GetHostAccessTokenFilename(Dictionary<string, string> KeyNameDictionary)
        {
            if (HasValidUrl(new[]{ m_AuthenticationPlatformSupport.ActivationUrl }, out Uri browserUri))
            {
                foreach (var kvp in KeyNameDictionary)
                {
                    var prefixKey = $"https://{kvp.Key}";
                    if (HasValidUrl( new[]{ kvp.Key, prefixKey }, out Uri uriExpected))
                    {
                        var expectedAbsolutePath = $"{uriExpected.Scheme}://{uriExpected.Host}{uriExpected.AbsolutePath}";
                        if (expectedAbsolutePath.Equals($"{browserUri.Scheme}://{browserUri.Host}{browserUri.AbsolutePath}"))
                        {
                            return kvp.Value;
                        }
                        if (uriExpected.Host.Equals(browserUri.Host))
                        {
                            return kvp.Value;
                        }
                    }
                }
            }
            return null;
        }

        bool HasValidUrl(string[] urls, out Uri validUri)
        {
            validUri = default;
            foreach (var url in urls)
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out validUri)
                    && (validUri.Host.EndsWith("dashboard.unity3d.com") || validUri.Host.EndsWith("dashboard.unity.com")))
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc/>
        public string GetUserInfo(string key)
        {
            return m_AuthenticatedUserInfoProvider.GetUserInfo(key);
        }
    }
}
