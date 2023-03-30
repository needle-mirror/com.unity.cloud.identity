using System;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{

    /// <summary>
    /// An interface for authentication flow that implements <see cref="IAccessTokenProvider"/> and <see cref="IAuthenticationStateProvider"/>.
    /// </summary>
    public interface IAuthenticator : IAccessTokenProvider, IAuthenticationStateProvider
    {
        /// <summary>
        /// Indicates if the `IAuthenticator` has valid preconditions to provide authentication in the current execution context.
        /// </summary>
        /// <returns>A task that when completed indicates if the `IAuthenticator` has valid preconditions to provide authentication in the current execution context.</returns>
        Task<bool> HasValidPreconditionsAsync();

        /// <summary>
        /// A task to initialize the <see cref="AuthenticationState"/> from either cache or direct injection value.
        /// </summary>
        Task InitializeAsync();
    }
}
