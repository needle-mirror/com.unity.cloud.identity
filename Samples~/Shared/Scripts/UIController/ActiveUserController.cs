#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Unity.Cloud.Identity.Samples
{
    using System.Linq;

    public class ActiveUserController : MonoBehaviour
    {
        public event Action<IOrganization> OrganizationSelectionChanged;

        [SerializeField]
        Button m_LoginButton;

        [SerializeField]
        Button m_CancelLoginButton;

        [SerializeField]
        Button m_LogoutButton;

        [SerializeField]
        Button m_SignOutButton;

        [SerializeField]
        Button m_OwnerButton;

        [SerializeField]
        Dropdown m_OrganizationsDropdown;

        [SerializeField]
        UIController m_UIController;

        [SerializeField]
        Text m_UserNameText;

        ICompositeAuthenticator m_CompositeAuthenticator;
        IAuthenticatedUserInfoProvider m_AuthenticatedUserInfoProvider => m_CompositeAuthenticator;
        IOrganizationRepository m_OrganizationRepository => m_CompositeAuthenticator;

        IEnumerable<IOrganization> m_Organizations;

        IOrganization SelectedOrganization;

        readonly List<IOrganization> m_OrganizationDropdownValue = new();

        [SerializeField]
        UnityEvent m_UserUnauthorized;

        void Start()
        {
            RegisterButtons();

            UpdateButton(m_OwnerButton, false);

            if (m_OrganizationsDropdown != null)
            {
                m_OrganizationsDropdown.enabled = false;
                m_OrganizationsDropdown.onValueChanged.AddListener(ApplyDropDownValueChanged);
            }

            if (m_CompositeAuthenticator == null)
            {
                m_CompositeAuthenticator = PlatformServices.CompositeAuthenticator;
                m_CompositeAuthenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;


                // Update UI with current state
                _ = ApplyAuthenticationState(m_CompositeAuthenticator.AuthenticationState);
            }

        }

        void OnDestroy()
        {
            UnregisterButtons();
            m_OrganizationsDropdown?.onValueChanged.RemoveAllListeners();
            m_CompositeAuthenticator.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        public void Login()
        {
            try
            {
                m_CompositeAuthenticator.LoginAsync();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException
                    or AuthenticationFailedException)
                {
                    Debug.LogError(ex.Message);
                }
                throw;
            }
        }

        public void CancelLogin()
        {
            try
            {
                m_CompositeAuthenticator.CancelLogin();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                {
                    Debug.LogError(ex.Message);
                }
                throw;
            }
        }

        public void Logout()
        {
            try
            {
                m_CompositeAuthenticator.LogoutAsync();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException
                    or AuthenticationFailedException)
                {
                    Debug.LogError(ex.Message);
                }
                throw;
            }
        }

        void SignOut()
        {
            try
            {
                m_CompositeAuthenticator.LogoutAsync(true);
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException
                    or AuthenticationFailedException)
                {
                    Debug.LogError(ex.Message);
                }
                throw;
            }
        }

        void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
        {
            _ = ApplyAuthenticationState(newAuthenticationState);
        }

        async Task ApplyAuthenticationState(AuthenticationState state)
        {
            // Clear status text on authentication change
            m_UserNameText.text = string.Empty;
            switch (state)
            {
                case AuthenticationState.AwaitingInitialization:
                case AuthenticationState.AwaitingLogin:
                case AuthenticationState.AwaitingLogout:
                    UpdateButton(m_LoginButton, false);
                    UpdateButton(m_LogoutButton, false);
                    UpdateButton(m_SignOutButton, false);
                    break;
                case AuthenticationState.LoggedIn:
                    UpdateButton(m_LoginButton, false);
                    UpdateButton(m_LogoutButton, m_CompositeAuthenticator.RequiresGUI);
                    UpdateButton(m_SignOutButton, m_CompositeAuthenticator.RequiresGUI);

                    m_Organizations = await m_OrganizationRepository.ListOrganizationsAsync();
                    SelectedOrganization = m_Organizations.FirstOrDefault(p => p.Role.Equals("owner")) ?? m_Organizations.ElementAt(0);

                    FillOrganizationsDropDown();
                    break;
                case AuthenticationState.LoggedOut:
                    UpdateButton(m_LoginButton, m_CompositeAuthenticator.RequiresGUI);
                    UpdateButton(m_LogoutButton, false);
                    UpdateButton(m_SignOutButton, false);
                    m_UserNameText.text = "No User";
                    m_OrganizationDropdownValue.Clear();
                    if (m_OrganizationsDropdown != null)
                        m_OrganizationsDropdown.enabled = false;
                    break;
            }
        }

        void FillOrganizationsDropDown()
        {
            m_OrganizationsDropdown?.ClearOptions();
            m_OrganizationDropdownValue.Clear();

            int selectedOrganizationIndex = -1;
            var list = new List<Dropdown.OptionData>();
            if (m_Organizations != null && m_OrganizationsDropdown != null)
            {
                m_OrganizationsDropdown.enabled = true;

                foreach (var org in m_Organizations)
                {
                    list.Add(new Dropdown.OptionData(org.Name));
                    m_OrganizationDropdownValue.Add(org);

                    if (SelectedOrganization != null && SelectedOrganization.Id.Equals(org.Id))
                    {
                        selectedOrganizationIndex = list.Count - 1;
                    }
                }
            }

            if (list.Count > 0)
            {
                m_OrganizationsDropdown?.AddOptions(list);
            }

            if (selectedOrganizationIndex != -1 && m_OrganizationsDropdown != null)
            {
                m_OrganizationsDropdown.value = selectedOrganizationIndex;
            }

        }

        void ApplyDropDownValueChanged(int value)
        {
            SelectedOrganization = m_OrganizationDropdownValue.ElementAt(value);
            Debug.Log($"Selected org '{SelectedOrganization.Id}'");
            _ = OnSelectOrganization();
            OrganizationSelectionChanged?.Invoke(SelectedOrganization);
        }

        async Task OnSelectOrganization()
        {
            var hasOwnerRole = await SelectedOrganization.HasRoleAsync("owner");
            // Enable the button only if the user is the owner of the organization.
            UpdateButton(m_OwnerButton, hasOwnerRole);

            var username = m_AuthenticatedUserInfoProvider.GetUserInfo(AuthenticatedUserInfoClaims.Name);
            m_UserNameText.text = !string.IsNullOrEmpty(username) ? $"{username} : {SelectedOrganization.Role}" : "No User";
        }

        static void UpdateButton(Button button, bool enabled)
        {
            if (button != null)
                button.interactable = enabled;
        }

        void RegisterButtons()
        {
            if (m_LoginButton != null)
                m_LoginButton.onClick.AddListener(Login);
            if(m_CancelLoginButton != null)
                m_CancelLoginButton.onClick.AddListener(CancelLogin);
            if (m_LogoutButton != null)
                m_LogoutButton.onClick.AddListener(Logout);
            if (m_SignOutButton != null)
                m_SignOutButton.onClick.AddListener(SignOut);
        }

        void UnregisterButtons()
        {
            if (m_LoginButton != null)
                m_LoginButton.onClick.RemoveListener(Login);
            if(m_CancelLoginButton != null)
                m_CancelLoginButton.onClick.RemoveListener(CancelLogin);
            if (m_LogoutButton != null)
                m_LogoutButton.onClick.RemoveListener(Logout);
            if (m_SignOutButton != null)
                m_SignOutButton.onClick.RemoveListener(SignOut);

        }

    }
}
#endif
