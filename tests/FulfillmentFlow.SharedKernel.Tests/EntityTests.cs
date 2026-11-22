using FulfillmentFlow.SharedKernel;

namespace FulfillmentFlow.SharedKernel.Tests;

public sealed class EntityTests
{
    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_entities_with_the_same_id_are_equal()
    {
        var id = Guid.NewGuid();

        var first = new TestEntity(id, "first");
        var second = new TestEntity(id, "second");

        first.Equals(second).ShouldBeTrue();
        (first == second).ShouldBeTrue();
        (first != second).ShouldBeFalse();
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_entities_with_different_ids_are_not_equal()
    {
        var first = new TestEntity(Guid.NewGuid(), "same");
        var second = new TestEntity(Guid.NewGuid(), "same");

        var equal = first.Equals(second);

        equal.ShouldBeFalse();
        (first == second).ShouldBeFalse();
        (first != second).ShouldBeTrue();
    }

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_entities_of_different_types_with_the_same_id_are_not_equal()
    {
        var id = Guid.NewGuid();

        var entity = new TestEntity(id, "x");
        var other = new OtherTestEntity(id);

        entity.Equals(other).ShouldBeFalse();
    }

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_entity_is_not_equal_to_null()
    {
        var entity = new TestEntity(Guid.NewGuid(), "x");

        var equal = entity.Equals(null);

        equal.ShouldBeFalse();
        (entity == null).ShouldBeFalse();
        (null == entity).ShouldBeFalse();
    }

    private sealed class TestEntity(Guid id, string name) : Entity<Guid>(id)
    {
        public string Name { get; } = name;
    }

    private sealed class OtherTestEntity(Guid id) : Entity<Guid>(id);
}
