# Best practice | Dependency injection

This section explains how to follow best practices when using the Unity Cloud SDKs.

## Overview

The Unity Cloud SDKs have APIs that follow the dependency injection pattern, which means each service constructor requires you to provide dependencies based on your preferences.

This pattern is useful for advanced use cases because it simplifies the customization of specific components and uses the default implementation for the remaining components. You can use the default implementations for simple usage, but you must link the services together.

The samples and documentation for Identity use a best practice pattern and this section explains how that best practice pattern works.

## Implement the PlatformServices pattern

1. Create a `PlatformServices` static class with `Create`,  `InitializeAsync` and `Shutdown` methods. This class initializes and disposes of all Unity Cloud services, while also publicly exposing the services as singletons.

    ```csharp
        public static class PlatformServices
        {
            public static void Create()
            {
                
            }

            public static async Task InitializeAsync()
            {
                
            }

            public static void Shutdown()
            {
                
            }
        }
    ```

2. Create a `PlatformServicesInitialization` MonoBehaviour that calls the `PlatformService`'s `Create` in `Awake` and `InitializeAsync` in `Start`. The `DefaultExecutionOrder` attribute ensures this script is called before any other script in your project and `DontDestroyOnLoad` ensures the GameObject isn't destroyed when loading another scene.

    ```csharp
        [DefaultExecutionOrder(int.MinValue)]
        public class PlatformServicesInitialization : MonoBehaviour
        {
            void Awake()
            {
                DontDestroyOnLoad(gameObject);
                PlatformServices.Create();
            }
            
            async Task Start()
            {
                await PlatformServices.InitializeAsync();
            }
        }
    ```

3. Create a `PlatformServicesShutdown` MonoBehavior that calls the `PlatformService`'s `Shutdown` in `OnDestroy`. The `DefaultExecutionOrder` attribute ensures this script is called after any other script in your project.

    ```csharp
        [DefaultExecutionOrder(int.MaxValue)]
        public class PlatformServicesShutdown : MonoBehaviour
        {
            void OnDestroy()
            {
                PlatformServices.Shutdown();
            }
        }
    ```

4. Create a `PlatformServices` GameObject and attach the `PlatformServicesInitialization` and `PlatformServicesShutdown` scripts. Make sure this GameObject instantiates only once in your project, which is when the application launches.
5. Populate the `PlatformServices` class with the services you require.
