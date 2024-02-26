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
        readonly ICompositeAuthenticator m_CompositeAuthenticator;
        readonly IAssetRepositoryFactory m_AssetRepository;

        ServiceAuthorizerExample()
        {
            #region ServiceAuthorizer
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, serviceHostResolver, playerSettings)
                .AddDefaultBrowserAuthenticatedAccessTokenProvider(playerSettings)
                .AddDefaultPkceAuthenticator(playerSettings)
                .Build();

            // Create m_CompositeAuthenticator from compositeAuthenticatorSettings
            m_CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

            // Injecting the CompositeAuthenticator as an IServiceAuthorizer to build the ServiceHttpClient
            var serviceHttpClient = new ServiceHttpClient(httpClient, m_CompositeAuthenticator, playerSettings);

            // Injecting the serviceHttpClient to build an authorized IAssetRepository to retrieve IAsset, IDataset, ... from Unity Cloud.
            m_AssetRepository = AssetRepositoryFactory.Create(serviceHttpClient, serviceHostResolver);
            #endregion
        }

        void UseServiceAuthorizerExample()
        {
            var serviceAuthorizerExample = new ServiceAuthorizerExample();
            var isAssetRepositoryNull = m_AssetRepository == null;
            var isCompositeAuthenticatorNull = m_CompositeAuthenticator == null;
        }

    }
}

#pragma warning restore S1481
#pragma warning restore S1144
