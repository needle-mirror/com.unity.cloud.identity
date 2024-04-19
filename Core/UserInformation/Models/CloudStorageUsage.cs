namespace Unity.Cloud.Identity
{
    public class CloudStorageUsage : ICloudStorageUsage
    {
        public ulong UsageBytes { get; set; }
        public ulong TotalStorageQuotaBytes { get; set; }

        internal CloudStorageUsage(CloudStorageUsageJson cloudStorageUsageJson)
        {
            UsageBytes = cloudStorageUsageJson.UsageBytes;
            TotalStorageQuotaBytes = cloudStorageUsageJson.TotalStorageQuotaBytes;
        }

    }
}
