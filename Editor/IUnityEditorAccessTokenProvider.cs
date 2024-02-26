using System.Threading.Tasks;

namespace Unity.Cloud.Identity.Editor
{
    /// <summary>
    /// An interface to provide a Unity Editor access token.
    /// </summary>
    public interface IUnityEditorAccessTokenProvider
    {
        /// <summary>
        /// Returns an access token.
        /// </summary>
        /// <returns>
        /// A task that once completed returns an access token.
        /// </returns>
        Task<string> GetAccessTokenAsync();
    }
}
