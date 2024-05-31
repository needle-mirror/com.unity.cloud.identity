using System.Collections.Generic;

namespace Unity.Cloud.Identity
{
    internal interface ICloudStorageEntitlements
    {
        public bool MeteredOptInEnabled { get; set; }
    }
}
