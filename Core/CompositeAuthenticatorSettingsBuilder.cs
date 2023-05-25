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
        readonly IHttpClient m_HttpClient;
        readonly IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;
        readonly ServiceHostConfiguration m_ServiceHostConfiguration;

        internal readonly List<IAuthenticator> m_Authenticators = new List<IAuthenticator>();

        /// <summary>
        /// Creates a <see cref="CompositeAuthenticatorSettingsBuilder"/> that builds a <see cref="CompositeAuthenticatorSettings"/> to inject into the <see cref="CompositeAuthenticator"/>.
        /// </summary>
        public CompositeAuthenticatorSettingsBuilder(IHttpClient httpClient, IAuthenticationPlatformSupport authenticationPlatformSupport, ServiceHostConfiguration serviceHostConfiguration)
        {
            m_HttpClient = httpClient;
            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
            m_ServiceHostConfiguration = serviceHostConfiguration;
        }

        /// <summary>
        /// Adds a default <see cref="PkceAuthenticator"/> to the list of <see cref="IAuthenticator"/>.
        /// </summary>
        /// <returns>The modified <see cref="CompositeAuthenticatorSettingsBuilder"/>.</returns>
        public CompositeAuthenticatorSettingsBuilder AddDefaultPkceAuthenticator(IAppIdProvider appIdProvider, IAppNameProvider appNameProvider)
        {
            m_Authenticators.Add(new PkceAuthenticator(m_AuthenticationPlatformSupport, m_HttpClient, appIdProvider, appNameProvider, m_ServiceHostConfiguration));
            return this;
        }

        /// <summary>
        /// Adds a default <see cref="PersonalAccessTokenProvider"/> to the list of <see cref="IAuthenticator"/>.
        /// </summary>
        /// <returns>The modified <see cref="CompositeAuthenticatorSettingsBuilder"/>.</returns>
        public CompositeAuthenticatorSettingsBuilder AddDefaultPersonalAccessTokenProvider()
        {
            m_Authenticators.Add(new PersonalAccessTokenProvider(m_AuthenticationPlatformSupport));
            return this;
        }

        /// <summary>
        /// Adds a default <see cref="BrowserAuthenticatedAccessTokenProvider"/> to the list of <see cref="IAuthenticator"/>.
        /// </summary>
        /// <returns>The modified <see cref="CompositeAuthenticatorSettingsBuilder"/>.</returns>
        public CompositeAuthenticatorSettingsBuilder AddDefaultBrowserAuthenticatedAccessTokenProvider(Dictionary<string, string> localStorageKeyNames = null)
        {
            localStorageKeyNames = localStorageKeyNames == null ? new Dictionary<string, string>() { { "*", "genesis-access-token" } } : localStorageKeyNames;
            m_Authenticators.Add(new BrowserAuthenticatedAccessTokenProvider(m_AuthenticationPlatformSupport, localStorageKeyNames));
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
