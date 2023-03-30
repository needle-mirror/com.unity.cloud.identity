using System;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// An <see cref="IAuthenticator"/> implementation that expects a personal access token (PAT) from a provided `-DT_PERSONAL_ACCESS_TOKEN` launch argument or a `DT_PERSONAL_ACCESS_TOKEN` environment variable.
    /// </summary>
    /// <example>
    /// <code source="../../Samples/Documentation/Scripting/PersonalAccessTokenProviderExample.cs" region="PersonalAccessTokenProvider"/>
    /// </example>
    public class PersonalAccessTokenProvider : IAuthenticator
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<PersonalAccessTokenProvider>();

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
        /// The expected key name in launch arguments that holds the personal access token value.
        /// </summary>
        public static readonly string s_PersonalAccessTokenKeyName = "UNITY_CLOUD_PERSONAL_ACCESS_TOKEN";

        readonly IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;

        /// <summary>
        /// Returns an `IAccessTokenProvider` implementation that expects a personal access token (PAT) from a provided `-UNITY_CLOUD_PERSONAL_ACCESS_TOKEN` launch argument or a `UNITY_CLOUD_PERSONAL_ACCESS_TOKEN` environment variable.
        /// </summary>
        /// <remarks>
        /// A Unity user account is required to generate a valid PAT from the Unity Cloud online dashboard.
        /// </remarks>
        /// <param name="authenticationPlatformSupport">The <see cref="IAuthenticationPlatformSupport"/> that handles PAT injection.</param>
        public PersonalAccessTokenProvider(IAuthenticationPlatformSupport authenticationPlatformSupport)
        {
            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
        }

        /// <inheritdoc/>
        public Task InitializeAsync()
        {
            var keyName = $"-{s_PersonalAccessTokenKeyName}";
            // If launch arguments key value pairs contains the token key name
            if (m_AuthenticationPlatformSupport != null && m_AuthenticationPlatformSupport.ActivationKeyValue.Count > 0 && m_AuthenticationPlatformSupport.ActivationKeyValue.ContainsKey(keyName))
            {
                s_Logger.LogInfo($"Personal Access Token provided from CLI -{s_PersonalAccessTokenKeyName} key value pair");
                m_AccessToken = m_AuthenticationPlatformSupport.ActivationKeyValue[keyName];
                AuthenticationState = AuthenticationState.LoggedIn;
            }
            else
            {
                // Otherwise look at Environment variables for value
                var envVarPersonalAccessTokenValue = Environment.GetEnvironmentVariable(s_PersonalAccessTokenKeyName);
                if (!string.IsNullOrEmpty(envVarPersonalAccessTokenValue))
                {
                    s_Logger.LogInfo($"Personal Access Token provided from { s_PersonalAccessTokenKeyName} environment variable");
                    m_AccessToken = envVarPersonalAccessTokenValue;
                    AuthenticationState = AuthenticationState.LoggedIn;
                    if (m_AuthenticationPlatformSupport != null && !string.IsNullOrEmpty(m_AuthenticationPlatformSupport.ActivationUrl))
                    {
                        m_AuthenticationPlatformSupport.UrlRedirectionInterceptor.InterceptAwaitedUrl(m_AuthenticationPlatformSupport.ActivationUrl);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Cannot Initialize PersonalAccessTokenProvider. Missing -{s_PersonalAccessTokenKeyName} value in launch arguments or {s_PersonalAccessTokenKeyName} in environment variables.");
                }
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<string> GetAccessTokenAsync()
        {
            return Task.FromResult(m_AccessToken);
        }

        /// <summary>
        /// Indicates if the <see cref="PersonalAccessTokenProvider"/> running instance has detected injection of a personal access token.
        /// </summary>
        /// <returns>If the <see cref="PersonalAccessTokenProvider"/> running instance  has detected injection of a personal access token.</returns>
        public Task<bool> HasValidPreconditionsAsync()
        {
            if (m_AuthenticationPlatformSupport.ActivationKeyValue.ContainsKey($"-{s_PersonalAccessTokenKeyName}"))
                return Task.FromResult(true);

            var personalAccessTokenEnvVarValue = Environment.GetEnvironmentVariable(s_PersonalAccessTokenKeyName);
            return Task.FromResult(!string.IsNullOrEmpty(personalAccessTokenEnvVarValue));
        }
    }
}
