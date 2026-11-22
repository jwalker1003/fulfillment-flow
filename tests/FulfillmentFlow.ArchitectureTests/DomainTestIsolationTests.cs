using System.Xml.Linq;

namespace FulfillmentFlow.ArchitectureTests;

/// <summary>
/// NFR-6: domain test projects need no infrastructure. Neither the test project nor anything it
/// references (followed through project references) may pull in EF Core, SQLite, ASP.NET Core or a
/// mocking library. Checked from the project files so the rule does not depend on build order.
/// </summary>
public sealed class DomainTestIsolationTests
{
    private static readonly string[] ForbiddenPackages =
    [
        "Microsoft.EntityFrameworkCore",
        "Microsoft.AspNetCore",
        "Microsoft.Data.Sqlite",
        "SQLitePCLRaw",
        "Moq",
        "NSubstitute",
        "FakeItEasy",
    ];

    private static readonly string[] ForbiddenFrameworks = ["Microsoft.AspNetCore.App"];

    private static readonly string[] ForbiddenSdks = ["Microsoft.NET.Sdk.Web", "Microsoft.NET.Sdk.Razor"];

    public static TheoryData<string> DomainTestProjects => new(RepositoryLayout.DomainTestProjects);

    [Fact]
    [Trait("Req", "NFR-6")]
    public void NFR_6_every_domain_test_project_on_disk_is_covered_by_the_rule()
    {
        var onDisk = Directory.EnumerateFiles(RepositoryLayout.PathOf("tests"), "*.Domain.Tests.csproj", SearchOption.AllDirectories)
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();

        var covered = RepositoryLayout.DomainTestProjects.Select(Path.GetFileNameWithoutExtension).ToList();
        var missing = RepositoryLayout.DomainTestProjects.Where(p => !File.Exists(RepositoryLayout.PathOf(p))).ToList();

        missing.ShouldBeEmpty();
        onDisk.ShouldBeSubsetOf(covered);
    }

    [Theory]
    [MemberData(nameof(DomainTestProjects))]
    [Trait("Req", "NFR-6")]
    public void NFR_6_domain_test_projects_reference_no_ef_core_sqlite_asp_net_core_or_mocking_library(string testProject)
    {
        var closure = ProjectClosure(RepositoryLayout.PathOf(testProject));

        var violations = closure.SelectMany(project =>
                RepositoryLayout.PackageReferencesOf(project)
                    .Where(p => SolutionAssemblies.MatchesAny(p, ForbiddenPackages))
                    .Concat(RepositoryLayout.FrameworkReferencesOf(project).Where(f => ForbiddenFrameworks.Contains(f, StringComparer.OrdinalIgnoreCase)))
                    .Concat(ForbiddenSdks.Where(s => string.Equals(RepositoryLayout.SdkOf(project), s, StringComparison.OrdinalIgnoreCase)))
                    .Select(reference => $"{Path.GetFileNameWithoutExtension(project)} -> {reference}"))
            .ToList();

        violations.ShouldBeEmpty();
    }

    /// <summary>The project file and every project file reachable from it through project references.</summary>
    private static IReadOnlyList<string> ProjectClosure(string projectFile)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var pending = new Stack<string>([Path.GetFullPath(projectFile)]);

        while (pending.TryPop(out var current))
        {
            if (!seen.Add(current))
            {
                continue;
            }

            var directory = Path.GetDirectoryName(current)!;
            foreach (var include in XDocument.Load(current).Descendants("ProjectReference")
                         .Select(e => (string?)e.Attribute("Include"))
                         .OfType<string>())
            {
                pending.Push(Path.GetFullPath(Path.Combine(directory, include.Replace('\\', Path.DirectorySeparatorChar))));
            }
        }

        return [.. seen];
    }
}
