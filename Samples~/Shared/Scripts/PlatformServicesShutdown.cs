using UnityEngine;

namespace Unity.Cloud.Identity.Samples
{
    /// <summary>
    /// A Monobehaviour class to shut down services and dependencies from the Unity Cloud platform.
    /// </summary>
    [DefaultExecutionOrder(int.MaxValue)]
    public class PlatformServicesShutdown : MonoBehaviour
    {
        void OnDestroy()
        {
            PlatformServices.ShutDownServices();
        }
    }
}
