using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.AppLinking.Runtime;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity.Runtime;
using UnityEngine;

#pragma warning disable S1144 // Remove the unused private method

namespace Unity.Cloud.Identity.Documentation
{
    // Referenced:
    // - /Documentation~/entities-rbac.md
    namespace EntitiesRbacExample
    {
        public static class PlatformServices
        {
            static CompositeAuthenticator s_CompositeAuthenticator;
            public static ICompositeAuthenticator CompositeAuthenticator => s_CompositeAuthenticator;

            static void Create()
            {
                var httpClient = new UnityHttpClient();
                var playerSettings = UnityCloudPlayerSettings.Instance;
                var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
                var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

                var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, serviceHostResolver, playerSettings)
                    .AddDefaultBrowserAuthenticatedAccessTokenProvider(playerSettings)
                    .AddDefaultPkceAuthenticator(playerSettings)
                    .Build();

                s_CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);
            }
        }

        #region ListOrganizations
        public class ListOrganizationsBehaviour : MonoBehaviour
        {
            ICompositeAuthenticator m_CompositeAuthenticator;
            IOrganizationRepository m_OrganizationRepository => m_CompositeAuthenticator;
            readonly List<IOrganization> m_Organizations = new ();

            void Awake()
            {
                m_CompositeAuthenticator = PlatformServices.CompositeAuthenticator;
                m_CompositeAuthenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
            }

            async Task Start()
            {
                await ApplyAuthenticationState(m_CompositeAuthenticator.AuthenticationState);
            }

            void OnDestroy()
            {
                m_CompositeAuthenticator.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            }

            async void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
            {
                await ApplyAuthenticationState(newAuthenticationState);
            }

            async Task ApplyAuthenticationState(AuthenticationState state)
            {
                switch (state)
                {
                    case AuthenticationState.AwaitingInitialization:
                    case AuthenticationState.AwaitingLogin:
                    case AuthenticationState.AwaitingLogout:
                        break;
                    case AuthenticationState.LoggedIn:
                        var organizationsAsyncEnumerable = m_OrganizationRepository.ListOrganizationsAsync(Range.All);
                        await foreach (var organization in organizationsAsyncEnumerable)
                        {
                            m_Organizations.Add(organization);
                        }
                        break;
                    case AuthenticationState.LoggedOut:
                        break;
                }
            }
        }
        #endregion

        public class ListOrganizationProjectsBehaviour : MonoBehaviour
        {
            #region ListOrganizationProjects
            readonly List<IProject> m_Projects = new ();

            async Task FetchOrganizationProjects(IOrganization organization)
            {
                m_Projects.Clear();
                var projectsAsyncEnumerable = organization.ListProjectsAsync(Range.All);
                await foreach (var project in projectsAsyncEnumerable)
                {
                    m_Projects.Add(project);
                }
            }
            #endregion
        }

        public class ListOrganizationRolesBehaviour : MonoBehaviour
        {
            #region ListOrganizationRoles
            async Task FetchOrganizationRoles(IOrganization organization)
            {
                var organizationRoles = await organization.ListRolesAsync();
                if (organizationRoles.HasRole(Role.Owner))
                {
                    // Organization Owner specific logic
                }
            }
            #endregion
        }

        public class ListProjectPermissionsBehaviour : MonoBehaviour
        {
            #region ListProjectPermissions
            readonly Permission m_AssetManagerCreatorPermission = new Permission("amc.assets.create");

            async Task FetchProjectPermissions(IProject project)
            {
                var projectPermissions = await project.ListPermissionsAsync();
                if (projectPermissions.HasPermission(m_AssetManagerCreatorPermission))
                {
                    // Project Asset Manager Creator specific logic
                }
            }
            #endregion
        }
    }
}

#pragma warning restore S1144
