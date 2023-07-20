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
        /// Returns a <see cref="UserInfoProvider"/>.
        /// </summary>
        public static IUserInfoProvider UserInfoProvider { get; private set; }

        /// <summary>
        /// Creates all platform services.
        /// </summary>
        public static void Create()
        {
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, serviceHostResolver)
                .AddDefaultBrowserAuthenticatedAccessTokenProvider()
                .AddDefaultPersonalAccessTokenProvider()
                .AddDefaultPkceAuthenticator(playerSettings)
                .Build();

            s_CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

            var serviceHttpClient = new ServiceHttpClient(httpClient, s_CompositeAuthenticator, playerSettings);

            UserInfoProvider = new UserInfoProvider(serviceHttpClient, serviceHostResolver);
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
            UserInfoProvider = null;
            s_CompositeAuthenticator.Dispose();
            s_CompositeAuthenticator = null;
        }
    }
}
