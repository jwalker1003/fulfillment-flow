namespace FulfillmentFlow.SharedKernel;

/// <summary>Shared kernel error codes (docs/04-contracts/error-catalog.md, "Shared kernel").</summary>
public static class SharedErrors
{
    public const string InvalidSku = "shared.invalid-sku";
    public const string InvalidQuantity = "shared.invalid-quantity";
    public const string InvalidMoney = "shared.invalid-money";
    public const string InvalidGeoPoint = "shared.invalid-geo-point";
}
