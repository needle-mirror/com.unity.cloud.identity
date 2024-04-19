using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    public interface ICloudStorageInfoProvider
    {
        public Task<ICloudStorageUsage> GetCloudStorageUsageAsync(CancellationToken cancellationToken);

        public Task<ICloudStorageEntitlements> GetCloudStorageEntitlementsAsync(CancellationToken cancellationToken);
    }
}
