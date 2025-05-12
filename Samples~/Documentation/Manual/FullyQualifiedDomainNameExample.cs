using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.AppLinking.Runtime;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Cloud.Identity.Documentation
{
    // Referenced:
    // - /Documentation~/use-case-fqdn.md
    namespace FullyQualifiedDomainNameExample
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        #region PlatformServices
    public static class PlatformServices
    {
        static ICompositeAuthenticator s_CompositeAuthenticator;

        public static ICompositeAuthenticator CompositeAuthenticator => s_CompositeAuthenticator;

        public static async Task InitializeAsync()
        {
            var fullyQualifiedDomainName = "my.fully.qualified.domain.name.com";
            var openIdConfigurationUrl = "https://my.fully.qualified.domain.name.com/.well-known/openid-configuration";

            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;

            var serviceConnector = ServiceConnectorFactory.CreateForFullyQualifiedDomainName(platformSupport, httpClient, playerSettings, playerSettings, fullyQualifiedDomainName, openIdConfigurationUrl);

            s_CompositeAuthenticator = serviceConnector.CompositeAuthenticator;

            await s_CompositeAuthenticator.InitializeAsync();
        }

        public static void Shutdown()
        {
            (s_CompositeAuthenticator as IDisposable)?.Dispose();
            s_CompositeAuthenticator = null;
        }

     }
        #endregion
#pragma warning restore S1075
    }
}
