using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable S1144 // Remove the unused private method

namespace Unity.Cloud.Identity.Documentation
{
    // Referenced:
    // - /Documentation~/cloud-storage.md
    namespace CloudStorageExample
    {
        public class GetOrganizationStorageUsageBehaviour : MonoBehaviour
        {
            #region GetOrganizationStorageUsage
            async Task FetchOrganizationCloudStorage(IOrganization organization)
            {
                var cloudStorageUsage = await organization.GetCloudStorageUsageAsync();
                if (cloudStorageUsage.UsageBytes < cloudStorageUsage.TotalStorageQuotaBytes)
                {
                    // Organization with available storage usage bytes logic
                }
            }
            #endregion
        }

        public class GetOrganizationMeteredBillingBehaviour : MonoBehaviour
        {
            #region GetOrganizationMeteredBilling
            async Task FetchOrganizationMeteredBilling(IOrganization organization)
            {
                var hasMeteredBillingActivated = await organization.HasMeteredBillingActivatedAsync();
                if (hasMeteredBillingActivated)
                {
                    // Organization with metered billing activated logic
                }
            }
            #endregion
        }
    }
}

#pragma warning restore S1144

