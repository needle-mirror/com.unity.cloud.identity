using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Cloud.Identity.Samples.GetUserInfo
{
    /// <summary>
    /// A Monobehaviour class to fetch user information using platform services.
    /// </summary>
    public class UserNameUpdater : MonoBehaviour
    {
        [SerializeField]
        Text m_UserInfoText;

        [SerializeField]
        ActiveUserController m_ActiveUserController;

        ICompositeAuthenticator m_CompositeAuthenticator;
        IAuthenticationStateProvider m_AuthenticationStateProvider => m_CompositeAuthenticator;
        IUserInfoProvider m_UserInfoProvider => m_CompositeAuthenticator;

        readonly List<IProject> m_Projects = new();
        readonly List<IMemberInfo> m_Members = new();
        readonly List<IMemberInfo> m_FirstProjectMembers = new();

        IOrganization SelectedOrganization;
        IEnumerable<Role> m_OrganizationRoles;
        IUserInfo m_UserInfo;
        IEntitlements m_Entitlements;

        void Awake()
        {
            m_CompositeAuthenticator = PlatformServices.CompositeAuthenticator;
            m_AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
            if (m_ActiveUserController)
            {
                m_ActiveUserController.OrganizationSelectionChanged +=  OnOrganizationSelectionChanged;
            }
        }

        async Task Start()
        {
            // Update UI with current state
            await ApplyAuthenticationState(m_AuthenticationStateProvider.AuthenticationState);
        }

        void OnDestroy()
        {
            m_AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            if (m_ActiveUserController)
            {
                m_ActiveUserController.OrganizationSelectionChanged -=  OnOrganizationSelectionChanged;
            }
        }

        async void OnAuthenticationStateChanged(AuthenticationState state)
        {
            await ApplyAuthenticationState(state);
        }

        async Task ApplyAuthenticationState(AuthenticationState state)
        {
            switch (state)
            {
                case AuthenticationState.AwaitingInitialization:
                case AuthenticationState.AwaitingLogout:
                case AuthenticationState.LoggedOut:
                    m_UserInfoText.text = "...";
                    m_Members.Clear();
                    m_Projects.Clear();
                    break;
                case AuthenticationState.AwaitingLogin:
                    m_UserInfoText.text = "Awaiting completion of a user initiated manual login operation...";
                    break;
                case AuthenticationState.LoggedIn:
                    await GetUserInfoAsync();
                    BuildUserInfoText();
                    break;
            }
        }

        async void OnOrganizationSelectionChanged(IOrganization organization)
        {
            await ApplyOrganizationSelectionChanged(organization);
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

        async Task ApplyOrganizationSelectionChanged(IOrganization organization)
        {
            if (organization == null)
                return;

            SelectedOrganization = organization;

            m_Entitlements = await organization.GetEntitlementsAsync();

            m_Members.Clear();

            var membersAsyncEnumerable = SelectedOrganization.ListMembersAsync(Range.All);
            await foreach (var member in membersAsyncEnumerable)
            {
                m_Members.Add(member);
            }

            m_OrganizationRoles = await SelectedOrganization.ListRolesAsync();

            m_Projects.Clear();

            var projectsAsyncEnumerable = SelectedOrganization.ListProjectsAsync(Range.All);
            await foreach (var project in projectsAsyncEnumerable)
            {
                m_Projects.Add(project);
            }

            m_FirstProjectMembers.Clear();
            if (m_Projects.Count > 0)
            {
                var projectMembersAsyncEnumerable = m_Projects[0].ListMembersAsync(Range.All);
                await foreach (var member in projectMembersAsyncEnumerable)
                {
                    m_FirstProjectMembers.Add(member);
                }
            }
            BuildUserInfoText(true);
        }

        void BuildUserInfoText(bool withProjects = false)
        {
            var sb = new StringBuilder();
            sb.Append(m_UserInfo == null ? "service account" : m_UserInfo.Name);
            if (m_CompositeAuthenticator.RequiresGUI)
            {
                sb.Append(" is logged in with an access token issued after a successful user initiated login operation.");
            }
            else
            {
                sb.Append(" logged in with an access token coming from an environment variable, a browser local storage or injected as a launch argument to the current process.");
            }
            if (withProjects)
            {
                sb.Append($"\n\n User has access to {m_Projects.Count} projects in '{SelectedOrganization.Name}' that has {m_Members.Count} members.");
                var organizationEntitlementsList = m_Entitlements.OrganizationEntitlements.ToList();
                var userSeatsList = m_Entitlements.UserSeats.ToList();
                var hasOrganizationEntitlements = organizationEntitlementsList.Count > 0;
                var hasUserSeats = userSeatsList.Count > 0;
                if (hasOrganizationEntitlements)
                {
                    var organizationEntitlementsListString = String.Join(", ", organizationEntitlementsList);
                    sb.Append($"\n\n Organization has {organizationEntitlementsList.Count} entitlements in organization '{SelectedOrganization.Name}'");
                    sb.Append($"\n\n {organizationEntitlementsListString}");
                }
                else
                {
                    sb.Append($"\n\n Organization has no entitlements in organization: '{SelectedOrganization.Name}'");
                }

                if (hasUserSeats)
                {
                    var userSeatsListString = String.Join(", ", userSeatsList);
                    sb.Append($"\n\n User has {userSeatsList.Count} seats in organization: '{SelectedOrganization.Name}'");
                    sb.Append($"\n\n {userSeatsListString}");
                }
                else
                {
                    sb.Append($"\n\n User has no seats in organization: '{SelectedOrganization.Name}'");
                }

                if (m_FirstProjectMembers.Count > 0)
                {
                    sb.Append($"\n\n The first project has {m_FirstProjectMembers.Count} member(s).");
                }

                if (m_Projects.Count > 0)
                {
                    var isEnabledInAssetManager = m_Projects[0].EnabledInAssetManager ? "enabled" : "not enabled";
                    sb.Append($"\n\n This project ({m_Projects[0].Name}) is {isEnabledInAssetManager} in the Asset Manager.");
                }

                var roleList = String.Join(", ", m_OrganizationRoles);
                sb.Append($"\n\n Full list of assigned roles in organization:\n {roleList}.");
            }
            m_UserInfoText.text = sb.ToString();
        }
    }
}
