# Cloud Storage Usage and Metered Billing Information

Use the following methods of the Unity Cloud Identity package to fetch Cloud storage usage and check billing information for a single organization:

## Fetch an organization's Cloud storage usage

The `IOrganization` interface inherits the `GetCloudStorageUsageAsync()` method from the `ICloudStorageInfoProvider interface to fetch the total usage bytes and the total storage quota bytes that are available for that organization.

>[!NOTE]
>When metered billing is activated for an organization, the usage bytes can exceed the total storage quota bytes.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/CloudStorageExample.cs#GetOrganizationStorageUsage)]

## Check whether an organization has activated metered billing

The `IOrganization` interface inherits the `HasMeteredBillingActivatedAsync()` method from the `ICloudStorageInfoProvider` interface to check whether metered billing is activated for the organization.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/CloudStorageExample.cs#GetOrganizationMeteredBilling)]

