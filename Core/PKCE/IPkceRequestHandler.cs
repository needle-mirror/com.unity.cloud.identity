using System.Threading.Tasks;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// This interface defines methods related to retrieving pkce-related tokens
    /// </summary>
    public interface IPkceRequestHandler
    {
        /// <summary>
        /// Retrieves the device token from specified end-point
        /// </summary>
        /// <param name="tokenEndPointParams">The content of the request</param>
        /// <returns>
        /// A task that results in a <see cref="DeviceToken"/> when completed.
        /// </returns>
        Task<DeviceToken> ExchangeCodeForDeviceTokenAsync(string tokenEndPointParams);

        /// <summary>
        /// Updates the device token from specified end-point
        /// </summary>
        /// <param name="tokenEndPointParams">The content of the request</param>
        /// <param name="refreshToken">The refresh token needed for the refresh request</param>
        /// <returns>
        /// A task that results in a <see cref="DeviceToken"/> when completed.
        /// </returns>
        Task<DeviceToken> RefreshTokenAsync(string tokenEndPointParams, string refreshToken);

        /// <summary>
        /// Revokes the current refresh token
        /// </summary>
        /// <param name="revokeEndPointParams">The content of the request</param>
        /// <returns>
        /// A task.
        /// </returns>
        Task RevokeRefreshTokenAsync(string revokeEndPointParams);
    }
}
