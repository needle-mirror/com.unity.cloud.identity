namespace Unity.Cloud.Identity
{
    public interface ICloudStorageUsage
    {
        public ulong UsageBytes { get; set; }

        public ulong TotalStorageQuotaBytes { get; set; }
    }
}
