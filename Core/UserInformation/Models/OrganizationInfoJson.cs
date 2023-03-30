using System;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// This class holds the organization information and its user related properties.
    /// </summary>
    [Serializable]
    internal class OrganizationInfoJson
    {
#pragma warning disable S1104 // Fields should not have public accessibility
        /// <summary>
        /// The id of the organization.
        /// </summary>
        public string Id = "";
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
}
