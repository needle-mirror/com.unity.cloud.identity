using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{

    /// <summary>
    /// This class holds the authenticated user information, like organizations the user belongs to (<see cref="OrganizationInfo"/>) and granted licenses (<see cref="LicenseInfo"/>).
    /// </summary>
    public class UserInfo
    {
        public UserInfo() { }

        internal UserInfo(UserInfoJson json)
        {
            Id = json.Id;
            Name = json.Name;
            Email = json.Email;
            Organizations = json.Organizations.Select(jsonOrg => new OrganizationInfo(jsonOrg)).ToList();
            LogoutUrl = json.LogoutUrl;
            SessionExpiryTicks = json.SessionExpiryTicks;
            License = new LicenseInfo(json.License);
            Entitlements = json.Entitlements.Select(jsonEntitlement => new LicenseInfo(jsonEntitlement)).ToList();
        }

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
        public List<OrganizationInfo> Organizations;
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
        public LicenseInfo License;
        /// <summary>
        /// The list of <see cref="LicenseInfo"/> attributed to the user.
        /// </summary>
        public List<LicenseInfo> Entitlements;
#pragma warning restore S1104
    }

    /// <summary>
    /// This class holds the organization information and its user related properties.
    /// </summary>
    public class OrganizationInfo
    {
        public OrganizationInfo() { }

        internal OrganizationInfo(OrganizationInfoJson json)
        {
            Id = new OrganizationId(json.Id);
            Name = json.Name;
            AllowCreateNewProject = json.AllowCreateNewProject;
            IsPrimaryOrg = json.IsPrimaryOrg;
            AllowRequestLicense = json.AllowRequestLicense;
        }

#pragma warning disable S1104 // Fields should not have public accessibility
        /// <summary>
        /// The id of the organization.
        /// </summary>
        public OrganizationId Id = new("");
        /// <summary>
        /// The name of the organization.
        /// </summary>
        public string Name = "";
        /// <summary>
        /// Boolean about user capabilities of creating new projects in the organization.
        /// </summary>
        public bool AllowCreateNewProject = false;
        /// <summary>
        /// Boolean exposing if current organization is the primary one for the user.
        /// </summary>
        public bool IsPrimaryOrg = false;
        /// <summary>
        /// Boolean about user capabilities to request licenses from the organization.
        /// </summary>
        public bool AllowRequestLicense = false;
#pragma warning restore S1104
    }

    /// <summary>
    /// This class holds the licence information, such as expiry date, <see cref="LicenseType"/> and entitlement string identifier.
    /// </summary>
    public class LicenseInfo
    {
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
