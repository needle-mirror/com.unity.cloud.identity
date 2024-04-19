using System.Collections.Generic;

namespace Unity.Cloud.Identity
{
    internal class CloudStorageEntitlementsJson
    {
        public ulong TotalStorageQuotaBytes { get; set; }

        public IEnumerable<CloudStorageEntitlementJson> Entitlements { get; set; }

        public bool MeteredOptInEnabled { get; set; }

        public bool CanOptIn { get; set; }
    }

    internal class CloudStorageEntitlementJson
    {
        public string DisplayName { get; set; }

        public string Type { get; set; }

        public int Count { get; set; }

        public ulong StorageQuotaBytes { get; set; }
    }
}
