using System.Xml.Linq;

namespace FulfillmentFlow.ArchitectureTests;

/// <summary>
/// Reads the solution and project files from disk so structural rules can be checked
/// without loading any assembly.
/// </summary>
internal static class RepositoryLayout
{
    public const string SolutionFileName = "FulfillmentFlow.sln";

    public static readonly string[] BoundedContexts = ["Inventory", "Ordering", "Fulfillment", "Billing"];

    /// <summary>Every project from the *Solution layout* section of docs/03-architecture/architecture.md.</summary>
    public static readonly string[] ExpectedProjects =
    [
        "src/BuildingBlocks/FulfillmentFlow.BuildingBlocks.Contracts/FulfillmentFlow.BuildingBlocks.Contracts.csproj",
        "src/BuildingBlocks/FulfillmentFlow.SharedKernel/FulfillmentFlow.SharedKernel.csproj",
        "src/BuildingBlocks/FulfillmentFlow.BuildingBlocks.Application/FulfillmentFlow.BuildingBlocks.Application.csproj",
        "src/BuildingBlocks/FulfillmentFlow.BuildingBlocks.Infrastructure/FulfillmentFlow.BuildingBlocks.Infrastructure.csproj",
        .. BoundedContexts.SelectMany(context => new[] { "Domain", "Application", "Infrastructure", "Contracts" }
            .Select(layer => $"src/Modules/{context}/FulfillmentFlow.{context}.{layer}/FulfillmentFlow.{context}.{layer}.csproj")),
        "src/Modules/Catalog/FulfillmentFlow.Catalog/FulfillmentFlow.Catalog.csproj",
        "src/Modules/Catalog/FulfillmentFlow.Catalog.Contracts/FulfillmentFlow.Catalog.Contracts.csproj",
        "src/Modules/Reporting/FulfillmentFlow.Reporting/FulfillmentFlow.Reporting.csproj",
        "src/FulfillmentFlow.Api/FulfillmentFlow.Api.csproj",
        "src/FulfillmentFlow.Dashboard/FulfillmentFlow.Dashboard.csproj",
        "tests/FulfillmentFlow.Inventory.Domain.Tests/FulfillmentFlow.Inventory.Domain.Tests.csproj",
        "tests/FulfillmentFlow.Ordering.Domain.Tests/FulfillmentFlow.Ordering.Domain.Tests.csproj",
        "tests/FulfillmentFlow.Fulfillment.Domain.Tests/FulfillmentFlow.Fulfillment.Domain.Tests.csproj",
        "tests/FulfillmentFlow.Billing.Tests/FulfillmentFlow.Billing.Tests.csproj",
        "tests/FulfillmentFlow.Application.Tests/FulfillmentFlow.Application.Tests.csproj",
        "tests/FulfillmentFlow.IntegrationTests/FulfillmentFlow.IntegrationTests.csproj",
        "tests/FulfillmentFlow.ArchitectureTests/FulfillmentFlow.ArchitectureTests.csproj",

        // Listed in docs/06-quality/test-strategy.md (Domain layer); added by P1-02.
        "tests/FulfillmentFlow.SharedKernel.Tests/FulfillmentFlow.SharedKernel.Tests.csproj",
    ];

    /// <summary>The Domain layer test projects from docs/06-quality/test-strategy.md (NFR-6).</summary>
    public static readonly string[] DomainTestProjects =
    [
        .. new[] { "Inventory", "Ordering", "Fulfillment" }
            .Select(context => $"tests/FulfillmentFlow.{context}.Domain.Tests/FulfillmentFlow.{context}.Domain.Tests.csproj"),
        "tests/FulfillmentFlow.Billing.Tests/FulfillmentFlow.Billing.Tests.csproj",
        "tests/FulfillmentFlow.SharedKernel.Tests/FulfillmentFlow.SharedKernel.Tests.csproj",
    ];

    private static readonly Lazy<string> LazyRoot = new(FindRoot);

    public static string Root => LazyRoot.Value;

    public static string PathOf(string relativePath) =>
        Path.Combine(Root, relativePath.Replace('/', Path.DirectorySeparatorChar));

    public static IReadOnlyList<string> AllProjectFiles() =>
        Directory.EnumerateFiles(Path.Combine(Root, "src"), "*.csproj", SearchOption.AllDirectories)
            .Concat(Directory.EnumerateFiles(Path.Combine(Root, "tests"), "*.csproj", SearchOption.AllDirectories))
            .ToList();

    public static string SourceProject(string name) =>
        ExpectedProjects.Single(p => p.StartsWith("src/", StringComparison.Ordinal)
                                     && Path.GetFileNameWithoutExtension(p) == name);

    /// <summary>Names (without extension) of the projects referenced by a project file.</summary>
    public static IReadOnlyList<string> ProjectReferencesOf(string relativeProjectPath) =>
        XDocument.Load(PathOf(relativeProjectPath))
            .Descendants("ProjectReference")
            .Select(e => (string?)e.Attribute("Include") ?? string.Empty)
            .Select(include => Path.GetFileNameWithoutExtension(include.Replace('\\', '/')))
            .ToList();

    /// <summary>Package ids referenced directly by a project file.</summary>
    public static IReadOnlyList<string> PackageReferencesOf(string projectFile) =>
        XDocument.Load(projectFile)
            .Descendants("PackageReference")
            .Select(e => (string?)e.Attribute("Include") ?? string.Empty)
            .ToList();

    /// <summary>Shared frameworks referenced directly by a project file (e.g. Microsoft.AspNetCore.App).</summary>
    public static IReadOnlyList<string> FrameworkReferencesOf(string projectFile) =>
        XDocument.Load(projectFile)
            .Descendants("FrameworkReference")
            .Select(e => (string?)e.Attribute("Include") ?? string.Empty)
            .ToList();

    /// <summary>The MSBuild SDK declared on the root Project element.</summary>
    public static string SdkOf(string projectFile) =>
        (string?)XDocument.Load(projectFile).Root?.Attribute("Sdk") ?? string.Empty;

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, SolutionFileName)))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
               ?? throw new InvalidOperationException($"Could not find {SolutionFileName} above {AppContext.BaseDirectory}.");
    }
}
