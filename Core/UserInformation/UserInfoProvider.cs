using System;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// Provides access to a <see cref="UserInfo"/> instance returned from a cloud endpoint.
    /// </summary>
    public class UserInfoProvider : IUserInfoProvider
    {
        readonly IServiceHttpClient m_ServiceHttpClient;
        readonly IServiceHostResolver m_ServiceHostResolver;

        /// <summary>
        /// Provides access to a <see cref="UserInfo"/> instance returned from a cloud endpoint.
        /// </summary>
        /// <param name="serviceHttpClient">An IServiceHttpClient instance.</param>
        /// <param name="serviceHostResolver">The service host resolver for the service Url.</param>
        /// <example>
        /// <code source="../../Samples/Documentation/Scripting/UserInfoProviderExample.cs" region="UserInfoProvider"/>
        /// </example>
        public UserInfoProvider(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            m_ServiceHttpClient = serviceHttpClient.WithApiSourceHeadersFromAssembly(Assembly.GetExecutingAssembly());
            m_ServiceHostResolver = serviceHostResolver;
        }


        /// <summary>
        /// Retrieves a <see cref="UserInfo"/> instance from a cloud endpoint.
        /// </summary>
        /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the request fails to complete. See the returned StatusCode for more details.</exception>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ConnectionException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <returns>
        /// A task that results in a <see cref="UserInfo"/> instance after completion.
        /// </returns>
        public async Task<UserInfo> GetUserInfoAsync()
        {
            var requestUri = m_ServiceHostResolver.GetResolvedRequestUri("/api/auth/userinfo");
            var response = await m_ServiceHttpClient.GetAsync(requestUri);
            var userInfoJson = await response.JsonDeserializeAsync<UserInfoJson>();
            return new UserInfo(userInfoJson);
        }
    }
}
