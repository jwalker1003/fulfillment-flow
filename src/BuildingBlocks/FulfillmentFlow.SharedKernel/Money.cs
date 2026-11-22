using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FulfillmentFlow.SharedKernel;

/// <summary>A non-negative amount in USD with at most 2 decimal places.</summary>
public readonly record struct Money
{
    private const string Usd = "USD";

    private Money(decimal amount) => Amount = amount;

    public static Money Zero => default;

    public decimal Amount { get; }

    /// <summary>Always USD in v1.</summary>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Currency belongs to each Money value; it is constant only while v1 is USD-only.")]
    public string Currency => Usd;

    public static Result<Money> Create(decimal amount)
    {
        if (amount < 0m || decimal.Round(amount, 2) != amount)
        {
            return new DomainError(
                SharedErrors.InvalidMoney,
                $"Money must be a non-negative amount with at most 2 decimal places (was {amount.ToString(CultureInfo.InvariantCulture)}).");
        }

        return Result.Success(new Money(amount));
    }

    public static Money operator +(Money left, Money right) => new(left.Amount + right.Amount);

    public static Money operator *(Money money, Quantity quantity) => new(money.Amount * quantity.Value);

    public override string ToString() => $"{Amount.ToString("0.00", CultureInfo.InvariantCulture)} {Currency}";
}
