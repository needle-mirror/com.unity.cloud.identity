using System;
using System.Threading.Tasks;

namespace Unity.Cloud.Identity
{
    public class UnityServicesToken
    {
        public string AccessToken { get; internal set; }
    }

    public interface IAccessTokenExchanger<T1, T2>
    {
        /// <summary>
        /// Returns an exchanged token
        /// </summary>
        /// <returns>
        /// A task that once completed returns an exchanged token.
        /// </returns>
        Task<T2> ExchangeAsync(T1 exchangeToken);
    }
}
