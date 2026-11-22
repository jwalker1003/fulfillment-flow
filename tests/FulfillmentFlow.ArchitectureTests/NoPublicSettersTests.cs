using System.Reflection;
using FulfillmentFlow.SharedKernel;

namespace FulfillmentFlow.ArchitectureTests;

/// <summary>
/// NFR-8: entities and aggregates change state only through methods. A public or internal
/// <c>set</c> or <c>init</c> accessor on any of their properties breaks the rule.
/// </summary>
public sealed class NoPublicSettersTests
{
    [Fact]
    [Trait("Req", "NFR-8")]
    public void NFR_8_entities_and_aggregates_have_no_public_or_internal_setters()
    {
        var entityTypes = SolutionAssemblies.All
            .Select(SolutionAssemblies.Load)
            .SelectMany(LoadableTypes)
            .Where(IsEntityOrAggregate);

        var violations = entityTypes.SelectMany(SetterViolations).ToList();

        violations.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "NFR-8")]
    public void NFR_8_the_setter_rule_catches_public_internal_and_init_setters_but_allows_private_ones()
    {
        var type = typeof(SampleAggregateWithSetters);

        var violations = SetterViolations(type);

        IsEntityOrAggregate(type).ShouldBeTrue();
        violations.ShouldBe(
            [$"{type.FullName}.{nameof(SampleAggregateWithSetters.PublicSet)}",
             $"{type.FullName}.{nameof(SampleAggregateWithSetters.InternalSet)}",
             $"{type.FullName}.{nameof(SampleAggregateWithSetters.PublicInit)}"],
            ignoreOrder: true);
    }

    internal static bool IsEntityOrAggregate(Type type)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (current.IsGenericType
                && (current.GetGenericTypeDefinition() == typeof(Entity<>)
                    || current.GetGenericTypeDefinition() == typeof(AggregateRoot<>)))
            {
                return true;
            }
        }

        return false;
    }

    internal static IReadOnlyList<string> SetterViolations(Type type)
    {
        const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        return
        [
            .. type.GetProperties(all)
                .Where(p => p.SetMethod is { } setter && (setter.IsPublic || setter.IsAssembly || setter.IsFamilyOrAssembly))
                .Select(p => $"{p.DeclaringType?.FullName}.{p.Name}"),
        ];
    }

    private static IEnumerable<Type> LoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.OfType<Type>();
        }
    }

    private sealed class SampleAggregateWithSetters : AggregateRoot<Guid>
    {
        public SampleAggregateWithSetters()
            : base(Guid.Empty)
        {
        }

        public int PublicSet { get; set; }

        public int InternalSet { get; internal set; }

        public int PublicInit { get; init; }

        public int PrivateSet { get; private set; }

        public void Touch() => PrivateSet++;
    }
}
