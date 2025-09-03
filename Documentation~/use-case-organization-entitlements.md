# Unity organization entitlements and user seats

Unity Cloud Identity package can list Unity Organization entitlements and user seats for a given Organization. 

Organization entitlements and user seats can be listed using awaitable methods from `IOrganization` implementation as it inherits the `IEntitlementsProvider` interface.

## Fetching Unity Organization Entitlements

From an `IOrganization` reference, you can call the inherited `IEntitlementsProvider.GetEntitlementsAsync()`
method to return the lists of Unity Organization entitlements. The `IEntitlement.OrganizationEntitlements` property holds the list of all entitlements of the Unity Organization.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/EntitlementsExample.cs#OrganizationEntitlements)]

## Fetching Unity Organization User Seats

From an `IOrganization` reference, you can call the inherited `IEntitlementsProvider.GetEntitlementsAsync()`
method to return the lists of Unity Organization entitlements. The `IEntitlement.UserSeats` property holds the list of all entitlements assigned to the current logged in user in this Unity Organization.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/EntitlementsExample.cs#UserSeats)]
