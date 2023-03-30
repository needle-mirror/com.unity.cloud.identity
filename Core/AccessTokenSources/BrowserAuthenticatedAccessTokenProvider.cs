using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// Returns an `IAccessTokenProvider`implementation that expects an access token from a browser environment.
        /// </summary>
        /// <remarks>The `BrowserAuthenticatedAccessTokenProvider` tries to match running host location with location provided in <see cref="s_LocalStorageKeyNames"/>. Use a single wildcard character (*) to match any host location.</remarks>
        /// <param name="authenticationPlatformSupport">The <see cref="IAuthenticationPlatformSupport"/> that handles <see cref="IKeyValueStore"/> used to cache the access token.</param>
        /// <param name="localStorageKeyNames">A dictionary with browser locations as keys and local storage key name as values.</param>
        public BrowserAuthenticatedAccessTokenProvider(IAuthenticationPlatformSupport authenticationPlatformSupport, Dictionary<string, string> localStorageKeyNames)
        {
            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
            m_LocalStorageKeyName = GetHostAccessTokenFilename(localStorageKeyNames);
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            try
            {
                var browserAccessTokenValue = await m_AuthenticationPlatformSupport.SecretCacheStore.ReadCacheAsync(m_LocalStorageKeyName);
                s_Logger.LogInfo("Access Token provided from a browser environment.");

                AuthenticationState = string.IsNullOrEmpty(browserAccessTokenValue) ? AuthenticationState.LoggedOut : AuthenticationState.LoggedIn;
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

        /// <inheritdoc/>
        public async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var browserAccessTokenValue = await m_AuthenticationPlatformSupport.SecretCacheStore.ReadCacheAsync(m_LocalStorageKeyName);
                if (string.IsNullOrEmpty(browserAccessTokenValue))
                {
                    throw new InvalidOperationException($"Missing '{m_LocalStorageKeyName}' value from browser local storage.");
                }
                return browserAccessTokenValue;
            }
            catch (FileNotFoundException)
            {
                throw new InvalidOperationException($"Missing '{m_LocalStorageKeyName}' value from browser local storage.");
            }
        }

        /// <summary>
        /// Indicates if the <see cref="BrowserAuthenticatedAccessTokenProvider"/> running instance has access to an access token from the browser environment.
        /// </summary>
        /// <returns>If the <see cref="BrowserAuthenticatedAccessTokenProvider"/> running instance has access to an access token.</returns>
        public Task<bool> HasValidPreconditionsAsync()
        {
            if (Uri.TryCreate(m_AuthenticationPlatformSupport.ActivationUrl, UriKind.Absolute, out _))
            {
                return m_AuthenticationPlatformSupport.SecretCacheStore.ValidateFilenameExistsAsync(m_LocalStorageKeyName);
            }
            return Task.FromResult(false);
        }

        string GetHostAccessTokenFilename(Dictionary<string, string> KeyNameDictionary)
        {
            if (Uri.TryCreate(m_AuthenticationPlatformSupport.ActivationUrl, UriKind.Absolute, out Uri browserUri))
            {
                foreach (var kvp in KeyNameDictionary)
                {
                    if (kvp.Key.Equals("*"))
                    {
                        return kvp.Value;
                    }
                    Uri uriExpected;
                    if (Uri.TryCreate(kvp.Key, UriKind.Absolute, out uriExpected)
                    || Uri.TryCreate($"https://{kvp.Key}", UriKind.Absolute, out uriExpected))
                    {
                        var expectedAbsolutePath = $"{uriExpected.Scheme}://{uriExpected.Host}{uriExpected.AbsolutePath}";
                        if (expectedAbsolutePath.Equals($"{browserUri.Scheme}://{browserUri.Host}{browserUri.AbsolutePath}"))
                        {
                            return kvp.Value;
                        }
                        else if (uriExpected.Host.Equals(browserUri.Host))
                        {
                            return kvp.Value;
                        }
                    }
                }
            }
            return "";
        }

    }
}
