using System;
using UnityEngine;
using Unity.Cloud.Common;
using Unity.Cloud.Identity.Runtime;

namespace Unity.Cloud.Identity.Documentation
{
    #region ServiceAccountAuthorizer

    public class ServiceAccountAuthorizerExample : MonoBehaviour
    {
        IServiceAuthorizer m_ServiceAccountAuthorizer;

        void Awake()
        {
            var authenticationPlatformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            m_ServiceAccountAuthorizer = new ServiceAccountAuthorizer(authenticationPlatformSupport);
        }
    }

    #endregion

}
