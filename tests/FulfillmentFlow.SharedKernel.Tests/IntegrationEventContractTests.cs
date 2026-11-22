using FulfillmentFlow.BuildingBlocks.Contracts;

namespace FulfillmentFlow.SharedKernel.Tests;

public sealed class IntegrationEventContractTests
{
    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_integration_event_interface_uses_only_primitives()
    {
        var type = typeof(IIntegrationEvent);

        var properties = type.GetProperties().OrderBy(p => p.Name, StringComparer.Ordinal).ToList();

        type.IsInterface.ShouldBeTrue();
        type.IsPublic.ShouldBeTrue();
        properties.Select(p => $"{p.Name}:{p.PropertyType.Name}").ShouldBe(["MessageId:Guid", "OccurredAt:DateTimeOffset"]);
        properties.ShouldAllBe(p => p.CanRead && !p.CanWrite);
    }

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_building_blocks_contracts_hold_only_the_integration_event_interface()
    {
        var assembly = typeof(IIntegrationEvent).Assembly;

        var exported = assembly.GetExportedTypes();
        var references = assembly.GetReferencedAssemblies().Select(a => a.Name!).ToList();

        exported.ShouldBe([typeof(IIntegrationEvent)]);
        references.ShouldNotContain("FulfillmentFlow.SharedKernel");
    }

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_a_record_with_message_id_and_occurred_at_satisfies_the_interface()
    {
        var id = Guid.NewGuid();
        var at = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        IIntegrationEvent message = new SampleIntegrationEvent(id, at, "ABC-1");

        message.MessageId.ShouldBe(id);
        message.OccurredAt.ShouldBe(at);
    }

    private sealed record SampleIntegrationEvent(Guid MessageId, DateTimeOffset OccurredAt, string Sku) : IIntegrationEvent;
}
