using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Identity
{
    internal interface IEntitlementsJsonProvider
    {
        public Task<EntitlementsJson> GetEntitlementsJsonAsync(CancellationToken cancellationToken);
    }
}

