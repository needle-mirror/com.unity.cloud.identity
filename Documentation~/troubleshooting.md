# Troubleshooting

This section describes issues you might have while using the package.

## Build settings

On certain platforms such as WebGL, code stripping levels can cause certain runtime errors to occur.

When making builds that include this package, the **Managed Stripping Level** should be set to "Disabled" if the option is available for the chosen platform, else set to "Minimal".

![Managed Stripping Level dropdown](images/stripping-level.png)
