using System;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{

    /// <summary>
    /// An <see cref="IAuthenticator"/> implementation that expects an access token value from a provided `-UNITY_CLOUD_ACCESS_TOKEN` launch argument.
    /// </summary>
    /// <example>
    /// <code source="../../Samples/Documentation/Scripting/CommandLineAccessTokenProviderExample.cs" region="CommandLineAccessTokenProvider"/>
    /// </example>
    public class CommandLineAccessTokenProvider : IAuthenticator
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<CommandLineAccessTokenProvider>();

        string m_AccessToken = string.Empty;

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

        /// <summary>
        /// The expected key name in launch arguments that holds the access token value.
        /// </summary>
        public static readonly string s_AccessTokenKeyName = "UNITY_CLOUD_ACCESS_TOKEN";

        readonly IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;

        /// <summary>
        /// Returns an `IAccessTokenProvider` implementation that expects an access token value from a provided `-UNITY_CLOUD_ACCESS_TOKEN` launch argument.
        /// </summary>
        /// <param name="authenticationPlatformSupport">The <see cref="IAuthenticationPlatformSupport"/> that handles launch arguments.</param>
        public CommandLineAccessTokenProvider(IAuthenticationPlatformSupport authenticationPlatformSupport)
        {
            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
        }

        /// <inheritdoc/>
        public Task InitializeAsync()
        {
            var keyName = $"-{s_AccessTokenKeyName}";
            // If launch arguments key value pairs contains the token key name
            if (m_AuthenticationPlatformSupport.ActivationKeyValue.Count > 0 && m_AuthenticationPlatformSupport.ActivationKeyValue.ContainsKey(keyName))
            {
                s_Logger.LogInfo($"Access token provided from CLI -{s_AccessTokenKeyName} key value pair.");
                m_AccessToken = m_AuthenticationPlatformSupport.ActivationKeyValue[keyName];
                AuthenticationState = AuthenticationState.LoggedIn;
                if (!string.IsNullOrEmpty(m_AuthenticationPlatformSupport.ActivationUrl))
                {
                    m_AuthenticationPlatformSupport.UrlRedirectionInterceptor.InterceptAwaitedUrl(m_AuthenticationPlatformSupport.ActivationUrl);
                }
            }
            else
            {
                throw new InvalidOperationException($"Cannot Initialize CommandLineAccessTokenProvider. Missing {s_AccessTokenKeyName} value launch arguments.");
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<string> GetAccessTokenAsync()
        {
            return Task.FromResult(m_AccessToken);
        }

        /// <summary>
        /// Indicates if the <see cref="CommandLineAccessTokenProvider"/> running instance has detected command line injection of an access token.
        /// </summary>
        /// <returns>If the <see cref="CommandLineAccessTokenProvider"/> running instance  has detected command line injection of an access token.</returns>
        public Task<bool> HasValidPreconditionsAsync()
        {
            return Task.FromResult(m_AuthenticationPlatformSupport.ActivationKeyValue.ContainsKey($"-{s_AccessTokenKeyName}"));
        }
    }
}
