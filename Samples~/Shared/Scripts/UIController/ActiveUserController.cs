using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.Common;
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
        Dropdown m_OrganizationsDropdown;

        [SerializeField]
        UIController m_UIController;

        [SerializeField]
        Text m_UserNameText;

        ICompositeAuthenticator m_CompositeAuthenticator;
        IOrganizationRepository m_OrganizationRepository => m_CompositeAuthenticator;
        IUserInfoProvider m_UserInfoProvider => m_CompositeAuthenticator;

        readonly List<IOrganization> m_Organizations = new();

        IOrganization SelectedOrganization;

        readonly List<IOrganization> m_OrganizationDropdownValue = new();
        private IUserInfo m_UserInfo;

        async Task Start()
        {
            RegisterButtons();

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
                await ApplyAuthenticationState(m_CompositeAuthenticator.AuthenticationState);
            }

        }

        void OnDestroy()
        {
            UnregisterButtons();
            m_OrganizationsDropdown?.onValueChanged.RemoveAllListeners();
            m_CompositeAuthenticator.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        void Login()
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

        void CancelLogin()
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

        void Logout()
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

        async void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
        {
            await ApplyAuthenticationState(newAuthenticationState);
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
                    ClearUserInformation();
                    break;
                case AuthenticationState.LoggedIn:
                    UpdateButton(m_LoginButton, false);
                    UpdateButton(m_LogoutButton, m_CompositeAuthenticator.RequiresGUI);
                    UpdateButton(m_SignOutButton, m_CompositeAuthenticator.RequiresGUI);
                    await GetUserInfoAsync();
                    var organizationsAsyncEnumerable = m_OrganizationRepository.ListOrganizationsAsync(Range.All);
                    await foreach (var organization in organizationsAsyncEnumerable)
                    {
                        m_Organizations.Add(organization);
                    }
                    BuildOrganizationsDropDown();
                    break;
                case AuthenticationState.LoggedOut:
                    UpdateButton(m_LoginButton, m_CompositeAuthenticator.RequiresGUI);
                    UpdateButton(m_LogoutButton, false);
                    UpdateButton(m_SignOutButton, false);
                    ClearUserInformation();
                    break;
            }
        }

        void ClearUserInformation()
        {
            m_UserNameText.text = "No User";
            m_Organizations.Clear();
            SelectedOrganization = null;
            if (m_OrganizationsDropdown != null)
            {
                m_OrganizationDropdownValue.Clear();
                m_OrganizationsDropdown.ClearOptions();
                m_OrganizationsDropdown.enabled = false;
            }
        }

        async Task GetUserInfoAsync()
        {
            try
            {
                m_UserInfo = await m_UserInfoProvider.GetUserInfoAsync();
            }
            catch (NotImplementedException)
            {
                // Not implemented
            }
        }

        void BuildOrganizationsDropDown()
        {
            var list = new List<Dropdown.OptionData>();
            if (m_OrganizationsDropdown != null)
            {
                var FirstOwnerOrganizationOrDefault = m_Organizations.FirstOrDefault(p => p.Role.Equals("owner")) ?? m_Organizations.ElementAt(0);
                var selectedOrganizationIndex = 0;
                if (m_Organizations != null && m_Organizations.Any())
                {
                    m_OrganizationsDropdown.ClearOptions();
                    m_OrganizationDropdownValue.Clear();

                    m_OrganizationsDropdown.enabled = true;

                    foreach (var org in m_Organizations)
                    {
                        list.Add(new Dropdown.OptionData(org.Name));
                        if (org.Id.Equals(FirstOwnerOrganizationOrDefault.Id))
                        {
                            selectedOrganizationIndex = list.Count - 1;
                        }

                        m_OrganizationDropdownValue.Add(org);
                    }
                }
                FillOrganizationsDropDown(list, selectedOrganizationIndex);
            }
        }

        void FillOrganizationsDropDown(List<Dropdown.OptionData> list, int selectionIndex)
        {
            if (list.Count > 0)
            {
                m_OrganizationsDropdown.AddOptions(list);
                if (m_OrganizationsDropdown.value != selectionIndex)
                {
                    m_OrganizationsDropdown.value = selectionIndex;
                }
                if (SelectedOrganization == null)
                    ApplyDropDownValueChanged(selectionIndex);
            }
        }

        void ApplyDropDownValueChanged(int value)
        {
            SelectedOrganization = m_OrganizationDropdownValue.ElementAt(value);
            Debug.Log($"Selected org '{SelectedOrganization.Id}'");
            OnSelectOrganization();
            OrganizationSelectionChanged?.Invoke(SelectedOrganization);
        }

        void OnSelectOrganization()
        {
            var username = m_UserInfo == null ? "service account": m_UserInfo.Name;
            m_UserNameText.text = !string.IsNullOrEmpty(username) ? $"{username}" : "No User";
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
