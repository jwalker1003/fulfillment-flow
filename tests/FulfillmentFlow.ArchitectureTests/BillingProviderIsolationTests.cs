namespace FulfillmentFlow.ArchitectureTests;

/// <summary>
/// BIL-2.1 (rule only): the FakePay anti-corruption layer lives in Billing.Infrastructure; no
/// provider type may appear in Billing's Domain or Application layers.
/// </summary>
public sealed class BillingProviderIsolationTests
{
    private const string ProviderMarker = ".FakePay";
    private const string KnownProviderNamespace = "FulfillmentFlow.Billing.Infrastructure.FakePay";

    public static TheoryData<string> BillingInnerLayers => new(
        SolutionAssemblies.Layer("Billing", "Domain"),
        SolutionAssemblies.Layer("Billing", "Application"));

    [Theory]
    [MemberData(nameof(BillingInnerLayers))]
    [Trait("Req", "BIL-2.1")]
    public void BIL_2_1_billing_domain_and_application_types_do_not_depend_on_a_fakepay_namespace(string assembly)
    {
        var providerNamespaces = ProviderNamespacesVisibleTo(assembly);

        var offenders = SolutionAssemblies.TypesDependingOn(assembly, providerNamespaces);

        offenders.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(BillingInnerLayers))]
    [Trait("Req", "BIL-2.1")]
    public void BIL_2_1_billing_domain_and_application_reference_no_fakepay_assembly(string assembly)
    {
        var references = SolutionAssemblies.ReferencesOf(assembly);

        references.Where(r => r.Contains("FakePay", StringComparison.OrdinalIgnoreCase)).ShouldBeEmpty();
    }

    /// <summary>
    /// Every namespace containing <c>.FakePay</c> declared in the solution or in any assembly the layer
    /// references, plus the namespace the architecture reserves for the ACL.
    /// </summary>
    private static string[] ProviderNamespacesVisibleTo(string assembly)
    {
        var candidates = SolutionAssemblies.All
            .Concat(SolutionAssemblies.ReferencesOf(assembly))
            .Distinct(StringComparer.Ordinal)
            .SelectMany(TypesOrNone);

        return
        [
            .. candidates
                .Select(t => t.Namespace)
                .OfType<string>()
                .Where(ns => ns.Contains(ProviderMarker, StringComparison.Ordinal))
                .Append(KnownProviderNamespace)
                .Distinct(StringComparer.Ordinal),
        ];
    }

    private static IEnumerable<Type> TypesOrNone(string assemblyName)
    {
        try
        {
            return SolutionAssemblies.Load(assemblyName).GetTypes();
        }
        catch (System.Reflection.ReflectionTypeLoadException ex)
        {
            // Keep every type that did load so a partial load cannot hide a FakePay namespace.
            return ex.Types.OfType<Type>();
        }
        catch (Exception ex) when (ex is FileNotFoundException or BadImageFormatException)
        {
            return [];
        }
    }
}
