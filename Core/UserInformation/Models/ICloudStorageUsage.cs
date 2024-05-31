namespace Unity.Cloud.Identity
{
    /// <summary>
    /// An interface that provides Cloud Storage information for an <see cref="Organization"/>.
    /// </summary>
    public interface ICloudStorageUsage
    {
        /// <summary>
        /// The total usage bytes count.
        /// </summary>
        public ulong UsageBytes { get; }

        /// <summary>
        /// The total storage quota bytes available.
        /// </summary>
        public ulong TotalStorageQuotaBytes { get; }
    }
}
