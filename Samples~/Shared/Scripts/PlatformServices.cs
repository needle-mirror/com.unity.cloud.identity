using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity.Runtime;

namespace Unity.Cloud.Identity.Samples
{
    /// <summary>
    /// A class to initialize and provide services and dependencies for the Unity Cloud platform.
    /// </summary>
    public static class PlatformServices
    {
        static CompositeAuthenticator s_CompositeAuthenticator;

        /// <summary>
        /// Returns a <see cref="ICompositeAuthenticator"/>.
        /// </summary>
        public static ICompositeAuthenticator CompositeAuthenticator => s_CompositeAuthenticator;

        /// <summary>
        /// Returns a <see cref="IAuthenticationStateProvider"/>.
        /// </summary>
        public static IAuthenticationStateProvider AuthenticationStateProvider => s_CompositeAuthenticator;

        /// <summary>
        /// Returns a <see cref="IAuthenticatedUserInfoProvider"/>.
        /// </summary>
        public static IAuthenticatedUserInfoProvider AuthenticatedUserInfoProvider => s_CompositeAuthenticator;

        /// <summary>
        /// Creates all platform services.
        /// </summary>
        public static void Create()
        {
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, serviceHostResolver, playerSettings)
                .AddDefaultBrowserAuthenticatedAccessTokenProvider(playerSettings, playerSettings)
                .AddDefaultPkceAuthenticator(playerSettings, playerSettings)
                .Build();

            s_CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);
        }

        /// <summary>
        /// A Task that initializes all platform services.
        /// </summary>
        /// <returns>A Task.</returns>
        public static async Task InitializeAsync()
        {
            await s_CompositeAuthenticator.InitializeAsync();
        }

        /// <summary>
        /// Shuts down all platform services.
        /// </summary>
        public static void ShutDownServices()
        {
            s_CompositeAuthenticator.Dispose();
            s_CompositeAuthenticator = null;
        }
    }
}
