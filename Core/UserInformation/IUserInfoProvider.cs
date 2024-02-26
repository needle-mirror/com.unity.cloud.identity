using System.Threading.Tasks;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// An interface that exposes methods to fetch user information.
    /// </summary>
    public interface IUserInfoProvider
    {
        /// <summary>
        /// A task to fetch asynchronously user information.
        /// </summary>
        /// <returns>An <see cref="IUserInfo"/> instance.</returns>
        Task<IUserInfo> GetUserInfoAsync();
    }
}
