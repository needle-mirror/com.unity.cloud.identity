using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.Identity
{
    public class CloudStorageEntitlements : ICloudStorageEntitlements
    {
        public ulong TotalStorageQuotaBytes { get; set; }
        public IEnumerable<ICloudStorageEntitlement> Entitlements { get; set; }
        public bool MeteredOptInEnabled { get; set; }
        public bool CanOptIn { get; set; }

        internal CloudStorageEntitlements(CloudStorageEntitlementsJson cloudStorageEntitlementsJson)
        {
            TotalStorageQuotaBytes = cloudStorageEntitlementsJson.TotalStorageQuotaBytes;
            Entitlements = cloudStorageEntitlementsJson.Entitlements.Select(cloudStorageEntitlementJson => new CloudStorageEntitlement(cloudStorageEntitlementJson));
            MeteredOptInEnabled = cloudStorageEntitlementsJson.MeteredOptInEnabled;
            CanOptIn = cloudStorageEntitlementsJson.CanOptIn;
        }
    }

    public class CloudStorageEntitlement : ICloudStorageEntitlement
    {
        public string DisplayName { get; set; }

        public string Type { get; set; }

        public int Count { get; set; }

        public ulong StorageQuotaBytes { get; set; }

        internal CloudStorageEntitlement(CloudStorageEntitlementJson cloudStorageEntitlementJson)
        {
            DisplayName = cloudStorageEntitlementJson.DisplayName;
            Type = cloudStorageEntitlementJson.Type;
            Count = cloudStorageEntitlementJson.Count;
            StorageQuotaBytes = cloudStorageEntitlementJson.StorageQuotaBytes;
        }
    }
}
