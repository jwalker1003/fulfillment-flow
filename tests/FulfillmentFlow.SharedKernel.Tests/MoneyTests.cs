using System.Globalization;
using FulfillmentFlow.SharedKernel;

namespace FulfillmentFlow.SharedKernel.Tests;

public sealed class MoneyTests
{
    [Theory]
    [InlineData("0")]
    [InlineData("0.01")]
    [InlineData("19.99")]
    [InlineData("1000000")]
    [InlineData("1.50")]
    [InlineData("1.500")]
    [Trait("Req", "NFR-7")]
    public void NFR_7_non_negative_amount_with_at_most_two_decimals_is_created(string amount)
    {
        var value = decimal.Parse(amount, CultureInfo.InvariantCulture);

        var result = Money.Create(value);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Amount.ShouldBe(value);
        result.Value.Currency.ShouldBe("USD");
    }

    [Theory]
    [InlineData("-0.01")]
    [InlineData("-5")]
    [InlineData("0.001")]
    [InlineData("19.999")]
    [Trait("Req", "NFR-7")]
    public void NFR_7_negative_amount_or_more_than_two_decimals_is_rejected(string amount)
    {
        var value = decimal.Parse(amount, CultureInfo.InvariantCulture);

        var result = Money.Create(value);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(SharedErrors.InvalidMoney);
        result.Error.Code.ShouldBe("shared.invalid-money");
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_zero_is_zero_usd()
    {
        var zero = Money.Zero;

        zero.Amount.ShouldBe(0m);
        zero.Currency.ShouldBe("USD");
        default(Money).ShouldBe(Money.Zero);
        default(Money).Currency.ShouldBe("USD");
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_money_adds()
    {
        var sum = M(10.25m) + M(0.80m);

        sum.ShouldBe(M(11.05m));
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_money_multiplies_by_quantity()
    {
        var total = M(19.99m) * Quantity.Create(3).Value;

        total.ShouldBe(M(59.97m));
        (M(19.99m) * Quantity.Zero).ShouldBe(Money.Zero);
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_money_with_the_same_amount_is_equal()
    {
        var first = M(1.5m);
        var second = M(1.50m);

        var equal = first == second;

        equal.ShouldBeTrue();
        first.GetHashCode().ShouldBe(second.GetHashCode());
        (first == M(1.51m)).ShouldBeFalse();
    }

    private static Money M(decimal amount) => Money.Create(amount).Value;
}
