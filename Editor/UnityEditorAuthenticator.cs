using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;

namespace Unity.Cloud.Identity.Editor
{
    /// <summary>
    /// Provides an <see cref="IAuthenticator"/> to access the user authenticated session in the Unity Editor.
    /// </summary>
    public class UnityEditorAuthenticator : IAuthenticator, IDisposable
    {
#if !UNITY_EDITOR
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<UnityEditorAuthenticator>();
        const string k_InvalidOperationMessage = "This class can only be used in the Unity Editor execution context.";
#endif
        AuthenticationState m_AuthenticationState = AuthenticationState.AwaitingInitialization;

        /// <inheritdoc/>
        public event Action<AuthenticationState> AuthenticationStateChanged;

        /// <inheritdoc/>
        public AuthenticationState AuthenticationState
        {
            get => m_AuthenticationState;
            private set
            {
                m_AuthenticationState = value;
                AuthenticationStateChanged?.Invoke(m_AuthenticationState);
            }
        }

        readonly IAccessTokenExchanger<TargetClientIdToken, UnityServicesToken> m_TargetClientIdTokenToUnityServicesTokenExchanger;
        UnityServicesToken m_UnityServicesToken;

        /// <summary>
        /// Returns an `IAccessTokenProvider`implementation that expects an access token from a Unity Editor environment.
        /// </summary>
        public UnityEditorAuthenticator(IAccessTokenExchanger<TargetClientIdToken, UnityServicesToken> accessTokenExchanger)
        {
            m_TargetClientIdTokenToUnityServicesTokenExchanger = accessTokenExchanger;
#if !UNITY_EDITOR
            s_Logger.LogWarning(k_InvalidOperationMessage);
#endif
        }

        async void Update()
        {
            var isLoggedIn = !string.IsNullOrEmpty(CloudProjectSettings.accessToken);
            if (m_AuthenticationState.Equals(Identity.AuthenticationState.LoggedIn) && !isLoggedIn)
            {
                m_UnityServicesToken = null;
                AuthenticationState = AuthenticationState.LoggedOut;
            }
            else if (m_AuthenticationState.Equals(Identity.AuthenticationState.LoggedOut) && isLoggedIn)
            {
                var targetClientIdToken = new TargetClientIdToken() { token = CloudProjectSettings.accessToken};
                m_UnityServicesToken =
                    await m_TargetClientIdTokenToUnityServicesTokenExchanger.ExchangeAsync(targetClientIdToken);
                AuthenticationState = AuthenticationState.LoggedIn;
            }
        }

        /// <summary>
        /// Disposes of any `IDisposable` references.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of any `IDisposable` references.
        /// </summary>
        /// <param name="disposing">Dispose pattern boolean value received from public Dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
#if UNITY_EDITOR
                EditorApplication.update -= Update;
#endif
            }
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
#if UNITY_EDITOR

#pragma warning disable S2696 // Instance members should not write to static fields
            EditorApplication.update += Update;
#pragma warning restore S2696

            if (!string.IsNullOrEmpty(CloudProjectSettings.accessToken))
            {
                var targetClientIdToken = new TargetClientIdToken() { token = CloudProjectSettings.accessToken};
                m_UnityServicesToken =
                    await m_TargetClientIdTokenToUnityServicesTokenExchanger.ExchangeAsync(targetClientIdToken);
                AuthenticationState = AuthenticationState.LoggedIn;
            }
            else
            {
                AuthenticationState = AuthenticationState.LoggedOut;
            }
#else
            throw new InvalidOperationException(k_InvalidOperationMessage);
#endif
            await Task.CompletedTask;
        }

        /// <summary>
        /// Indicates if the <see cref="UnityEditorAuthenticator"/> running instance has access to an access token from the Unity Editor environment.
        /// </summary>
        /// <returns>If the <see cref="UnityEditorAuthenticator"/> running instance has access to an access token from the Unity Editor environment.</returns>
        public Task<bool> HasValidPreconditionsAsync()
        {
#if UNITY_EDITOR
            return Task.FromResult(true);
#else
            return Task.FromResult(false);
#endif
        }

        /// <inheritdoc/>
        public async Task<string> GetAccessTokenAsync()
        {
#if UNITY_EDITOR
            if (m_UnityServicesToken == null)
                return await Task.FromResult<string>(null);

            return await Task.FromResult(m_UnityServicesToken.AccessToken);
#else
            throw new InvalidOperationException(k_InvalidOperationMessage);
#endif
        }
    }
}

