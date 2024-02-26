using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity.Editor;
using UnityEditor;
using UnityEngine;

#pragma warning disable S1144 // Remove the unused private method
#pragma warning disable S1481 // Remove the unused local variable

namespace Unity.Cloud.Identity.Documentation
{
    public class SecurityExample
    {
        readonly PkceConfiguration m_PkceConfiguration;
        SecurityExample()
        {
            m_PkceConfiguration = new PkceConfiguration
            {
                #region DefaultConfiguration
                CacheRefreshToken = true,
                ClientId = new ClientId("digital_twins"),
                ProxyLoginRedirectRoute = "https://services.api.unity.com/app-linking/v1/login/redirect/",
                ProxyLoginCompletedRoute = "https://services.api.unity.com/app-linking/v1/login/completed/",
                ProxySignOutCompletedRoute = "https://services.api.unity.com/app-linking/v1/signout/completed/",
                LoginUrl = "https://api.unity.com/v1/oauth2/authorize",
                TokenUrl = "https://api.unity.com/v1/oauth2/token",
                RefreshTokenUrl = "https://api.unity.com/v1/oauth2/token",
                LogoutUrl = "https://api.unity.com/v1/oauth2/revoke",
                SignOutUrl = "https://api.unity.com/v1/oauth2/end-session?post_logout_redirect_uri=",
                UserInfoUrl = "https://api.unity.com/v1/users/current/openid",
                CustomLoginParams = ""
                #endregion
            };
        }

        void UsePkceConfiguration()
        {
            var clientId = m_PkceConfiguration.ClientId;
            var securityExample = new SecurityExample();
        }

    }
}

#pragma warning restore S1481
#pragma warning restore S1144
