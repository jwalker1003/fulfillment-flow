namespace FulfillmentFlow.SharedKernel;

public sealed class Result<T> : Result
{
    public T Value => throw new NotImplementedException();

    public static implicit operator Result<T>(DomainError error) => throw new NotImplementedException();
}
