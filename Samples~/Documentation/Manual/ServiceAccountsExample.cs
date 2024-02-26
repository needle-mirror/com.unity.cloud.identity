using Unity.Cloud.AppLinking.Runtime;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity.Runtime;

#pragma warning disable S1144 // Remove the unused private method
#pragma warning disable S1481 // Remove the unused local variable

namespace Unity.Cloud.Identity.Documentation
{
    public class ServiceAccountExample
    {
        readonly IServiceAuthorizer m_ServiceAccountAuthorizer;
        readonly IServiceHttpClient m_ServiceHttpClient;

        ServiceAccountExample()
        {
            #region ServiceAuthorizer
            m_ServiceAccountAuthorizer = new ServiceAccountAuthorizer(PlatformSupportFactory.GetAuthenticationPlatformSupport());

            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;

            m_ServiceHttpClient = new ServiceHttpClient(httpClient, m_ServiceAccountAuthorizer, playerSettings);
            #endregion
        }

        void UseServiceHttpClient()
        {
            var isServiceHttpClientNull = m_ServiceHttpClient == null;
            var isServiceAccountAuthorizerNull = m_ServiceAccountAuthorizer == null;
            var serviceAccountExample = new ServiceAccountExample();
        }
    }
}

#pragma warning restore S1481
#pragma warning restore S1144
