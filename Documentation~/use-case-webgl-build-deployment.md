# Use case: WebGL build & deployment recommendations

This section gathers all the relevant information required to build and deploy a WebGL build using the Identity SDK.

## Before you start

### Build recommendations

Before generating a WebGL build, make sure to have the right configuration in the Unity Editor **Player Settings**:

1. Select **Player Settings** > **Publishing Settings**.
2. Set **Compression Format** to **Disabled**.
3. Select **Player Settings** > **Other Settings**.
4. Under **Optimization** set **Manage Stripping Level** to **Minimal**.
5. Select **Build Settings...** > **Build** and follow instructions to generate a WebGL build.

### Host limitations

Identity's authentication engine calls specific Unity Cloud endpoints.
By default and for security reasons, these endpoints reject any request sent from any host that doesn't belong to the `*.unity.com` domain (including the `localhost` environment).

## How do I...?

### Upload a WebGL build on the Digital Twin Dashboard

To upload a WebGL build, follow these steps:

1. Log into the [Digital Twin Dashboard](https://dashboard.unity3d.com/digital-twins/).
2. Select **Asset Manager** > **Organization Workspace**.
3. Create a new dataset using the **+ New** button.
4. Select **folder** in the drag and drop field.
5. Select the folder containing your WebGL build.
6. Select **Create Digital Twin** to upload the files.

### Access and deploy a WebGL build with the Digital Twin Dashboard

Once the upload is completed, a summary of the new dataset details appears.

1. In the summary, make sure the **Public** option is checked. If the option is not checked by default, check it to make the dataset public.
2. Locate the `index.html` file.
3. Select and hold the file and select **Copy URL** from the dropdown menu.
4. Paste the URL in the address bar of your browser.
