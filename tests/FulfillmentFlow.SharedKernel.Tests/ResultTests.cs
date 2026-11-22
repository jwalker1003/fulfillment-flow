using FulfillmentFlow.SharedKernel;

namespace FulfillmentFlow.SharedKernel.Tests;

public sealed class ResultTests
{
    private static readonly DomainError SomeError = new("shared.some-rule", "Some rule was broken.");

    [Fact]
    [Trait("Req", "ADR-0014")]
    public void ADR_0014_domain_error_carries_code_and_message()
    {
        var error = new DomainError("inventory.insufficient-stock", "Not enough.");

        error.Code.ShouldBe("inventory.insufficient-stock");
        error.Message.ShouldBe("Not enough.");
        error.ShouldBe(new DomainError("inventory.insufficient-stock", "Not enough."));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("Req", "ADR-0014")]
    public void ADR_0014_domain_error_without_a_code_is_a_programming_error(string code)
    {
        var create = () => new DomainError(code, "message");

        create.ShouldThrow<ArgumentException>();
    }

    [Fact]
    [Trait("Req", "ADR-0014")]
    public void ADR_0014_success_result_is_success_and_has_no_error()
    {
        var result = Result.Success();

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        Should.Throw<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    [Trait("Req", "ADR-0014")]
    public void ADR_0014_failure_result_carries_the_error()
    {
        var result = Result.Failure(SomeError);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SomeError);
    }

    [Fact]
    [Trait("Req", "ADR-0014")]
    public void ADR_0014_domain_error_converts_implicitly_to_a_failed_result()
    {
        Result result = SomeError;

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SomeError);
    }

    [Fact]
    [Trait("Req", "ADR-0014")]
    public void ADR_0014_successful_typed_result_exposes_its_value()
    {
        var result = Result.Success(42);

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
        Should.Throw<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    [Trait("Req", "ADR-0014")]
    public void ADR_0014_reading_the_value_of_a_failed_typed_result_throws()
    {
        var result = Result.Failure<int>(SomeError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SomeError);
        Should.Throw<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    [Trait("Req", "ADR-0014")]
    public void ADR_0014_domain_error_converts_implicitly_to_a_failed_typed_result()
    {
        Result<string> result = SomeError;

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SomeError);
    }
}
