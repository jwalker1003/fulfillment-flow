namespace FulfillmentFlow.SharedKernel;

public sealed record DomainError
{
    public DomainError(string code, string message) => throw new NotImplementedException();

    public string Code => throw new NotImplementedException();

    public string Message => throw new NotImplementedException();
}
