using FulfillmentFlow.SharedKernel;

namespace FulfillmentFlow.SharedKernel.Tests;

public sealed class AggregateRootTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_new_aggregate_has_no_domain_events_and_version_zero()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        var events = aggregate.DomainEvents;

        events.ShouldBeEmpty();
        aggregate.Version.ShouldBe(0);
    }

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_raised_domain_events_are_recorded_in_order()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        aggregate.DoSomething(Now);
        aggregate.DoSomething(Now.AddMinutes(1));

        aggregate.DomainEvents.Count.ShouldBe(2);
        aggregate.DomainEvents[0].OccurredAt.ShouldBe(Now);
        aggregate.DomainEvents[1].OccurredAt.ShouldBe(Now.AddMinutes(1));
    }

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_clearing_domain_events_empties_the_list()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.DoSomething(Now);

        aggregate.ClearDomainEvents();

        aggregate.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_domain_events_cannot_be_modified_from_outside()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.DoSomething(Now);

        var events = aggregate.DomainEvents;

        events.ShouldNotBeAssignableTo<List<IDomainEvent>>();
        events.ShouldNotBeAssignableTo<ICollection<IDomainEvent>>();
    }

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_version_increments_through_the_internal_hook()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        aggregate.IncrementVersion();
        aggregate.IncrementVersion();

        aggregate.Version.ShouldBe(2);
    }

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_aggregates_use_identity_equality()
    {
        var id = Guid.NewGuid();

        var first = new TestAggregate(id);
        var second = new TestAggregate(id);
        second.DoSomething(Now);

        first.ShouldBe(second);
    }

    private sealed record SomethingHappened(DateTimeOffset OccurredAt) : IDomainEvent;

    private sealed class TestAggregate(Guid id) : AggregateRoot<Guid>(id)
    {
        public void DoSomething(DateTimeOffset now) => Raise(new SomethingHappened(now));
    }
}
