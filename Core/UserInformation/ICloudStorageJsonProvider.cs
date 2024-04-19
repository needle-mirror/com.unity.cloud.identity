using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    internal interface ICloudStorageJsonProvider
    {
        public Task<CloudStorageUsageJson> GetCloudStorageUsageAsync(CancellationToken cancellationToken);

        public Task<CloudStorageEntitlementsJson> GetCloudStorageEntitlementsAsync(CancellationToken cancellationToken);
    }
}
