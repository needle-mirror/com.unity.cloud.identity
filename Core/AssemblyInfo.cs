using System.Runtime.CompilerServices;
using Unity.Cloud.Common;

[assembly: ApiSourceVersion("com.unity.cloud.identity", "1.7.0")]
#if !(UC_NUGET)
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Tests")]
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.TestingTools")]
[assembly: InternalsVisibleTo("Unity.Cloud.TestingTools.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Debugging.Runtime")]
[assembly: InternalsVisibleTo("Unity.Cloud.Debugging.Editor")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // to allow moq implementations for internal interfaces
#endif
