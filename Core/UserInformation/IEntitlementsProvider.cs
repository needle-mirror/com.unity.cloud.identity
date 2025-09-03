using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// An interface that exposes organization entitlements information.
    /// </summary>
    public interface IEntitlementsProvider
    {
        /// <summary>
        /// A Task that returns an <see cref="IEntitlements"/> once completed.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="IEntitlements"/>.</returns>
        public Task<IEntitlements> GetEntitlementsAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }
}
