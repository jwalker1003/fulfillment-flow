namespace FulfillmentFlow.Billing.Tests;

public sealed class TestProjectDiscoveryTests
{
    [Fact]
    [Trait("Req", "ADR-0013")]
    public void ADR_0013_test_project_is_discovered_by_the_runner()
    {
        var assembly = typeof(TestProjectDiscoveryTests).Assembly;

        var name = assembly.GetName().Name;

        name.ShouldBe("FulfillmentFlow.Billing.Tests");
    }
}
