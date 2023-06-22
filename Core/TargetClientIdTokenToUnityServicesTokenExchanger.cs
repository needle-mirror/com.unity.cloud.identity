using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    public class TargetClientIdTokenToUnityServicesTokenExchanger : IAccessTokenExchanger<TargetClientIdToken, UnityServicesToken>
    {
        readonly IHttpClient m_HttpClient;

        static readonly string s_BaseUnityServicesApiUrl = "services.unity.com";
        readonly string m_UnityServicesApiUrl = "services.unity.com";

        /// <summary>
        /// Provides Unity Services token from TargetClientIdToken
        /// </summary>
        public TargetClientIdTokenToUnityServicesTokenExchanger(IHttpClient httpClient, ServiceHostConfiguration serviceHostConfiguration)
        {
            m_HttpClient = httpClient;

            var environment = serviceHostConfiguration?.ResolveEnvironment().environment;
            var provider = serviceHostConfiguration?.ResolveProvider();

            if (provider.Equals(ServiceDomainProvider.Azure))
            {
                m_UnityServicesApiUrl = environment switch
                {
                    ServiceEnvironment.Staging => string.Concat("staging.", s_BaseUnityServicesApiUrl),
                    ServiceEnvironment.Test => string.Concat("staging.", s_BaseUnityServicesApiUrl),
                    _ => s_BaseUnityServicesApiUrl
                };
            }
        }

        /// <inheritdoc/>
        public async Task<UnityServicesToken> ExchangeAsync(TargetClientIdToken exchangeToken)
        {
            var response = await m_HttpClient.PostAsync($"https://{m_UnityServicesApiUrl}/api/auth/v1/genesis-token-exchange/unity", new StringContent(JsonSerialization.Serialize(exchangeToken), Encoding.UTF8, "application/json"));
            var unityServicesToken = await response.JsonDeserializeAsync<ExchangeTargetClientIdTokenResponse>();
            return new UnityServicesToken{ AccessToken = unityServicesToken.token};
        }
    }
}
