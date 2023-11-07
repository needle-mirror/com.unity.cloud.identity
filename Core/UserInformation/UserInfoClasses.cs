using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// This class holds the licence information, such as expiry date, <see cref="LicenseType"/> and entitlement string identifier.
    /// </summary>
    public class LicenseInfo
    {
        /// <summary>
        /// LicenseInfo constructor.
        /// </summary>
        public LicenseInfo() {}

        internal LicenseInfo(LicenseInfoJson json)
        {
            ExpiryTicks = json.ExpiryTicks;
            Type = json.Type;
            EntitlementId = json.EntitlementId;
        }

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

    /// <summary>
    /// This enum exposes possible licence type values.
    /// </summary>
    public enum LicenseType
    {
#pragma warning disable S1104 // Fields should not have public accessibility
        /// <summary>
        /// No License.
        /// </summary>
        None = 0,
        /// <summary>
        /// Trial License.
        /// </summary>
        Trial = 1,
        /// <summary>
        /// Paid License.
        /// </summary>
        Paid = 2,
        /// <summary>
        /// Floating License.
        /// </summary>
        Floating = 3,
#pragma warning restore S1104
    }
}
