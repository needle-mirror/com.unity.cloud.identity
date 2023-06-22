# WebGL build & deployment recommendations

This section gathers all the relevant information required to build and deploy a WebGL build using the Identity SDK.

## Build recommendations

Before generating a WebGL build, make sure to have the right configuration in the Unity Editor **Player Settings**:

1. Select **Player Settings**> **Publishing Settings**
2. Set **Compression Format** to **Disabled**
3. Select **Player Settings**> **Other Settings**
4. Under **Optimization** set **Manage Stripping Level** to **Mininal**
5. Select **Build Settings...**> **Build** and follow instructions to generate a WebGL build.

## Deployment recommendations

### Host limitations

Identity's authentication engine calls specific cloud Unity Cloud endpoints.
By default and for security reasons, these endpoints reject any request sent from any host that doesn't belong to the `*.unity.com` domain (including the `localhost` environment).

The only valid option to deploy your WebGL build currently is to upload it on Unity Dashboard. This is described in details in the following section. 

### Upload a WebGL build on Unity Dashboard

To upload a WebGL build, follow these steps:

1. Log into the [Unity Dashboard](https://dashboard.unity3d.com/digital-twins/).
2. Select **Asset Manager** > **Organization Workspace**.
3. Create a new Dataset using the **+new** button.
4. Select **folder** in the Drag&Drop file area and locate the containing folder of your build on your machine.
5. Select **Create Digital Twin** to upload the files.
6. Select **Publish**, then from the prompt check the **Product** and confirm by selecting **Publish**.
7. Select the **Dataset tab** to display all uploaded files.
8. Locate the index.html file and select the **Copy URL** from the options dropdown menu.
9. Paste the URL from your clipboard in the address bar of your browser.