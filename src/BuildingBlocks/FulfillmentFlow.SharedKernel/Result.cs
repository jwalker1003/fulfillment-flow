namespace FulfillmentFlow.SharedKernel;

/// <summary>
/// The outcome of an operation that can break a business rule (ADR-0014).
/// A failure carries a <see cref="DomainError"/>; exceptions are for programming errors only.
/// </summary>
public class Result
{
    private static readonly Result SuccessResult = new(null);

    private readonly DomainError? _error;

    private protected Result(DomainError? error) => _error = error;

    public bool IsSuccess => _error is null;

    public bool IsFailure => !IsSuccess;

    /// <summary>The broken rule. Throws if the result is a success.</summary>
    public DomainError Error =>
        _error ?? throw new InvalidOperationException("A successful result has no error.");

    public static Result Success() => SuccessResult;

    public static Result Failure(DomainError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result(error);
    }

    public static Result<T> Success<T>(T value) => Result<T>.FromValue(value);

    public static Result<T> Failure<T>(DomainError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return Result<T>.FromError(error);
    }

    public static implicit operator Result(DomainError error) => Failure(error);
}
