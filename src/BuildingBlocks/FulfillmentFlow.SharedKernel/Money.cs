namespace FulfillmentFlow.SharedKernel;

public readonly record struct Money
{
    public static Money Zero => throw new NotImplementedException();

    public decimal Amount => throw new NotImplementedException();

    public string Currency => throw new NotImplementedException();

    public static Result<Money> Create(decimal amount) => throw new NotImplementedException();

    public static Money operator +(Money left, Money right) => throw new NotImplementedException();

    public static Money operator *(Money money, Quantity quantity) => throw new NotImplementedException();
}
