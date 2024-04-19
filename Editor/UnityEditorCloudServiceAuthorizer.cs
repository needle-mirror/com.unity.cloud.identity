using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.AppLinking.Runtime;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using UnityEditor;
using UnityEngine;

namespace Unity.Cloud.Identity.Editor
{
    /// <summary>
    /// An <see cref="IServiceAuthorizer"/> implementation that supports domain reload in the Unity Editor.
    /// </summary>
    public class UnityEditorCloudServiceAuthorizer : ScriptableSingleton<UnityEditorCloudServiceAuthorizer>, IServiceAuthorizer, IAuthenticationStateProvider, IUserInfoProvider, IOrganizationRepository, ISerializationCallbackReceiver
    {
        /// <inheritdoc/>
        public AuthenticationState AuthenticationState
        {
            get => m_AuthenticationState;
            private set
            {
                if (m_AuthenticationState == value)
                    return;
                m_AuthenticationState = value;
                AuthenticationStateChanged?.Invoke(m_AuthenticationState);
            }
        }

        [SerializeField]
        AuthenticationState m_AuthenticationState;

        /// <inheritdoc/>
        public event Action<AuthenticationState> AuthenticationStateChanged;

        string AccessToken {
            get => m_AccessToken;
            set => m_AccessToken = value;
        }

        [SerializeField]
        string m_AccessToken;

        string UnityServicesToken {
            get => m_UnityServicesToken;
            set => m_UnityServicesToken = value;
        }

        [SerializeField]
        string m_UnityServicesToken;

        double m_LastExchangeRequestCheck;

        const double k_ExchangeRequestRetryDelayInSeconds = 0.5;

        IAccessTokenExchanger<TargetClientIdToken, UnityServicesToken> m_TargetClientIdTokenToUnityServicesTokenExchanger;

        AuthenticatedUserSession m_AuthenticatedUserSession;

        void OnEnable()
        {
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

            m_TargetClientIdTokenToUnityServicesTokenExchanger = new TargetClientIdTokenToUnityServicesTokenExchanger(httpClient, serviceHostResolver);
            m_AuthenticatedUserSession = new AuthenticatedUserSession(new ServiceHttpClient(httpClient, this, playerSettings), serviceHostResolver);

            m_LastExchangeRequestCheck = EditorApplication.timeSinceStartup;

            if (AuthenticationState.Equals(AuthenticationState.AwaitingInitialization))
            {
                AuthenticationState = AuthenticationState.LoggedOut;
            }

            EditorApplication.update += Update;
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        async void Update()
        {
            var accessToken = CloudProjectSettings.accessToken;
            var editorHasToken = !string.IsNullOrEmpty(accessToken);

            // If editor token was refreshed
            if (editorHasToken && AccessToken != accessToken)
            {
                AccessToken = accessToken;
                UnityServicesToken = string.Empty;
                AuthenticationState = AuthenticationState.AwaitingLogin;
            }

            if (AuthenticationState.Equals(AuthenticationState.LoggedIn) && !editorHasToken)
            {
                UnityServicesToken = string.Empty;

                AuthenticationState = AuthenticationState.LoggedOut;
            }
            else if (AuthenticationState.Equals(AuthenticationState.LoggedOut) && editorHasToken)
            {
                AuthenticationState = AuthenticationState.AwaitingLogin;
            }
            else if (AuthenticationState.Equals(AuthenticationState.AwaitingLogin) && editorHasToken)
            {
                // Throttle request to retry token exchange until connectivity is restored
                if (EditorApplication.timeSinceStartup - m_LastExchangeRequestCheck < k_ExchangeRequestRetryDelayInSeconds)
                    return;

                m_LastExchangeRequestCheck = EditorApplication.timeSinceStartup;
                try
                {
                    await RefreshUnityTokenAsync(accessToken);
                    if (!string.IsNullOrEmpty(UnityServicesToken))
                        AuthenticationState = AuthenticationState.LoggedIn;
                }
                catch (HttpRequestException)
                {
                    /* silent fail */
                }
            }
        }

        async Task RefreshUnityTokenAsync(string accessToken)
        {
            var targetClientIdToken = new TargetClientIdToken { token = accessToken};
            var exchangedToken = await m_TargetClientIdTokenToUnityServicesTokenExchanger.ExchangeAsync(targetClientIdToken);
            UnityServicesToken = exchangedToken.AccessToken;
            if (!string.IsNullOrEmpty(UnityServicesToken))
                AccessToken = accessToken;
        }

        /// <inheritdoc cref="IServiceAuthorizer.AddAuthorization"/>
        public async Task AddAuthorization(HttpHeaders headers)
        {
#if UNITY_EDITOR
            headers.AddAuthorization(UnityServicesToken, ServiceHeaderUtils.k_BearerScheme);
            await Task.CompletedTask;
#else
            throw new InvalidOperationException(k_InvalidOperationMessage);
#endif
        }

        /// <inheritdoc/>
        public async Task<IUserInfo> GetUserInfoAsync()
        {
            return await m_AuthenticatedUserSession.GetUserInfoAsync();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IOrganization> ListOrganizationsAsync(Range range, CancellationToken cancellationToken = default)
        {
            return m_AuthenticatedUserSession.ListOrganizationsAsync(range, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IOrganization> GetOrganizationAsync(OrganizationId organizationId)
        {
            return await m_AuthenticatedUserSession.GetOrganizationAsync(organizationId);
        }

        public void OnBeforeSerialize()
        {
            // Nothing to do before serialization occurs
        }

        public void OnAfterDeserialize()
        {
            if (AuthenticationState.Equals(AuthenticationState.AwaitingInitialization))
            {
                AuthenticationState = AuthenticationState.LoggedOut;
            }
        }
    }
}
