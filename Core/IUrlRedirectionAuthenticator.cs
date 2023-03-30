using System;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// An interface for manual login and logout operations using redirection flows.
    /// </summary>
    public interface IUrlRedirectionAuthenticator : IAuthenticator
    {
        /// <summary>
        /// Performs a login operation.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="AuthenticationFailedException"></exception>
        /// <returns>
        /// A task.
        /// </returns>
        Task LoginAsync();


        /// <summary>
        /// Cancels the awaiting login operation.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        void CancelLogin();

        /// <summary>
        /// Performs a logout operation.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>
        /// A task.
        /// </returns>
        Task LogoutAsync();
    }
}
