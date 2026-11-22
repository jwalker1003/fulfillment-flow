using System.Reflection;
using FulfillmentFlow.SharedKernel;

namespace FulfillmentFlow.SharedKernel.Tests;

public sealed class SharedKernelSurfaceTests
{
    private static readonly Assembly SharedKernelAssembly = typeof(Result).Assembly;

    [Fact]
    [Trait("Req", "NFR-8")]
    public void NFR_8_shared_kernel_has_no_public_setters()
    {
        var types = SharedKernelAssembly.GetTypes();

        var publicSetters = types
            .SelectMany(t => t.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Where(p => p.SetMethod is { IsPublic: true })
            .Select(p => $"{p.DeclaringType!.Name}.{p.Name}")
            .ToList();

        publicSetters.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_shared_kernel_exposes_only_the_agreed_primitives()
    {
        string[] agreed =
        [
            "AggregateRoot`1", "DomainError", "Entity`1", "GeoPoint", "IDomainEvent",
            "Money", "Quantity", "Result", "Result`1", "SharedErrors", "Sku",
        ];

        var exported = SharedKernelAssembly.GetExportedTypes().Select(t => t.Name).Order(StringComparer.Ordinal).ToList();

        exported.ShouldBe(agreed.Order(StringComparer.Ordinal).ToList());
    }

    [Fact]
    [Trait("Req", "ADR-0004")]
    public void ADR_0004_shared_kernel_references_only_the_base_class_library()
    {
        var referenced = SharedKernelAssembly.GetReferencedAssemblies().Select(a => a.Name!).ToList();

        var nonFramework = referenced
            .Where(name => !name.StartsWith("System", StringComparison.Ordinal) && name != "netstandard" && name != "mscorlib")
            .ToList();

        nonFramework.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "NFR-7")]
    public void NFR_7_value_objects_have_no_public_constructor_that_takes_arguments()
    {
        Type[] valueObjects = [typeof(Sku), typeof(Quantity), typeof(Money), typeof(GeoPoint)];

        var publicConstructors = valueObjects
            .SelectMany(t => t.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
            .Where(c => c.GetParameters().Length > 0)
            .Select(c => $"{c.DeclaringType!.Name}({string.Join(", ", c.GetParameters().Select(p => p.ParameterType.Name))})")
            .ToList();

        publicConstructors.ShouldBeEmpty();
    }
}
