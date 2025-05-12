using System;
using System.Threading.Tasks;
using Unity.Cloud.AppLinking.Runtime;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity.Runtime;

namespace Unity.Cloud.Identity.Samples
{
    /// <summary>
    /// A class to initialize and provide services and dependencies for the Unity Cloud platform.
    /// </summary>
    public static class PlatformServices
    {
        static ICompositeAuthenticator s_CompositeAuthenticator;

        /// <summary>
        /// Returns a <see cref="ICompositeAuthenticator"/>.
        /// </summary>
        public static ICompositeAuthenticator CompositeAuthenticator => s_CompositeAuthenticator;

        /// <summary>
        /// Creates all platform services.
        /// </summary>
        public static void Create()
        {
            // Create platform implementations to inject in factory
            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;

            // Create the required set of classes to handle access to the cloud API
            var serviceConnector = ServiceConnectorFactory.Create(platformSupport, httpClient, playerSettings, playerSettings);

            s_CompositeAuthenticator = serviceConnector.CompositeAuthenticator;
        }

        /// <summary>
        /// A Task that initializes all platform services.
        /// </summary>
        /// <returns>A Task.</returns>
        public static async Task InitializeAsync()
        {
            await CompositeAuthenticator.InitializeAsync();
        }

        /// <summary>
        /// Shuts down all platform services.
        /// </summary>
        public static void ShutDownServices()
        {
            (s_CompositeAuthenticator as IDisposable)?.Dispose();
            s_CompositeAuthenticator = null;
        }
    }
}
