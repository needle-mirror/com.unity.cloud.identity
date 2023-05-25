using System;
using System.Collections.Generic;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// Holds the authenticated user information, like organizations the user belongs to (<see cref="OrganizationInfo"/>), and granted licenses (<see cref="LicenseInfo"/>).
    /// </summary>
    [Serializable]
    internal class UserInfoJson
    {
#pragma warning disable S1104 // Fields should not have public accessibility
        /// <summary>
        /// The id of the user.
        /// </summary>
        public string Id = "";
        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Name = "";
        /// <summary>
        /// The email of the user.
        /// </summary>
        public string Email = "";
        /// <summary>
        /// The list of <see cref="OrganizationInfo"/> the user belongs to.
        /// </summary>
        public List<OrganizationInfoJson> Organizations;
        /// <summary>
        /// The logout url of the user.
        /// </summary>
        public string LogoutUrl = "";
        /// <summary>
        /// The expiry date of current user session.
        /// </summary>
        public long SessionExpiryTicks = 0;
        /// <summary>
        /// The <see cref="LicenseInfo"/> attributed to the user.
        /// </summary>
        public LicenseInfoJson License;
        /// <summary>
        /// The list of <see cref="LicenseInfo"/> attributed to the user.
        /// </summary>
        public List<LicenseInfoJson> Entitlements;
#pragma warning restore S1104
    }
}
