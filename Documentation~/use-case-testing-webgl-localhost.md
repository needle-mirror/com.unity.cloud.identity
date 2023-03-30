# Test with WebGL in a localhost

This section describes how to test a localhost WebGL build.

Identity's authentication engine must call specific cloud Unity Cloud endpoints. By default and for security reasons, these endpoints reject any request sent from a localhost environment (this is a CORS issue).

1. Build your project using the WebGL target.

2. Start your localhost project on port 55155, which is allowlisted to support this flow. You can use and adapt the following PowerShell script to automate starting your localhost project.

    ```powershell
        $monoPath = "C:/Program Files/Unity/Hub/Editor/2021.3.21f1/Editor/Data/MonoBleedingEdge/bin/mono.exe"
        $webServerPath = "C:/Program Files/Unity/Hub/Editor/2021.3.21f1/Editor/Data/PlaybackEngines/WebGLSupport/BuildTools/SimpleWebServer.exe"
        $buildPath = "path_to_my_build_folder"
        $portNumber = 55155

        &"$monoPath" "$webServerPath" "$buildPath" $portNumber
    ```

3. Test your project locally.
