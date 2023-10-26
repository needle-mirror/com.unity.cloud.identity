#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        IAuthenticationStateProvider m_AuthenticationStateProvider;
        ICompositeAuthenticator m_CompositeAuthenticator;
        IAuthenticatedUserInfoProvider m_AuthenticatedUserInfoProvider;

        readonly List<IProject> m_Projects = new();
        IOrganization SelectedOrganization;

        void Awake()
        {
            m_AuthenticationStateProvider = PlatformServices.AuthenticationStateProvider;
            m_CompositeAuthenticator = PlatformServices.CompositeAuthenticator;
            m_AuthenticatedUserInfoProvider = PlatformServices.AuthenticatedUserInfoProvider;

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

        void OnAuthenticationStateChanged(AuthenticationState state)
        {
            _ = ApplyAuthenticationState(state);
        }

        async Task ApplyAuthenticationState(AuthenticationState state)
        {
            switch (state)
            {
                case AuthenticationState.AwaitingInitialization:
                case AuthenticationState.AwaitingLogout:
                case AuthenticationState.LoggedOut:
                    m_UserInfoText.text = "...";
                    m_Projects.Clear();
                    break;
                case AuthenticationState.AwaitingLogin:
                    m_UserInfoText.text = "Awaiting completion of a user initiated manual login operation...";
                    break;
                case AuthenticationState.LoggedIn:
                    BuildUserInfoText();
                    break;
            }
        }

        void OnOrganizationSelectionChanged(IOrganization organization)
        {
            _ = ApplyOrganizationSelectionChanged(organization);
        }

        async Task ApplyOrganizationSelectionChanged(IOrganization organization)
        {
            SelectedOrganization = organization;
            m_Projects.Clear();

            var projectsEnumerator = SelectedOrganization.ListProjectsAsync(Range.All).GetAsyncEnumerator();
            while (await projectsEnumerator.MoveNextAsync())
            {
                m_Projects.Add(projectsEnumerator.Current);
            }
            BuildUserInfoText(true);
        }

        void BuildUserInfoText(bool withProjects = false)
        {
            var sb = new StringBuilder();
            sb.Append(m_AuthenticatedUserInfoProvider.GetUserInfo(AuthenticatedUserInfoClaims.Name));
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
                sb.Append($"\n\n User has access to {m_Projects.Count()} projects in '{SelectedOrganization.Name}'.");
            }
            m_UserInfoText.text = sb.ToString();
        }
    }
}
#endif
