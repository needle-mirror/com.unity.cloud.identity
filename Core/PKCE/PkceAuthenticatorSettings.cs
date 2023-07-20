
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// Creates the <see cref="PkceAuthenticatorSettings"/> required to inject in a <see cref="PkceAuthenticator"/>.
    /// </summary>
    public readonly struct PkceAuthenticatorSettings
    {
        /// <summary>
        /// The <see cref="IAuthenticationPlatformSupport"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IAuthenticationPlatformSupport AuthenticationPlatformSupport;

        /// <summary>
        /// The <see cref="IPkceConfigurationProvider"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IPkceConfigurationProvider PkceConfigurationProvider;

        /// <summary>
        /// The <see cref="Common.ServiceHostConfiguration"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IServiceHostResolver ServiceHostResolver;

        /// <summary>
        /// The <see cref="IAccessTokenExchanger{TInput,TOutput}"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IAccessTokenExchanger<DeviceToken, UnityServicesToken> AccessTokenExchanger;

        /// <summary>
        /// The <see cref="IPkceRequestHandler"/> to use for PKCE authentication.
        /// </summary>
        internal readonly IPkceRequestHandler PkceRequestHandler;

        /// <summary>
        /// Creates a <see cref="PkceAuthenticatorSettings"/> to inject in a <see cref="PkceAuthenticator"/>.
        /// </summary>
        internal PkceAuthenticatorSettings(
            IAuthenticationPlatformSupport authenticationPlatformSupport,
            IPkceConfigurationProvider pkceConfigurationProvider,
            IPkceRequestHandler pkceRequestHandler,
            IAccessTokenExchanger<DeviceToken, UnityServicesToken> accessTokenExchanger,
            IServiceHostResolver serviceHostResolver
            )
        {
            AuthenticationPlatformSupport = authenticationPlatformSupport;
            PkceConfigurationProvider = pkceConfigurationProvider;
            PkceRequestHandler = pkceRequestHandler;
            AccessTokenExchanger = accessTokenExchanger;
            ServiceHostResolver = serviceHostResolver;
        }
    }
}
