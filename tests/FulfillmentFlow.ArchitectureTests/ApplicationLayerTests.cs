namespace FulfillmentFlow.ArchitectureTests;

/// <summary>
/// The Application layer orchestrates the domain and stays free of persistence and web frameworks
/// (docs/03-architecture/architecture.md, *Allowed project references*; ADR-0001).
/// </summary>
public sealed class ApplicationLayerTests
{
    private static readonly string[] ForbiddenFrameworks =
        [.. SolutionAssemblies.EntityFrameworkCore, .. SolutionAssemblies.AspNetCore];

    public static TheoryData<string> ApplicationAssemblies => new(SolutionAssemblies.ApplicationAssemblies);

    [Theory]
    [MemberData(nameof(ApplicationAssemblies))]
    [Trait("Req", "ADR-0001")]
    public void ADR_0001_application_types_do_not_depend_on_ef_core_or_asp_net_core(string application)
    {
        var offenders = SolutionAssemblies.TypesDependingOn(application, ForbiddenFrameworks);

        offenders.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(ApplicationAssemblies))]
    [Trait("Req", "ADR-0001")]
    public void ADR_0001_application_assemblies_reference_no_ef_core_or_asp_net_core(string application)
    {
        var references = SolutionAssemblies.ReferencesOf(application);

        references.Where(r => SolutionAssemblies.MatchesAny(r, ForbiddenFrameworks)).ShouldBeEmpty();
    }
}
