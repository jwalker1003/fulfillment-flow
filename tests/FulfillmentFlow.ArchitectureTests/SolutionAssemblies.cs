using System.Reflection;
using NetArchTest.Rules;

namespace FulfillmentFlow.ArchitectureTests;

/// <summary>
/// Loads the compiled solution assemblies (this test project references every source project,
/// so each one is copied next to the test assembly) and offers small helpers shared by the
/// assembly-level rules (story P1-03). Loading by name means no marker type is needed in an
/// empty scaffold project.
/// </summary>
internal static class SolutionAssemblies
{
    public const string Prefix = "FulfillmentFlow";
    public const string SharedKernel = "FulfillmentFlow.SharedKernel";
    public const string BuildingBlocksContracts = "FulfillmentFlow.BuildingBlocks.Contracts";
    public const string BuildingBlocksApplication = "FulfillmentFlow.BuildingBlocks.Application";
    public const string BuildingBlocksInfrastructure = "FulfillmentFlow.BuildingBlocks.Infrastructure";
    public const string Catalog = "FulfillmentFlow.Catalog";
    public const string CatalogContracts = "FulfillmentFlow.Catalog.Contracts";
    public const string Reporting = "FulfillmentFlow.Reporting";
    public const string Dashboard = "FulfillmentFlow.Dashboard";
    public const string Api = "FulfillmentFlow.Api";

    public static readonly string[] InnerLayers = ["Domain", "Application", "Infrastructure"];

    public static readonly string[] EntityFrameworkCore = ["Microsoft.EntityFrameworkCore"];
    public static readonly string[] AspNetCore = ["Microsoft.AspNetCore"];

    public static string Layer(string context, string layer) => $"{Prefix}.{context}.{layer}";

    public static IReadOnlyList<string> DomainAssemblies =>
        [.. RepositoryLayout.BoundedContexts.Select(c => Layer(c, "Domain"))];

    public static IReadOnlyList<string> ApplicationAssemblies =>
        [.. RepositoryLayout.BoundedContexts.Select(c => Layer(c, "Application"))];

    public static IReadOnlyList<string> ContractsAssemblies =>
        [.. RepositoryLayout.BoundedContexts.Select(c => Layer(c, "Contracts")), CatalogContracts];

    /// <summary>Domain, Application and Infrastructure of every layered bounded context.</summary>
    public static IReadOnlyList<string> InnerLayerAssemblies =>
        [.. RepositoryLayout.BoundedContexts.SelectMany(c => InnerLayers.Select(l => Layer(c, l)))];

    /// <summary>Every assembly that belongs to a module (bounded context or Reporting).</summary>
    public static IReadOnlyList<string> ModuleAssemblies =>
        [.. InnerLayerAssemblies, .. ContractsAssemblies, Catalog, Reporting];

    /// <summary>Every production assembly under <c>src/</c>.</summary>
    public static IReadOnlyList<string> All =>
    [
        SharedKernel,
        BuildingBlocksContracts,
        BuildingBlocksApplication,
        BuildingBlocksInfrastructure,
        .. ModuleAssemblies,
        Dashboard,
        Api,
    ];

    public static Assembly Load(string name) => Assembly.Load(new AssemblyName(name));

    /// <summary>Names of the solution assemblies the compiled assembly actually references.</summary>
    public static IReadOnlyList<string> SolutionReferencesOf(string name) =>
        [.. Load(name).GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .Where(n => n == Prefix || n.StartsWith(Prefix + ".", StringComparison.Ordinal))];

    /// <summary>Names of every assembly the compiled assembly references.</summary>
    public static IReadOnlyList<string> ReferencesOf(string name) =>
        [.. Load(name).GetReferencedAssemblies().Select(a => a.Name ?? string.Empty)];

    /// <summary>
    /// NetArchTest: the full names of types in <paramref name="assemblyName"/> that depend on any of
    /// <paramref name="namespaces"/>. Empty means the rule holds.
    /// </summary>
    public static IReadOnlyList<string> TypesDependingOn(string assemblyName, IEnumerable<string> namespaces)
    {
        var forbidden = namespaces.ToArray();
        if (forbidden.Length == 0)
        {
            return [];
        }

        var result = Types.InAssembly(Load(assemblyName))
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        return result.IsSuccessful ? [] : [.. result.FailingTypeNames ?? []];
    }

    /// <summary>True when <paramref name="reference"/> starts with one of the package/assembly prefixes (on a dot boundary).</summary>
    public static bool MatchesAny(string reference, IEnumerable<string> prefixes) =>
        prefixes.Any(p => reference.Equals(p, StringComparison.OrdinalIgnoreCase)
                          || reference.StartsWith(p + ".", StringComparison.OrdinalIgnoreCase));
}
