namespace FulfillmentFlow.SharedKernel;

/// <summary>A <see cref="Result"/> that carries a value when it succeeds (ADR-0014).</summary>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(T? value, DomainError? error)
        : base(error) => _value = value;

    /// <summary>The value. Throws if the result is a failure; check <see cref="Result.IsSuccess"/> first.</summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"A failed result has no value ({Error.Code}).");

    public static implicit operator Result<T>(DomainError error) => Failure<T>(error);

    internal static Result<T> FromValue(T value) => new(value, null);

    internal static Result<T> FromError(DomainError error) => new(default, error);
}
