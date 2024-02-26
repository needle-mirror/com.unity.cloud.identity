using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    internal interface IProjectsJsonProvider
    {
        IAsyncEnumerable<ProjectJson> GetOrganizationProjectsJson(OrganizationId organizationId, IEntityRoleProvider entityRoleProvider, Range range, CancellationToken cancellationToken);
    }
}
