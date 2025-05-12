using System;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// Builds a <see cref="ServiceAccountAuthenticatorSettings"/> to inject into the <see cref="ServiceAccountAuthenticator"/>.
    /// </summary>
    public class ServiceAccountAuthenticatorSettingsBuilder
    {
        IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;
        IServiceHostResolver m_ServiceHostResolver;
        IHttpClient m_HttpClient;
        IAccessTokenExchanger<ServiceAccountBase64EncodedCredentials, UnityServicesToken> m_AccessTokenExchanger;
        IAppIdProvider m_AppIdProvider;
        IJwtDecoder m_JwtDecoder;

        /// <summary>
        /// Adds an <see cref="IAuthenticationPlatformSupport"/> to the authenticator settings.
        /// </summary>
        /// <param name="authenticationPlatformSupport">The <see cref="IAuthenticationPlatformSupport"/> ionstance.</param>
        /// <returns>The modified <see cref="ServiceAccountAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public ServiceAccountAuthenticatorSettingsBuilder AddAuthenticationPlatformSupport(IAuthenticationPlatformSupport authenticationPlatformSupport)
        {
            ThrowIfNull(authenticationPlatformSupport, nameof(authenticationPlatformSupport));
            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
            return this;
        }

        /// <summary>
        /// Adds an <see cref="IServiceHostResolver"/> to the authenticator settings.
        /// </summary>
        /// <param name="serviceHostResolver">The <see cref="IServiceHostResolver"/> ionstance.</param>
        /// <returns>The modified <see cref="ServiceAccountAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public ServiceAccountAuthenticatorSettingsBuilder AddServiceHostResolver(IServiceHostResolver serviceHostResolver)
        {
            ThrowIfNull(serviceHostResolver, nameof(serviceHostResolver));
            m_ServiceHostResolver = serviceHostResolver;
            return this;
        }

        /// <summary>
        /// Adds the default Service Account credentials exchanger to the authenticator settings.
        /// </summary>
        /// <param name="httpClient">The <see cref="IHttpClient"/> to inject in the Service Account credentials exchanger.</param>
        /// <param name="pkceConfigurationProvider">The <see cref="IPkceConfigurationProvider"/> to inject in the Service Account credentials exchanger.</param>
        /// <returns>The modified <see cref="ServiceAccountAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public ServiceAccountAuthenticatorSettingsBuilder AddDefaultServiceAccountCredentialsExchanger(
            IHttpClient httpClient,
            IPkceConfigurationProvider pkceConfigurationProvider)
        {
            ThrowIfNull(httpClient, nameof(httpClient));
            ThrowIfNull(pkceConfigurationProvider, nameof(pkceConfigurationProvider));

            m_AccessTokenExchanger = new ServiceAccountCredentialsToUnityServicesTokenExchanger(httpClient, pkceConfigurationProvider);
            return this;
        }

        /// <summary>
        /// Adds a Service Account credentials exchanger to the authenticator settings.
        /// </summary>
        /// <param name="accessTokenExchanger">The <see cref="IAccessTokenExchanger{T, T}"/> to add to the authenticator settings.</param>
        /// <returns>The modified <see cref="ServiceAccountAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public ServiceAccountAuthenticatorSettingsBuilder AddServiceAccountCredentialsExchanger(
            IAccessTokenExchanger<ServiceAccountBase64EncodedCredentials, UnityServicesToken> accessTokenExchanger)
        {
            ThrowIfNull(accessTokenExchanger, nameof(accessTokenExchanger));

            m_AccessTokenExchanger = accessTokenExchanger;
            return this;
        }

        /// <summary>
        /// Adds an <see cref="IAppIdProvider"/> to the authenticator settings.
        /// </summary>
        /// <param name="appIdProvider">The <see cref="IAppIdProvider"/> to provide with the app Id.</param>
        /// <returns>The modified <see cref="ServiceAccountAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public ServiceAccountAuthenticatorSettingsBuilder AddAppIdProvider(IAppIdProvider appIdProvider)
        {
            ThrowIfNull(appIdProvider, nameof(appIdProvider));
            m_AppIdProvider = appIdProvider;
            return this;
        }

        /// <summary>
        /// Adds a <see cref="IHttpClient"/> to the authenticator settings.
        /// </summary>
        /// <param name="httpClient">The <see cref="IHttpClient"/> to add to the authenticator settings.</param>
        /// <returns>The modified <see cref="ServiceAccountAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public ServiceAccountAuthenticatorSettingsBuilder AddHttpClient(IHttpClient httpClient)
        {
            ThrowIfNull(httpClient, nameof(httpClient));

            m_HttpClient = httpClient;
            return this;
        }

        /// <summary>
        /// Adds a <see cref="IJwtDecoder"/> to the authenticator settings.
        /// </summary>
        /// <param name="jwtDecoder">The <see cref="IJwtDecoder"/> to add to the authenticator settings.</param>
        /// <returns>The modified <see cref="ServiceAccountAuthenticatorSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        internal ServiceAccountAuthenticatorSettingsBuilder AddJwtDecoder(IJwtDecoder jwtDecoder)
        {
            ThrowIfNull(jwtDecoder, nameof(jwtDecoder));

            m_JwtDecoder = jwtDecoder;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="ServiceAccountAuthenticatorSettings"/> to inject into the <see cref="ServiceAccountAuthenticator"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceAccountAuthenticatorSettings"/>.
        /// </returns>
        public ServiceAccountAuthenticatorSettings Build()
        {
            m_JwtDecoder ??= new JwtDecoder();

            ValidateRequiredSettings();

            return new ServiceAccountAuthenticatorSettings(
                m_AuthenticationPlatformSupport,
                m_AccessTokenExchanger,
                m_ServiceHostResolver,
                m_HttpClient,
                m_AppIdProvider,
                m_JwtDecoder
                );
        }

        /// <summary>
        /// Validates that all required settings for building a <see cref="ServiceAccountAuthenticatorSettings"/> have been added to the builder.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if any of the required settings are null.</exception>
        void ValidateRequiredSettings()
        {
            var settingsAreMissing = false;
            var missingSettingsMessage = $"The following settings must be set in order to build a {nameof(ServiceAccountAuthenticator)}: ";

            ValidateRequiredSetting(m_AuthenticationPlatformSupport, ref missingSettingsMessage, ref settingsAreMissing);
            ValidateRequiredSetting(m_ServiceHostResolver, ref missingSettingsMessage, ref settingsAreMissing);
            ValidateRequiredSetting(m_HttpClient, ref missingSettingsMessage, ref settingsAreMissing);

            // If any settings are missing, throw an exception.
            if (settingsAreMissing)
                throw new ArgumentNullException(missingSettingsMessage);
        }

        /// <summary>
        /// Validate if the setting is null, and append to the exception message if it is.
        /// </summary>
        /// <param name="setting">The setting to validate.</param>
        /// <param name="nullSettingsMessage">The exception message to append to.</param>
        /// <param name="anySettingsNull">Whether a setting is already null.</param>
        /// <typeparam name="T"></typeparam>
        static void ValidateRequiredSetting<T>(T setting, ref string nullSettingsMessage, ref bool anySettingsNull) where T : class
        {
            if (setting == null)
            {
                if (anySettingsNull)
                    nullSettingsMessage += ", ";
                nullSettingsMessage += typeof(T).Name;

                anySettingsNull = true;
            }
        }

        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> exception if the given field is null.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <param name="parameterName">The name of the parameter to include in the exception.</param>
        /// <exception cref="ArgumentNullException">Thrown if the parameter is null.</exception>
        static void ThrowIfNull(object parameter, string parameterName)
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName);
        }
    }
}
