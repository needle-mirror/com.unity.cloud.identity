using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable CS1998 // Async method lacks await operators

namespace Unity.Cloud.Identity.Documentation
{
    // Referenced:
    // - /Documentation~/best-practices-dependency-injection.md
    namespace BestPracticesExample
    {
        #region PlatformServices
        public static class PlatformServices
        {
            public static void Create()
            {
                // Create all service instances
            }

            public static async Task InitializeAsync()
            {
                // Initialize IAuthenticator
            }

            public static void Shutdown()
            {
                // Clear all service instances
            }
        }
        #endregion

        #region PlatformServicesInitialization
        [DefaultExecutionOrder(int.MinValue)]
        public class PlatformServicesInitialization : MonoBehaviour
        {
            void Awake()
            {
                DontDestroyOnLoad(gameObject);
                PlatformServices.Create();
            }

            async Task Start()
            {
                await PlatformServices.InitializeAsync();
            }
        }
        #endregion

        #region PlatformServicesShutdown
        [DefaultExecutionOrder(int.MaxValue)]
        public class PlatformServicesShutdown : MonoBehaviour
        {
            void OnDestroy()
            {
                PlatformServices.Shutdown();
            }
        }
        #endregion
    }
}

#pragma warning restore CS1998
