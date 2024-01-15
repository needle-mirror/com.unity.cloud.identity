using System.Runtime.CompilerServices;
using Unity.Cloud.Common;

[assembly: ApiSourceVersion("com.unity.cloud.identity", "1.0.0-pre.5")]
#if !(UC_NUGET)
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Tests")]
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Tests.Editor")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // to allow moq implementations for internal interfaces
#endif
