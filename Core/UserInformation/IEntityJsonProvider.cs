using System.Collections.Generic;

namespace Unity.Cloud.Identity
{
    internal interface IEntityJsonProvider
    {
        IEnumerable<EntityJson> GetEntityJsonAsync(string entityId, string entityType);
    }
}
