using System.Text.RegularExpressions;

namespace FulfillmentFlow.SharedKernel;

/// <summary>A stock-keeping unit identifier: uppercase letters, digits and '-', 3 to 32 characters.</summary>
public sealed partial record Sku
{
    private Sku(string value) => Value = value;

    public string Value { get; }

    public static Result<Sku> Create(string value)
    {
        var normalised = value?.Trim().ToUpperInvariant() ?? string.Empty;

        if (!SkuFormat().IsMatch(normalised))
        {
            return new DomainError(
                SharedErrors.InvalidSku,
                $"'{value}' is not a valid SKU: use 3-32 letters, digits or '-', starting with a letter or digit.");
        }

        return Result.Success(new Sku(normalised));
    }

    /// <summary>Creates a SKU from trusted data (tests, persistence). Throws if the value is invalid.</summary>
    public static Sku From(string value)
    {
        var result = Create(value);

        return result.IsSuccess ? result.Value : throw new ArgumentException(result.Error.Message, nameof(value));
    }

    public override string ToString() => Value;

    [GeneratedRegex("^[A-Z0-9][A-Z0-9-]{2,31}$", RegexOptions.CultureInvariant)]
    private static partial Regex SkuFormat();
}
