using System.Runtime.CompilerServices;
using Unity.Cloud.Common;

[assembly: ApiSourceVersion("com.unity.cloud.identity", "1.0.0-exp.1")]
#if !(UC_NUGET)
[assembly: InternalsVisibleTo("Unity.Cloud.Identity.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // to allow moq implementations for internal interfaces
#endif
