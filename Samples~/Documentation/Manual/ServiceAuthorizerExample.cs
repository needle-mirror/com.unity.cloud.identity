using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.AppLinking.Runtime;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity.Runtime;

#pragma warning disable S1144 // Remove the unused private method
#pragma warning disable S1481 // Remove the unused local variable

namespace Unity.Cloud.Identity.Documentation
{

    internal interface IAssetRepositoryFactory
    {
        static IAssetRepositoryFactory Create(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            throw new NotImplementedException();
        }
    }

    internal class AssetRepositoryFactory : IAssetRepositoryFactory
    {
        public static IAssetRepositoryFactory Create(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            return new AssetRepositoryFactory();
        }
    }

    // Referenced:
    // - /Documentation~/use-case-getting-user-information.md
    public class ServiceAuthorizerExample
    {
        readonly IAssetRepositoryFactory m_AssetRepository;
        readonly ServiceAccountAuthenticator m_ServiceAccountAuthenticator;
        readonly IServiceHttpClient m_ServiceHttpClient;

        ServiceAuthorizerExample()
        {
            #region ServiceAuthorizer
            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var serviceHostResolver = ServiceHostResolverFactory.Create();

            var serviceAccountAuthenticatorSettingsBuilder = new ServiceAccountAuthenticatorSettingsBuilder();
            serviceAccountAuthenticatorSettingsBuilder.AddAuthenticationPlatformSupport(platformSupport)
                .AddServiceHostResolver(serviceHostResolver)
                .AddHttpClient(httpClient)
                .AddAppIdProvider(playerSettings);

            m_ServiceAccountAuthenticator = new ServiceAccountAuthenticator(serviceAccountAuthenticatorSettingsBuilder.Build());

            m_ServiceHttpClient = new ServiceHttpClient(httpClient, m_ServiceAccountAuthenticator, playerSettings);

            // Injecting the ServiceHttpClient to build an authorized IAssetRepository to retrieve IAsset, IDataset, ... from Unity Cloud.
            m_AssetRepository = AssetRepositoryFactory.Create(m_ServiceHttpClient, serviceHostResolver);
            #endregion
        }

        void UseServiceAuthorizerExample()
        {
            var serviceAuthorizerExample = new ServiceAuthorizerExample();
            var isAssetRepositoryNull = m_AssetRepository == null;
            var isCompositeAuthenticatorNull = m_ServiceAccountAuthenticator == null;
            var isServiceHttpClientNull = m_ServiceHttpClient == null;
        }

    }
}

#pragma warning restore S1481
#pragma warning restore S1144
