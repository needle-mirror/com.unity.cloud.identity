using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Unity.Cloud.Identity.Editor
{
    internal class CloudProjectSettingsUnityEditorAccessTokenProvider : IUnityEditorAccessTokenProvider
    {
        public Task<string> GetAccessTokenAsync()
        {
            return Task.FromResult(CloudProjectSettings.accessToken);
        }
    }
}
