using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Identity.Samples
{
    /// <summary>
    /// A Monobehaviour class to initialize services and dependencies for the Unity Cloud platform.
    /// </summary>
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
}
