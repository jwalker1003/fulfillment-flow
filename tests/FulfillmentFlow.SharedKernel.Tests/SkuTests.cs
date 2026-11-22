using FulfillmentFlow.SharedKernel;

namespace FulfillmentFlow.SharedKernel.Tests;

public sealed class SkuTests
{
    [Theory]
    [InlineData("ABC", "ABC")]
    [InlineData("ABC-1", "ABC-1")]
    [InlineData("  abc-123  ", "ABC-123")]
    [InlineData("9XY", "9XY")]
    [InlineData("A2345678901234567890123456789012", "A2345678901234567890123456789012")]
    [Trait("Req", "NFR-7")]
    public void NFR_7_valid_sku_is_created_trimmed_and_uppercased(string input, string expected)
    {
        var result = Sku.Create(input);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(expected);
        result.Value.ToString().ShouldBe(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("AB")]
    [InlineData("-AB")]
    [InlineData("AB_1")]
    [InlineData("AB 1")]
    [InlineData("ÄBC")]
    [InlineData("A23456789012345678901234567890123")]
    [Trait("Req", "NFR-7")]
    public void NFR_7_invalid_sku_is_rejected(string input)
    {
        var result = Sku.Create(input);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(SharedErrors.InvalidSku);
        result.Error.Code.ShouldBe("shared.invalid-sku");
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_null_sku_is_rejected()
    {
        var result = Sku.Create(null!);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe(SharedErrors.InvalidSku);
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_sku_from_returns_a_sku_for_trusted_input()
    {
        var sku = Sku.From("abc-1");

        sku.Value.ShouldBe("ABC-1");
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_sku_from_throws_for_invalid_input()
    {
        var from = () => Sku.From("x");

        from.ShouldThrow<ArgumentException>();
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_skus_with_the_same_normalised_value_are_equal()
    {
        var first = Sku.From("abc-1");
        var second = Sku.From(" ABC-1 ");

        var equal = first == second;

        equal.ShouldBeTrue();
        first.GetHashCode().ShouldBe(second.GetHashCode());
        (Sku.From("ABC-2") == first).ShouldBeFalse();
    }
}
