using System;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// This class holds the licence information, such as expiry date, <see cref="LicenseType"/> and entitlement string identifier.
    /// </summary>
    [Serializable]
    internal class LicenseInfoJson
    {
#pragma warning disable S1104 // Fields should not have public accessibility
        /// <summary>
        /// The expiry date of the license.
        /// </summary>
        public long ExpiryTicks = 0;
        /// <summary>
        /// The LicenseType of the license.
        /// </summary>
        public LicenseType Type;
        /// <summary>
        /// The entitlement identifier of the license.
        /// </summary>
        public string EntitlementId = "";
#pragma warning restore S1104
    }
}
