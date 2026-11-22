namespace FulfillmentFlow.SharedKernel;

public sealed record Sku
{
    private Sku()
    {
    }

    public string Value => throw new NotImplementedException();

    public static Result<Sku> Create(string value) => throw new NotImplementedException();

    public static Sku From(string value) => throw new NotImplementedException();
}
