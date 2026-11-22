namespace FulfillmentFlow.SharedKernel;

/// <summary>
/// A broken business rule (ADR-0014). <see cref="Code"/> is a stable code from
/// docs/04-contracts/error-catalog.md, for example <c>inventory.insufficient-stock</c>.
/// </summary>
public sealed record DomainError
{
    public DomainError(string code, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Code = code;
        Message = message ?? string.Empty;
    }

    public string Code { get; }

    public string Message { get; }
}
