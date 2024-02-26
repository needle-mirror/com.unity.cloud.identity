using System;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    internal interface IOrganizationJsonProvider
    {
        public Task<OrganizationJson> GetOrganizationJsonAsync(OrganizationId organizationId);
    }
}
