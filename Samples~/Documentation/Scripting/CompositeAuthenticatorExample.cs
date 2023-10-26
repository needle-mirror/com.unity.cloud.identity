using System;
using Unity.Cloud.Common.Runtime;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Cloud.Identity.Runtime;
using System.Collections.Generic;

namespace Unity.Cloud.Identity.Documentation
{
    #region InitializeAndShutdown

    public class CompositeAuthenticatorExample : MonoBehaviour
    {
        CompositeAuthenticator m_CompositeAuthenticator;

        void Awake()
        {
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var httpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, PlatformSupportFactory.GetAuthenticationPlatformSupport(), serviceHostResolver, playerSettings)
                .AddDefaultBrowserAuthenticatedAccessTokenProvider(playerSettings, playerSettings)
                .AddDefaultPkceAuthenticator(playerSettings, playerSettings)
                .Build();

            m_CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);
        }

        async Task Start()
        {
            await m_CompositeAuthenticator.InitializeAsync();
        }
    }

    #endregion

}
