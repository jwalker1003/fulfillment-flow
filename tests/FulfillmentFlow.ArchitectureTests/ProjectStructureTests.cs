using System.Xml.Linq;

namespace FulfillmentFlow.ArchitectureTests;

/// <summary>
/// Structural rules for the solution scaffold (story P1-01): the layout and the allowed
/// project references from docs/03-architecture/architecture.md, and the build settings
/// from ADR-0013 / ADR-0020. Assembly-level dependency rules live in the NetArchTest suite (P1-03).
/// </summary>
public sealed class ProjectStructureTests
{
    private const string SharedKernel = "FulfillmentFlow.SharedKernel";
    private const string BuildingBlocksContracts = "FulfillmentFlow.BuildingBlocks.Contracts";
    private const string BuildingBlocksApplication = "FulfillmentFlow.BuildingBlocks.Application";
    private const string BuildingBlocksInfrastructure = "FulfillmentFlow.BuildingBlocks.Infrastructure";

    private static readonly string[] AllContractsProjects =
    [
        .. RepositoryLayout.BoundedContexts.Select(c => $"FulfillmentFlow.{c}.Contracts"),
        "FulfillmentFlow.Catalog.Contracts",
    ];

    public static TheoryData<string> BoundedContexts => new(RepositoryLayout.BoundedContexts);

    public static TheoryData<string> ContractsProjects => new(AllContractsProjects);

    [Fact]
    [Trait("Req", "NFR-1")]
    public void NFR_1_solution_contains_every_project_in_the_architecture_layout()
    {
        var solution = File.ReadAllText(RepositoryLayout.PathOf(RepositoryLayout.SolutionFileName));

        var missingOnDisk = RepositoryLayout.ExpectedProjects.Where(p => !File.Exists(RepositoryLayout.PathOf(p))).ToList();
        var missingInSolution = RepositoryLayout.ExpectedProjects
            .Where(p => !solution.Contains(p.Replace('/', '\\'), StringComparison.OrdinalIgnoreCase))
            .ToList();

        missingOnDisk.ShouldBeEmpty();
        missingInSolution.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(BoundedContexts))]
    [Trait("Req", "NFR-1")]
    public void NFR_1_domain_projects_reference_only_the_shared_kernel(string context)
    {
        var project = RepositoryLayout.SourceProject($"FulfillmentFlow.{context}.Domain");

        var projectReferences = RepositoryLayout.ProjectReferencesOf(project);
        var packageReferences = RepositoryLayout.PackageReferencesOf(RepositoryLayout.PathOf(project));

        projectReferences.ShouldBe([SharedKernel]);
        packageReferences.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(SharedKernel)]
    [InlineData(BuildingBlocksContracts)]
    [Trait("Req", "NFR-1")]
    public void NFR_1_shared_kernel_and_building_blocks_contracts_reference_nothing(string projectName)
    {
        var project = RepositoryLayout.SourceProject(projectName);

        var projectReferences = RepositoryLayout.ProjectReferencesOf(project);
        var packageReferences = RepositoryLayout.PackageReferencesOf(RepositoryLayout.PathOf(project));

        projectReferences.ShouldBeEmpty();
        packageReferences.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(ContractsProjects))]
    [Trait("Req", "NFR-1")]
    public void NFR_1_contracts_projects_reference_only_building_blocks_contracts(string projectName)
    {
        var project = RepositoryLayout.SourceProject(projectName);

        var projectReferences = RepositoryLayout.ProjectReferencesOf(project);
        var packageReferences = RepositoryLayout.PackageReferencesOf(RepositoryLayout.PathOf(project));

        projectReferences.ShouldBe([BuildingBlocksContracts]);
        packageReferences.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "NFR-1")]
    public void NFR_1_building_blocks_application_references_only_shared_kernel_and_building_blocks_contracts()
    {
        var project = RepositoryLayout.SourceProject(BuildingBlocksApplication);

        var projectReferences = RepositoryLayout.ProjectReferencesOf(project);

        projectReferences.ShouldBe([SharedKernel, BuildingBlocksContracts], ignoreOrder: true);
    }

    [Theory]
    [MemberData(nameof(BoundedContexts))]
    [Trait("Req", "NFR-1")]
    public void NFR_1_application_projects_reference_only_own_domain_contracts_and_building_blocks(string context)
    {
        var project = RepositoryLayout.SourceProject($"FulfillmentFlow.{context}.Application");
        string[] allowed =
        [
            $"FulfillmentFlow.{context}.Domain",
            BuildingBlocksApplication,
            .. AllContractsProjects,
        ];

        var projectReferences = RepositoryLayout.ProjectReferencesOf(project);

        projectReferences.ShouldContain($"FulfillmentFlow.{context}.Domain");
        projectReferences.ShouldContain($"FulfillmentFlow.{context}.Contracts");
        projectReferences.ShouldContain(BuildingBlocksApplication);
        projectReferences.Except(allowed).ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(BoundedContexts))]
    [Trait("Req", "NFR-1")]
    public void NFR_1_infrastructure_projects_reference_only_own_application_and_building_blocks_infrastructure(string context)
    {
        var project = RepositoryLayout.SourceProject($"FulfillmentFlow.{context}.Infrastructure");

        var projectReferences = RepositoryLayout.ProjectReferencesOf(project);

        projectReferences.ShouldBe([$"FulfillmentFlow.{context}.Application", BuildingBlocksInfrastructure], ignoreOrder: true);
    }

    [Fact]
    [Trait("Req", "NFR-1")]
    public void NFR_1_reporting_references_only_contracts_and_building_blocks()
    {
        var project = RepositoryLayout.SourceProject("FulfillmentFlow.Reporting");
        string[] allowed = [.. AllContractsProjects, BuildingBlocksContracts, BuildingBlocksApplication, BuildingBlocksInfrastructure];

        var projectReferences = RepositoryLayout.ProjectReferencesOf(project);

        projectReferences.Except(allowed).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "NFR-1")]
    public void NFR_1_dashboard_references_no_solution_project()
    {
        var project = RepositoryLayout.SourceProject("FulfillmentFlow.Dashboard");

        var projectReferences = RepositoryLayout.ProjectReferencesOf(project);

        projectReferences.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "NFR-1")]
    public void NFR_1_api_host_composes_every_module_and_the_dashboard()
    {
        var project = RepositoryLayout.SourceProject("FulfillmentFlow.Api");
        string[] required =
        [
            .. RepositoryLayout.BoundedContexts.Select(c => $"FulfillmentFlow.{c}.Infrastructure"),
            "FulfillmentFlow.Catalog",
            "FulfillmentFlow.Reporting",
            "FulfillmentFlow.Dashboard",
        ];

        var projectReferences = RepositoryLayout.ProjectReferencesOf(project);

        required.Except(projectReferences).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "ADR-0013")]
    public void ADR_0013_package_versions_are_managed_centrally()
    {
        var packagesProps = XDocument.Load(RepositoryLayout.PathOf("Directory.Packages.props"));

        var centralManagement = packagesProps.Descendants("ManagePackageVersionsCentrally").Single().Value;
        var projectsDeclaringVersions = RepositoryLayout.AllProjectFiles()
            .Where(file => XDocument.Load(file).Descendants("PackageReference")
                .Any(e => e.Attribute("Version") is not null
                          || e.Attribute("VersionOverride") is not null
                          || e.Element("Version") is not null))
            .Select(file => Path.GetRelativePath(RepositoryLayout.Root, file))
            .ToList();

        centralManagement.ShouldBe("true");
        projectsDeclaringVersions.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "ADR-0013")]
    public void ADR_0013_all_projects_build_net9_with_strict_settings_from_directory_build_props()
    {
        var properties = XDocument.Load(RepositoryLayout.PathOf("Directory.Build.props"))
            .Descendants("PropertyGroup").Elements()
            .ToDictionary(e => e.Name.LocalName, e => e.Value);

        var projectsOverridingFramework = RepositoryLayout.AllProjectFiles()
            .Where(file => XDocument.Load(file).Descendants()
                .Any(e => e.Name.LocalName is "TargetFramework" or "TargetFrameworks"))
            .Select(file => Path.GetRelativePath(RepositoryLayout.Root, file))
            .ToList();

        properties["TargetFramework"].ShouldBe("net9.0");
        properties["Nullable"].ShouldBe("enable");
        properties["ImplicitUsings"].ShouldBe("enable");
        properties["TreatWarningsAsErrors"].ShouldBe("true");
        properties["LangVersion"].ShouldBe("latest");
        properties["AnalysisLevel"].ShouldBe("latest-recommended");
        projectsOverridingFramework.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Req", "ADR-0020")]
    public void ADR_0020_sqlite_provider_is_referenced_by_building_blocks_infrastructure_and_never_by_inner_layers()
    {
        var buildingBlocksInfrastructure = RepositoryLayout.PathOf(RepositoryLayout.SourceProject(BuildingBlocksInfrastructure));
        var innerLayerProjects = RepositoryLayout.ExpectedProjects
            .Where(p => p.StartsWith("src/", StringComparison.Ordinal))
            .Where(p => IsInnerLayer(Path.GetFileNameWithoutExtension(p)))
            .ToList();

        var innerLayerProjectsUsingEfCore = innerLayerProjects
            .Where(p => RepositoryLayout.PackageReferencesOf(RepositoryLayout.PathOf(p))
                .Any(package => package.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal)))
            .ToList();

        RepositoryLayout.PackageReferencesOf(buildingBlocksInfrastructure).ShouldContain("Microsoft.EntityFrameworkCore.Sqlite");
        innerLayerProjectsUsingEfCore.ShouldBeEmpty();
    }

    private static bool IsInnerLayer(string projectName) =>
        projectName == SharedKernel
        || projectName.EndsWith(".Domain", StringComparison.Ordinal)
        || projectName.EndsWith(".Application", StringComparison.Ordinal)
        || projectName.EndsWith(".Contracts", StringComparison.Ordinal);
}
