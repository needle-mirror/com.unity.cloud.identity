using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cloud.Identity.Samples
{
    public class UIController : MonoBehaviour
    {
        [SerializeField, Tooltip("UI Elements to activate at start up.")]
        List<GameObject> m_AwaitingAuthenticationInitializedUIElements = new();

        [SerializeField, Tooltip("UI Elements required for manual login.")]
        List<GameObject> m_ManualLoginUIElements = new();

        [SerializeField, Tooltip("UI Elements required to cancel manual login.")]
        List<GameObject> m_ManuaCancelLoginUIElements = new();

        [SerializeField, Tooltip("UI Elements required for manual logout.")]
        List<GameObject> m_ManualLogoutUIElements = new();

        ICompositeAuthenticator m_CompositeAuthenticator;
        IAuthenticationStateProvider m_AuthenticationStateProvider => m_CompositeAuthenticator;
        AuthenticationState m_AuthenticationState;

        void Awake()
        {
            m_CompositeAuthenticator = PlatformServices.CompositeAuthenticator;
            m_AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        void Start()
        {
            // Update UI with current state
            OnAuthenticationStateChanged(m_AuthenticationStateProvider.AuthenticationState);
        }

        void OnDestroy()
        {
            m_AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        void OnAuthenticationStateChanged(AuthenticationState state)
        {
            m_AuthenticationState = state;
            OnUIContextChanged();
        }

        void OnUIContextChanged()
        {
            var requiresGUI = m_CompositeAuthenticator.RequiresGUI;

            Debug.Log($"m_AuthenticationState: {m_AuthenticationState}, requiresGUI: {requiresGUI}");

            switch (m_AuthenticationState)
            {
                case AuthenticationState.AwaitingInitialization:
                    ShowUIElements(m_AwaitingAuthenticationInitializedUIElements, true);

                    ShowUIElements(m_ManualLoginUIElements, false);
                    ShowUIElements(m_ManuaCancelLoginUIElements, false);
                    ShowUIElements(m_ManualLogoutUIElements, false);
                    break;
                case AuthenticationState.AwaitingLogin:
                    ShowUIElements(m_AwaitingAuthenticationInitializedUIElements, false);

                    ShowUIElements(m_ManualLoginUIElements, false);
                    ShowUIElements(m_ManuaCancelLoginUIElements, true);
                    ShowUIElements(m_ManualLogoutUIElements, false);
                    break;
                case AuthenticationState.LoggedIn:
                    ShowUIElements(m_AwaitingAuthenticationInitializedUIElements, false);

                    ShowUIElements(m_ManualLoginUIElements, false);
                    ShowUIElements(m_ManuaCancelLoginUIElements, false);
                    ShowUIElements(m_ManualLogoutUIElements, requiresGUI);
                    break;
                case AuthenticationState.LoggedOut:
                    ShowUIElements(m_AwaitingAuthenticationInitializedUIElements, false);

                    ShowUIElements(m_ManualLoginUIElements, requiresGUI);
                    ShowUIElements(m_ManuaCancelLoginUIElements, false);
                    ShowUIElements(m_ManualLogoutUIElements, false);
                    break;
            }
        }

        void ShowUIElements(List<GameObject> gameObjects, bool visible)
        {
            gameObjects.ForEach(go => go.SetActive(visible));
        }
    }
}
