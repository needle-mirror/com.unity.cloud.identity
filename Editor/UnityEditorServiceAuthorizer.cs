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
    public class UnityEditorServiceAuthorizer : ScriptableSingleton<UnityEditorServiceAuthorizer>, IServiceAuthorizer,
        IAuthenticationStateProvider, IUserInfoProvider, IOrganizationRepository, ISerializationCallbackReceiver
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

        [SerializeField] AuthenticationState m_AuthenticationState;

        /// <inheritdoc/>
        public event Action<AuthenticationState> AuthenticationStateChanged;

        string AccessToken
        {
            get => m_AccessToken;
            set => m_AccessToken = value;
        }

        [SerializeField] string m_AccessToken;

        string UnityServicesToken
        {
            get => m_UnityServicesToken;
            set => m_UnityServicesToken = value;
        }

        [SerializeField] string m_UnityServicesToken;

        double m_LastExchangeRequestCheck;

        const double k_ExchangeRequestRetryDelayInSeconds = 0.5;

        IAccessTokenExchanger<TargetClientIdToken, UnityServicesToken>
            m_TargetClientIdTokenToUnityServicesTokenExchanger;

        AuthenticatedUserSession m_AuthenticatedUserSession;

        IUnityEditorAccessTokenProvider m_UnityEditorAccessTokenProvider;

        IUnityUserInfoJsonProvider m_UnityUserInfoJsonProvider;
        IGuestProjectJsonProvider m_GuestProjectJsonProvider;
        IOrganizationJsonProvider m_OrganizationJsonProvider;

        bool m_UseOverride = false;
        Task<string> m_GetAccessTokenTask;

        internal void OverrideUnityEditorServiceAuthorizer(
            IAccessTokenExchanger<TargetClientIdToken, UnityServicesToken> accessTokenExchanger,
            IUnityEditorAccessTokenProvider unityEditorAccessTokenProvider,
            IUnityUserInfoJsonProvider unityUserInfoJsonProvider = null,
            IGuestProjectJsonProvider guestProjectJsonProvider = null,
            IOrganizationJsonProvider organizationJsonProvider = null)
        {
            m_TargetClientIdTokenToUnityServicesTokenExchanger = accessTokenExchanger;
            m_UnityEditorAccessTokenProvider = unityEditorAccessTokenProvider;
            m_UnityUserInfoJsonProvider = unityUserInfoJsonProvider;
            m_GuestProjectJsonProvider = guestProjectJsonProvider;
            m_OrganizationJsonProvider = organizationJsonProvider;

            m_UseOverride = true;
            InitAuthenticatedUserSession();
        }

        void OnEnable()
        {
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

        void InitAuthenticatedUserSession()
        {
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var httpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

            if (m_UseOverride)
            {
                m_AuthenticatedUserSession = new AuthenticatedUserSession(
                    new ServiceHttpClient(httpClient, this, playerSettings),
                    serviceHostResolver,
                    m_UnityUserInfoJsonProvider,
                    m_GuestProjectJsonProvider,
                    m_OrganizationJsonProvider
                );
            }
            else
            {
                m_TargetClientIdTokenToUnityServicesTokenExchanger = new TargetClientIdTokenToUnityServicesTokenExchanger(httpClient, serviceHostResolver);
                m_AuthenticatedUserSession = new AuthenticatedUserSession(new ServiceHttpClient(httpClient, this, playerSettings), serviceHostResolver);
            }
        }

        async void Update()
        {
            if (!AccessTokenAvailable(out var accessToken))
                return;

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

        bool AccessTokenAvailable(out string accessToken)
        {
            if (m_UseOverride)
            {
                accessToken = string.Empty;
                if (m_UnityEditorAccessTokenProvider == null)
                    return false;

                if (m_GetAccessTokenTask == null)
                {
                    m_GetAccessTokenTask = m_UnityEditorAccessTokenProvider.GetAccessTokenAsync();
                    return false;
                }

                if (!m_GetAccessTokenTask.IsCompleted)
                    return false;

                accessToken = m_GetAccessTokenTask.Result;
                m_GetAccessTokenTask = null;
                return true;
            }
            accessToken = CloudProjectSettings.accessToken;
            return true;
        }

        async Task RefreshUnityTokenAsync(string accessToken)
        {
            if (m_TargetClientIdTokenToUnityServicesTokenExchanger == null)
            {
                InitAuthenticatedUserSession();
            }

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
            if (m_AuthenticatedUserSession == null)
            {
                InitAuthenticatedUserSession();
            }
            return await m_AuthenticatedUserSession.GetUserInfoAsync();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IOrganization> ListOrganizationsAsync(Range range, CancellationToken cancellationToken = default)
        {
            if (m_AuthenticatedUserSession == null)
            {
                InitAuthenticatedUserSession();
            }
            return m_AuthenticatedUserSession.ListOrganizationsAsync(range, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IOrganization> GetOrganizationAsync(OrganizationId organizationId)
        {
            if (m_AuthenticatedUserSession == null)
            {
                InitAuthenticatedUserSession();
            }
            return await m_AuthenticatedUserSession.GetOrganizationAsync(organizationId);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            // Nothing to do before serialization occurs
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
            if (AuthenticationState.Equals(AuthenticationState.AwaitingInitialization))
            {
                AuthenticationState = AuthenticationState.LoggedOut;
            }
        }
    }
}
