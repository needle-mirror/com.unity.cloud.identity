using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using UnityEngine;

namespace Unity.Cloud.Identity.Editor
{
    /// <summary>
    /// An <see cref="IUnityEditorAccessTokenProvider"/> implementation to provide a Unity Editor access token from launch arguments.
    /// </summary>
    internal class LaunchArgumentsUnityEditorAccessTokenProvider : IUnityEditorAccessTokenProvider
    {
        readonly string m_Username;
        readonly string m_Password;
        readonly string m_UnitySubdomain;
        LoginToken m_LoginToken;

        /// <summary>
        /// Returns an <see cref="IUnityEditorAccessTokenProvider"/> implementation that provides a Unity Editor access token from launch arguments.
        /// </summary>
        /// <param name="serviceHostResolver">A <see cref="IServiceHostResolver"/> instance.</param>
        public LaunchArgumentsUnityEditorAccessTokenProvider(IServiceHostResolver serviceHostResolver)
        {
            var allArgs  = Environment.GetCommandLineArgs();
            for(var i=0;i<allArgs.Length;i++)
            {
                var arg = allArgs[i];
                if (arg.Equals("-username") && i < allArgs.Length -1)
                {
                    m_Username = allArgs[i + 1];
                }
                if (arg.Equals("-password") && i < allArgs.Length -1)
                {
                    m_Password = allArgs[i + 1];
                }
            }

            var serviceEnvironment = serviceHostResolver?.GetResolvedEnvironment();
            m_UnitySubdomain = serviceEnvironment switch
            {
                ServiceEnvironment.Staging => "api-staging",
                ServiceEnvironment.Test => "api-staging",
                _ => "api",
            };
        }

        async Task<LoginToken> GetDeviceTokenFromCredential() {
            if (m_LoginToken == null)
            {
                var loginCredentials = new LoginCredentials() { username = m_Username, password = m_Password, grant_type = "password" };
                var httpClient = new UnityHttpClient();
                var response = await httpClient.PostAsync($"https://{m_UnitySubdomain}.unity.com/v1/core/api/login", new StringContent(JsonSerialization.Serialize(loginCredentials), Encoding.UTF8, "application/json"));

                m_LoginToken = await response.JsonDeserializeAsync<LoginToken>();
            }
            return m_LoginToken;
        }

        /// <inheritdoc/>
        public async Task<string> GetAccessTokenAsync()
        {
            var loginToken = await GetDeviceTokenFromCredential();
            return loginToken.access_token;
        }
    }


    class LoginCredentials
    {
        public string username { get; set; }
        public string password { get; set; }
        public string grant_type { get; set; }
    }

    class LoginToken
    {
        public string access_token;
        public string refresh_token;
        public int expires_in;
    }

}
