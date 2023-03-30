using System;
using Unity.Cloud.Identity.Runtime;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Unity.Cloud.Identity.Documentation
{

    public class UserInfoProviderExample : MonoBehaviour
    {

    #region UserInfoProvider

    CompositeAuthenticator m_CompositeAuthenticator;
    IUserInfoProvider m_UserInfoProvider;

    void CreateUserInfoProvider()
    {
        var configuration = UnityRuntimeServiceHostConfigurationFactory.Create();
        var playerSettings = UnityCloudPlayerSettings.Instance;
        var httpClient = new UnityHttpClient();

        var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, PlatformSupportFactory.GetAuthenticationPlatformSupport())
            .AddDefaultBrowserAuthenticatedAccessTokenProvider()
            .AddDefaultPersonalAccessTokenProvider()
            .AddDefaultPkceAuthenticator(playerSettings, playerSettings)
            .Build();

        m_CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

        var serviceHttpClient = new ServiceHttpClient(httpClient, m_CompositeAuthenticator, playerSettings);

        m_UserInfoProvider = new UserInfoProvider(serviceHttpClient, configuration);
    }

    #endregion

    }

}
