using System;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable S1144 // Remove the unused private method

namespace Unity.Cloud.Identity.Documentation
{
    // Referenced:
    // - /Documentation~/use-case-organization-entitlements.md
    namespace EntitlementsExample
    {
        public class GetOrganizationEntitlements : MonoBehaviour
        {
            #region OrganizationEntitlements
            async Task FetchOrganizationEntitlements(IOrganization organization)
            {
                var entitlements = await organization.GetEntitlementsAsync();
                Debug.Log($"Entitlements for organization {organization.Name}: {string.Join(", ", entitlements.OrganizationEntitlements)}");
            }
            #endregion
        }

        public class GetOrganizationUserSeats : MonoBehaviour
        {
            #region UserSeats
            async Task FetchOrganizationUserSeats(IOrganization organization)
            {
                var entitlements = await organization.GetEntitlementsAsync();
                Debug.Log($"User seats for organization {organization.Name}: {string.Join(", ", entitlements.UserSeats)}");
            }
            #endregion
        }
    }
}

#pragma warning restore S1144

