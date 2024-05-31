# Cloud Storage Usage and Metered Billing Information

Unity Cloud Identity package exposes methods to fetch the Cloud storage usage and billing information for a single organization.

## Fetching an organization Cloud storage usage

The `IOrganization` inherits the `ICloudStorageInfoProvider` interface `GetCloudStorageUsageAsync()` method to fetch the total usage bytes and the total storage quota bytes available.

>[!NOTE]
>When metered billing is activated for an organization, the usage bytes may exceed the total storage quota bytes.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/CloudStorageExample.cs#GetOrganizationStorageUsage)]

## Knowing if an organization has activated metered billing

The `IOrganization` inherits the `ICloudStorageInfoProvider` interface `HasMeteredBillingActivatedAsync()` method to fetch if it has metered billing activated.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/CloudStorageExample.cs#GetOrganizationMeteredBilling)]

