using System.Collections.Generic;

namespace Unity.Cloud.Identity
{
    public interface ICloudStorageEntitlements
    {
        public ulong TotalStorageQuotaBytes { get; set; }

        public IEnumerable<ICloudStorageEntitlement> Entitlements { get; set; }

        public bool MeteredOptInEnabled { get; set; }

        public bool CanOptIn { get; set; }
    }

    public interface ICloudStorageEntitlement
    {
        public string DisplayName { get; set; }

        public string Type { get; set; }

        public int Count { get; set; }

        public ulong StorageQuotaBytes { get; set; }
    }
}
