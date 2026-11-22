namespace FulfillmentFlow.SharedKernel;

public class Result
{
    public bool IsSuccess => throw new NotImplementedException();

    public bool IsFailure => throw new NotImplementedException();

    public DomainError Error => throw new NotImplementedException();

    public static Result Success() => throw new NotImplementedException();

    public static Result Failure(DomainError error) => throw new NotImplementedException();

    public static Result<T> Success<T>(T value) => throw new NotImplementedException();

    public static Result<T> Failure<T>(DomainError error) => throw new NotImplementedException();

    public static implicit operator Result(DomainError error) => throw new NotImplementedException();
}
