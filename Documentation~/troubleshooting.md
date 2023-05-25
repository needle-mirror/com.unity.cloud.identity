# Troubleshooting

This section describes issues you might have while using the package.

## You have runtime errors when building the package

On some platforms such as WebGL, code stripping levels can cause runtime errors.

Make sure to set the **Managed stripping level** option to:

* **Disabled**
<br/> or
* **Minimal** (if the **Disabled** option isn't available)

![Managed Stripping Level dropdown](images/stripping-level.png)

See [Manage the package stripping level](getting-started.md#manage-the-package-stripping-level) for more information.
