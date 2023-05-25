using System;
using System.Threading.Tasks;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// An interface that allows access to a <see cref="UserInfo"/> instance.
    /// </summary>
    public interface IUserInfoProvider
    {
        /// <summary>
        /// Abstracts a Task that results in a <see cref="UserInfo"/> instance once completed.
        /// </summary>
        /// <returns>
        /// A task that results in a <see cref="UserInfo"/> instance once completed.
        /// </returns>
        Task<UserInfo> GetUserInfoAsync();
    }
}
