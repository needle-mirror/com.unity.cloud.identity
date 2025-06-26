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
        readonly ServiceAccountAuthenticator m_ServiceAccountAuthenticator;
        readonly IServiceHttpClient m_ServiceHttpClient;

        ServiceAccountExample()
        {
            #region ServiceAccountAuthenticator

            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var serviceHostResolver = ServiceHostResolverFactory.Create();

            var serviceAccountAuthenticatorSettingsBuilder =
                new ServiceAccountAuthenticatorSettingsBuilder(httpClient, serviceHostResolver, platformSupport)
                    .SetAppIdProvider(playerSettings);

            m_ServiceAccountAuthenticator = new ServiceAccountAuthenticator(serviceAccountAuthenticatorSettingsBuilder.Build());

            m_ServiceHttpClient = new ServiceHttpClient(httpClient, m_ServiceAccountAuthenticator, playerSettings);
            #endregion
        }

        void UseServiceHttpClient()
        {
            var isServiceHttpClientNull = m_ServiceHttpClient == null;
            var isServiceAccountAuthenticatorNull = m_ServiceAccountAuthenticator == null;
            var serviceAccountExample = new ServiceAccountExample();
        }
    }
}

#pragma warning restore S1481
#pragma warning restore S1144
