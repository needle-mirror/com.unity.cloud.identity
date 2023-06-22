using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    public class DeviceTokenToUnityServicesTokenExchanger : IAccessTokenExchanger<DeviceToken, UnityServicesToken>
    {

        readonly IHttpClient m_HttpClient;

        static readonly string s_BaseUnityApiUrl = ".unity.com";

        readonly string m_UnityApiUrl = ".unity.com";

        readonly TargetClientIdTokenToUnityServicesTokenExchanger m_TargetClientIdTokenToUnityServicesTokenExchanger;

        /// <summary>
        /// Provides Unity Services token from DeviceToken
        /// </summary>
        public DeviceTokenToUnityServicesTokenExchanger(IHttpClient httpClient, ServiceHostConfiguration serviceHostConfiguration)
        {
            m_HttpClient = httpClient;

            m_TargetClientIdTokenToUnityServicesTokenExchanger =
                new TargetClientIdTokenToUnityServicesTokenExchanger(m_HttpClient, serviceHostConfiguration);

            var environment = serviceHostConfiguration?.ResolveEnvironment().environment;

            m_UnityApiUrl = environment switch
            {
                ServiceEnvironment.Staging => string.Concat("api-staging", s_BaseUnityApiUrl),
                ServiceEnvironment.Test => string.Concat("api-staging", s_BaseUnityApiUrl),
                _ => string.Concat("api", s_BaseUnityApiUrl)
            };
        }

        // PKCE access token returned from Genesis requires a first exchange targeting a specific targetClientId
        // before reaching Unity Services exchange endpoint
        async Task<UnityServicesToken> ExchangeGenesisAccessTokenRequestAsync(string genesisAccessToken, string targetClientId = "ads-publisher")
        {
            var exchangeGenesisTokenRequest = new ExchangeGenesisTokenRequest
            {
                accessToken = genesisAccessToken, grantType = "EXCHANGE_ACCESS_TOKEN", targetClientId = targetClientId
            };
            var stringContent = new StringContent(JsonSerialization.Serialize(exchangeGenesisTokenRequest), Encoding.UTF8,
                    "application/json");
            var clientTargetIdTokenResponse = await m_HttpClient.PostAsync($"https://{m_UnityApiUrl}/v1/oauth2/token/exchange", stringContent);
            var exchangeGenesisAccessTokenResponse = await clientTargetIdTokenResponse.JsonDeserializeAsync<ExchangeGenesisAccessTokenResponse>();

            return await m_TargetClientIdTokenToUnityServicesTokenExchanger.ExchangeAsync(new TargetClientIdToken
                { token = exchangeGenesisAccessTokenResponse.access_token });
        }

        /// <inheritdoc/>
        public async Task<UnityServicesToken> ExchangeAsync(DeviceToken deviceToken)
        {
            return await ExchangeGenesisAccessTokenRequestAsync(deviceToken.AccessToken);
        }
    }
}
