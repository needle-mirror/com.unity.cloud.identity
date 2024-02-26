using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    internal interface IUnityUserInfoJsonProvider
    {
        public Task<UnityUserInfoJson> GetUnityUserInfoJsonAsync();
    }

    internal class UnityUserInfoJsonProvider : IUnityUserInfoJsonProvider
    {
        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IServiceHttpClient m_ServiceHttpClient;

        readonly GetRequestResponseCache<UnityUserInfoJson> m_GetUnityUserOrganizationRequestResponseCache;

        public UnityUserInfoJsonProvider(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            m_ServiceHostResolver = serviceHostResolver;
            m_ServiceHttpClient = serviceHttpClient;

            m_GetUnityUserOrganizationRequestResponseCache = new GetRequestResponseCache<UnityUserInfoJson>(60);
        }

        public async Task<UnityUserInfoJson> GetUnityUserInfoJsonAsync()
        {
            var url = m_ServiceHostResolver.GetResolvedRequestUri("/api/unity/v1/users/me/organizations");
            UnityUserInfoJson userInfoJson;
            if (m_GetUnityUserOrganizationRequestResponseCache.TryGetRequestResponseFromCache(url, out UnityUserInfoJson value))
            {
                userInfoJson = value;
            }
            else
            {
                var response = await m_ServiceHttpClient.GetAsync(url);
                var deserializedResponse = await response.JsonDeserializeAsync<UnityUserInfoJson>();
                userInfoJson = m_GetUnityUserOrganizationRequestResponseCache.AddGetRequestResponseToCache(url, deserializedResponse);
            }
            return userInfoJson;
        }
    }
}
