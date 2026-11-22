namespace FulfillmentFlow.ArchitectureTests;

/// <summary>NFR-1: a context's Domain depends only on the shared kernel and the BCL.</summary>
public sealed class DomainPurityTests
{
    private static readonly string[] ForbiddenFrameworks =
        [.. SolutionAssemblies.EntityFrameworkCore, .. SolutionAssemblies.AspNetCore, "System.Text.Json"];

    public static TheoryData<string> DomainAssemblies => new(SolutionAssemblies.DomainAssemblies);

    [Theory]
    [MemberData(nameof(DomainAssemblies))]
    [Trait("Req", "NFR-1")]
    public void NFR_1_domain_types_depend_on_no_solution_namespace_except_the_shared_kernel(string domain)
    {
        var forbidden = SolutionAssemblies.All.Where(n => n != SolutionAssemblies.SharedKernel && n != domain);

        var offenders = SolutionAssemblies.TypesDependingOn(domain, forbidden);

        offenders.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(DomainAssemblies))]
    [Trait("Req", "NFR-1")]
    public void NFR_1_domain_assemblies_reference_no_solution_assembly_except_the_shared_kernel(string domain)
    {
        var references = SolutionAssemblies.SolutionReferencesOf(domain);

        references.Where(r => r != SolutionAssemblies.SharedKernel).ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(DomainAssemblies))]
    [Trait("Req", "NFR-1")]
    public void NFR_1_domain_types_do_not_depend_on_ef_core_asp_net_core_or_system_text_json(string domain)
    {
        var offenders = SolutionAssemblies.TypesDependingOn(domain, ForbiddenFrameworks);

        offenders.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(DomainAssemblies))]
    [Trait("Req", "NFR-1")]
    public void NFR_1_domain_assemblies_reference_no_ef_core_asp_net_core_or_system_text_json(string domain)
    {
        var references = SolutionAssemblies.ReferencesOf(domain);

        references.Where(r => SolutionAssemblies.MatchesAny(r, ForbiddenFrameworks)).ShouldBeEmpty();
    }
}
