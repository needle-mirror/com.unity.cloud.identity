using System;
using System.Collections.Generic;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// This interface abstracts platform-specific logic to handle application activation from a URL or key value pairs.
    /// </summary>
    public interface IActivatePlatformSupport
    {
        /// <summary>
        /// The URL used to activate the application.
        /// </summary>
        string ActivationUrl { get; }

        /// <summary>
        /// The key value pairs used to activate the application.
        /// </summary>
        Dictionary<string, string> ActivationKeyValue { get; }
    }
}
