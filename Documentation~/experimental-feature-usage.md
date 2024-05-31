# WebGL Proxy Experimental Support

This version of the Unity Cloud Identity package introduces experimental endpoints that will allow access from any origin. Once enabled, WebGL builds can be hosted on any domain and support user authentication, token management and access authorized entities such as organizations, projects, roles and permissions.

This experimental feature can be enabled by defining the EXPERIMENTAL_WEBGL_PROXY compile flag in the Unity Editor player settings.

## Define the EXPERIMENTAL_WEBGL_PROXY compile flag

To define the compile flag, follow these steps:

1. In your Unity Editor Project window, go to **Edit** > **Project settings**. <br/> The Project setting window opens.
2. Select the **Player** option.
3. Scroll to the **Script Compilation** section.
4. Add the **EXPERIMENTAL_WEBGL_PROXY** compile flag to the **Scripting Define Symbols** list.

## Additional WebGL required player settings 

To avoid runtime errors when building with this package, follow these steps:

1. In your Unity Editor Project window, go to **Edit** > **Project settings**. <br/> The Project setting window opens.
2. Select the **Player** option.
3. Scroll to the **Optimization** section.
4. Set the **Managed stripping level** option to:

- **Disabled**
<br/> or
- **Minimal** (if the **Disabled** option isn't available)

![Managed Stripping Level dropdown](images/stripping-level.png)
