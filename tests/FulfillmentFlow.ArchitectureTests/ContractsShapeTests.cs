using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FulfillmentFlow.BuildingBlocks.Contracts;

namespace FulfillmentFlow.ArchitectureTests;

/// <summary>
/// Contracts shape (ADR-0005): what a context publishes is immutable data. Every public type in a
/// <c>*.Contracts</c> assembly is a sealed record, and every <c>*IntegrationEvent</c> (or versioned
/// <c>*IntegrationEventV2</c>, docs/04-contracts/integration-events.md rule 4) implements
/// <see cref="IIntegrationEvent"/>.
/// </summary>
public sealed partial class ContractsShapeTests
{
    public static TheoryData<string> ContractsAssemblies => new(SolutionAssemblies.ContractsAssemblies);

    [Theory]
    [MemberData(nameof(ContractsAssemblies))]
    [Trait("Req", "ADR-0005")]
    public void ADR_0005_every_public_contracts_type_is_a_sealed_record(string contracts)
    {
        var publicTypes = SolutionAssemblies.Load(contracts).GetExportedTypes();

        var violations = publicTypes.Where(t => !IsSealedRecord(t)).Select(t => t.FullName).ToList();

        violations.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(ContractsAssemblies))]
    [Trait("Req", "ADR-0005")]
    public void ADR_0005_every_integration_event_implements_the_integration_event_interface(string contracts)
    {
        var types = SolutionAssemblies.Load(contracts).GetTypes().Where(t => !IsCompilerGenerated(t));

        var violations = types.Where(IsMisdeclaredIntegrationEvent).Select(t => t.FullName).ToList();

        violations.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "ADR-0005")]
    public void ADR_0005_the_shape_rules_accept_a_sealed_record_event_and_reject_other_shapes()
    {
        var good = typeof(SampleIntegrationEvent);

        var openRecord = typeof(SampleOpenRecord);
        var plainClass = typeof(SampleClass);
        var recordStruct = typeof(SampleRecordStruct);
        var eventWithoutInterface = typeof(SampleWithoutInterfaceIntegrationEvent);
        var versionedEventWithoutInterface = typeof(SampleWithoutInterfaceIntegrationEventV2);
        var derivedRecord = typeof(SampleDerivedRecord);

        IsSealedRecord(good).ShouldBeTrue();
        IsMisdeclaredIntegrationEvent(good).ShouldBeFalse();
        IsSealedRecord(openRecord).ShouldBeFalse();
        IsSealedRecord(plainClass).ShouldBeFalse();
        IsSealedRecord(recordStruct).ShouldBeFalse();
        IsMisdeclaredIntegrationEvent(eventWithoutInterface).ShouldBeTrue();
        IsMisdeclaredIntegrationEvent(versionedEventWithoutInterface).ShouldBeTrue();
        IsSealedRecord(derivedRecord).ShouldBeTrue();
    }

    /// <summary>A sealed record class: the compiler emits a <c>&lt;Clone&gt;$</c> method only for record classes.</summary>
    internal static bool IsSealedRecord(Type type) =>
        type.IsClass
        && type.IsSealed
        && type.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance) is not null;

    internal static bool IsMisdeclaredIntegrationEvent(Type type) =>
        IntegrationEventName().IsMatch(type.Name)
        && !typeof(IIntegrationEvent).IsAssignableFrom(type);

    [GeneratedRegex(@"IntegrationEvent(V\d+)?$", RegexOptions.CultureInvariant)]
    private static partial Regex IntegrationEventName();

    private static bool IsCompilerGenerated(Type type) =>
        type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false) || type.Name.StartsWith('<');

    private sealed record SampleIntegrationEvent(Guid MessageId, DateTimeOffset OccurredAt) : IIntegrationEvent;

    private record SampleOpenRecord(int Value);

    private sealed record SampleDerivedRecord(int Value) : SampleOpenRecord(Value);

    private sealed class SampleClass;

    private readonly record struct SampleRecordStruct(int Value);

    private sealed record SampleWithoutInterfaceIntegrationEvent(Guid MessageId);

    private sealed record SampleWithoutInterfaceIntegrationEventV2(Guid MessageId);
}
