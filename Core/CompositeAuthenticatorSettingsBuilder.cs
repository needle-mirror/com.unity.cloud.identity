using System;
using System.Collections.Generic;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{

    /// <summary>
    /// Creates a <see cref="CompositeAuthenticatorSettingsBuilder"/> that builds a <see cref="CompositeAuthenticatorSettings"/> to inject into the <see cref="CompositeAuthenticator"/>.
    /// </summary>
    public class CompositeAuthenticatorSettingsBuilder
    {
        readonly IAppIdProvider m_AppIdProvider;
        readonly IHttpClient m_HttpClient;
        readonly IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;
        readonly IServiceHostResolver m_ServiceHostResolver;

        internal readonly List<IAuthenticator> m_Authenticators = new List<IAuthenticator>();

        /// <summary>
        /// Creates a <see cref="CompositeAuthenticatorSettingsBuilder"/> that builds a <see cref="CompositeAuthenticatorSettings"/> to inject into the <see cref="CompositeAuthenticator"/>.
        /// </summary>
        public CompositeAuthenticatorSettingsBuilder(IHttpClient httpClient, IAuthenticationPlatformSupport authenticationPlatformSupport, IServiceHostResolver serviceHostResolver)
        {
            m_HttpClient = httpClient;
            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
            m_ServiceHostResolver = serviceHostResolver;
        }

        public CompositeAuthenticatorSettingsBuilder(IHttpClient httpClient, IAuthenticationPlatformSupport authenticationPlatformSupport, IServiceHostResolver serviceHostResolver, IAppIdProvider appIdProvider)
        {
            m_HttpClient = httpClient;
            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
            m_ServiceHostResolver = serviceHostResolver;
            m_AppIdProvider = appIdProvider;
        }

        /// <summary>
        /// Adds a default <see cref="PkceAuthenticator"/> to the list of <see cref="IAuthenticator"/>.
        /// </summary>
        /// <returns>The modified <see cref="CompositeAuthenticatorSettingsBuilder"/>.</returns>
        public CompositeAuthenticatorSettingsBuilder AddDefaultPkceAuthenticator(IAppNameProvider appNameProvider, IAppNamespaceProvider appNamespaceProvider)
        {
            var pkceAuthenticatorSettingsBuilder = new PkceAuthenticatorSettingsBuilder(m_AuthenticationPlatformSupport, m_ServiceHostResolver);
            pkceAuthenticatorSettingsBuilder.AddDefaultConfigurationProviderAndRequestHandler(m_HttpClient, appNameProvider, appNamespaceProvider)
                .AddAppIdProvider(m_AppIdProvider)
                .AddDefaultAccessTokenExchanger(m_HttpClient);

            var pkceAuthenticatorSettings = pkceAuthenticatorSettingsBuilder.Build();

            m_Authenticators.Add(new PkceAuthenticator(pkceAuthenticatorSettings));
            return this;
        }

        /// <summary>
        /// Adds a default <see cref="BrowserAuthenticatedAccessTokenProvider"/> to the list of <see cref="IAuthenticator"/>.
        /// </summary>
        /// <returns>The modified <see cref="CompositeAuthenticatorSettingsBuilder"/>.</returns>
        public CompositeAuthenticatorSettingsBuilder AddDefaultBrowserAuthenticatedAccessTokenProvider(IAppNameProvider appNameProvider, IAppNamespaceProvider appNamespaceProvider, Dictionary<string, string> localStorageKeyNames = null)
        {
            localStorageKeyNames ??= BrowserAuthenticatedAccessTokenProvider.DefaultLocalStorageKeyNames;
            var pkceAuthenticatorSettingsBuilder = new PkceAuthenticatorSettingsBuilder(m_AuthenticationPlatformSupport, m_ServiceHostResolver);
            pkceAuthenticatorSettingsBuilder.AddDefaultConfigurationProviderAndRequestHandler(m_HttpClient, appNameProvider, appNamespaceProvider)
                .AddAppIdProvider(m_AppIdProvider)
                .AddDefaultAccessTokenExchanger(m_HttpClient);

            var pkceAuthenticatorSettings = pkceAuthenticatorSettingsBuilder.Build();

            m_Authenticators.Add(new BrowserAuthenticatedAccessTokenProvider(pkceAuthenticatorSettings, localStorageKeyNames));
            return this;
        }

        /// <summary>
        /// Adds any <see cref="IAuthenticator"/> to the list of <see cref="IAuthenticator"/>.
        /// </summary>
        /// <returns>The modified <see cref="CompositeAuthenticatorSettingsBuilder"/>.</returns>
        public CompositeAuthenticatorSettingsBuilder AddAuthenticator(IAuthenticator authenticator)
        {
            m_Authenticators.Add(authenticator);
            return this;
        }

        /// <summary>
        /// Builds the <see cref="CompositeAuthenticatorSettings"/> to inject into the <see cref="CompositeAuthenticator"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="CompositeAuthenticatorSettings"/>.
        /// </returns>
        public CompositeAuthenticatorSettings Build()
        {
            return new CompositeAuthenticatorSettings(m_Authenticators);
        }
    }
}
