namespace FulfillmentFlow.ArchitectureTests;

/// <summary>
/// NFR-2: contexts never reach into each other's Domain, Application or Infrastructure; they share
/// only <c>*.Contracts</c>. Catalog is a single project, so it counts as an inner layer of its own.
/// </summary>
public sealed class ContextIsolationTests
{
    public static TheoryData<string, string> InnerLayersOfEachContext
    {
        get
        {
            var data = new TheoryData<string, string>();
            foreach (var context in RepositoryLayout.BoundedContexts)
            {
                foreach (var layer in SolutionAssemblies.InnerLayers)
                {
                    data.Add(context, SolutionAssemblies.Layer(context, layer));
                }
            }

            return data;
        }
    }

    public static TheoryData<string> ContractsAssemblies => new(SolutionAssemblies.ContractsAssemblies);

    [Theory]
    [MemberData(nameof(InnerLayersOfEachContext))]
    [Trait("Req", "NFR-2")]
    public void NFR_2_context_types_do_not_depend_on_another_contexts_domain_application_or_infrastructure(string context, string assembly)
    {
        var offenders = SolutionAssemblies.TypesDependingOn(assembly, InnerLayersOfOtherContexts(context));

        offenders.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(InnerLayersOfEachContext))]
    [Trait("Req", "NFR-2")]
    public void NFR_2_context_assemblies_reference_no_other_contexts_domain_application_infrastructure_or_catalog(string context, string assembly)
    {
        string[] forbidden = [.. InnerLayersOfOtherContexts(context), SolutionAssemblies.Catalog];

        var references = SolutionAssemblies.SolutionReferencesOf(assembly);

        references.Intersect(forbidden).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "NFR-2")]
    public void NFR_2_catalog_does_not_depend_on_any_contexts_domain_application_or_infrastructure()
    {
        var forbidden = SolutionAssemblies.InnerLayerAssemblies;

        var offenders = SolutionAssemblies.TypesDependingOn(SolutionAssemblies.Catalog, forbidden);
        var references = SolutionAssemblies.SolutionReferencesOf(SolutionAssemblies.Catalog);

        offenders.ShouldBeEmpty();
        references.Intersect(forbidden).ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(ContractsAssemblies))]
    [Trait("Req", "NFR-2")]
    public void NFR_2_contracts_types_depend_only_on_building_blocks_contracts(string contracts)
    {
        // A namespace that encloses this assembly's own (FulfillmentFlow.Catalog for Catalog.Contracts)
        // cannot be expressed as a NetArchTest prefix; the assembly-reference rule below covers it.
        var forbidden = SolutionAssemblies.All.Where(n =>
            n != SolutionAssemblies.BuildingBlocksContracts
            && n != contracts
            && !contracts.StartsWith(n + ".", StringComparison.Ordinal));

        var offenders = SolutionAssemblies.TypesDependingOn(contracts, forbidden);

        offenders.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(ContractsAssemblies))]
    [Trait("Req", "NFR-2")]
    public void NFR_2_contracts_assemblies_reference_only_building_blocks_contracts(string contracts)
    {
        var references = SolutionAssemblies.SolutionReferencesOf(contracts);

        references.Where(r => r != SolutionAssemblies.BuildingBlocksContracts).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "NFR-2")]
    public void NFR_2_reporting_depends_on_no_domain_application_or_infrastructure()
    {
        var offenders = SolutionAssemblies.TypesDependingOn(SolutionAssemblies.Reporting, SolutionAssemblies.InnerLayerAssemblies);
        var references = SolutionAssemblies.SolutionReferencesOf(SolutionAssemblies.Reporting);

        offenders.ShouldBeEmpty();
        references.Intersect([.. SolutionAssemblies.InnerLayerAssemblies, SolutionAssemblies.Catalog]).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "NFR-2")]
    public void NFR_2_dashboard_depends_on_no_module()
    {
        var offenders = SolutionAssemblies.TypesDependingOn(SolutionAssemblies.Dashboard, SolutionAssemblies.ModuleAssemblies);
        var references = SolutionAssemblies.SolutionReferencesOf(SolutionAssemblies.Dashboard);

        offenders.ShouldBeEmpty();
        references.Intersect(SolutionAssemblies.ModuleAssemblies).ShouldBeEmpty();
    }

    private static string[] InnerLayersOfOtherContexts(string context) =>
    [
        .. RepositoryLayout.BoundedContexts
            .Where(other => other != context)
            .SelectMany(other => SolutionAssemblies.InnerLayers.Select(layer => SolutionAssemblies.Layer(other, layer))),
    ];
}
