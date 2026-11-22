using FulfillmentFlow.SharedKernel;

namespace FulfillmentFlow.SharedKernel.Tests;

public sealed class QuantityTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    [Trait("Req", "NFR-7")]
    public void NFR_7_non_negative_quantity_is_created(int value)
    {
        var result = Quantity.Create(value);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [Trait("Req", "NFR-7")]
    public void NFR_7_negative_quantity_is_rejected(int value)
    {
        var result = Quantity.Create(value);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(SharedErrors.InvalidQuantity);
        result.Error.Code.ShouldBe("shared.invalid-quantity");
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_zero_is_zero_and_not_positive()
    {
        var zero = Quantity.Zero;

        zero.Value.ShouldBe(0);
        zero.IsPositive.ShouldBeFalse();
        Q(1).IsPositive.ShouldBeTrue();
        default(Quantity).ShouldBe(Quantity.Zero);
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_quantities_add()
    {
        var sum = Q(2) + Q(3);

        sum.ShouldBe(Q(5));
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_adding_beyond_int_range_throws()
    {
        Action add = () => _ = Q(int.MaxValue) + Q(1);

        add.ShouldThrow<OverflowException>();
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_quantities_subtract()
    {
        var difference = Q(5) - Q(3);

        difference.ShouldBe(Q(2));
        (Q(3) - Q(3)).ShouldBe(Quantity.Zero);
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_subtracting_below_zero_throws()
    {
        Action subtract = () => _ = Q(2) - Q(3);

        subtract.ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_quantities_compare()
    {
        var two = Q(2);
        var three = Q(3);

        (two < three).ShouldBeTrue();
        (three > two).ShouldBeTrue();
        (two <= Q(2)).ShouldBeTrue();
        (two >= Q(2)).ShouldBeTrue();
        (three < two).ShouldBeFalse();
        (two > three).ShouldBeFalse();
        two.CompareTo(three).ShouldBeLessThan(0);
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_quantities_with_the_same_value_are_equal()
    {
        var first = Q(4);
        var second = Q(4);

        var equal = first == second;

        equal.ShouldBeTrue();
        (first != Q(5)).ShouldBeTrue();
        first.ToString().ShouldBe("4");
    }

    private static Quantity Q(int value) => Quantity.Create(value).Value;
}
