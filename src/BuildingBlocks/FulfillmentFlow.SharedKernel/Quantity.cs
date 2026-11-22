using System.Globalization;

namespace FulfillmentFlow.SharedKernel;

/// <summary>A non-negative whole number of units.</summary>
public readonly record struct Quantity : IComparable<Quantity>
{
    private Quantity(int value) => Value = value;

    public static Quantity Zero => default;

    public int Value { get; }

    public bool IsPositive => Value > 0;

    public static Result<Quantity> Create(int value)
    {
        if (value < 0)
        {
            return new DomainError(SharedErrors.InvalidQuantity, $"Quantity cannot be negative (was {value}).");
        }

        return Result.Success(new Quantity(value));
    }

    public static Quantity operator +(Quantity left, Quantity right) => new(checked(left.Value + right.Value));

    /// <summary>Subtracts. Callers must check <c>left &gt;= right</c> first; going below zero is a programming error.</summary>
    public static Quantity operator -(Quantity left, Quantity right) =>
        left.Value >= right.Value
            ? new Quantity(left.Value - right.Value)
            : throw new InvalidOperationException($"Cannot subtract {right.Value} from {left.Value}: quantity would be negative.");

    public static bool operator <(Quantity left, Quantity right) => left.Value < right.Value;

    public static bool operator >(Quantity left, Quantity right) => left.Value > right.Value;

    public static bool operator <=(Quantity left, Quantity right) => left.Value <= right.Value;

    public static bool operator >=(Quantity left, Quantity right) => left.Value >= right.Value;

    public int CompareTo(Quantity other) => Value.CompareTo(other.Value);

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
