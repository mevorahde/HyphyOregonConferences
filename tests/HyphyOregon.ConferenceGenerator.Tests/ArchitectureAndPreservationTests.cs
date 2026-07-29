using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Tests;

[TestClass]
public sealed class ArchitectureAndPreservationTests
{
    [TestMethod]
    public void CoreDoesNotReferenceOperationalAssemblies()
    {
        string[] references = typeof(AssignmentRequest).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToArray();

        CollectionAssert.DoesNotContain(references, "System.Console");
        CollectionAssert.DoesNotContain(references, "System.IO.FileSystem");
        CollectionAssert.DoesNotContain(references, "System.Net.Http");
        CollectionAssert.DoesNotContain(references, "System.Diagnostics.Process");
    }

    [TestMethod]
    public void CoreSourceContainsNoOperationalAccess()
    {
        string root = FindRepositoryRoot();
        string coreDirectory = Path.Combine(
            root,
            "src",
            "HyphyOregon.ConferenceGenerator.Core");
        string source = string.Join(
            "\n",
            Directory.EnumerateFiles(coreDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(path => !IsGeneratedPath(path))
                .Select(File.ReadAllText));

        string[] forbidden =
        [
            "Console.",
            "System.IO",
            "System.Net",
            "System.Diagnostics.Process",
            "Environment.GetEnvironmentVariable",
            "Microsoft.Win32"
        ];

        foreach (string value in forbidden)
        {
            Assert.IsFalse(
                source.Contains(value, StringComparison.Ordinal),
                $"Core source must not contain operational access through '{value}'.");
        }
    }

    [TestMethod]
    public void SupersededLegacyProjectFilesAreRetiredFromCurrentTree()
    {
        string root = FindRepositoryRoot();
        string[] retiredPaths =
        {
            "HyphyOregonConferences.sln",
            "HyphyOregonConferences/App.config",
            "HyphyOregonConferences/HyphyOregonConferences.csproj",
            "HyphyOregonConferences/Program.cs",
            "HyphyOregonConferences/Properties/AssemblyInfo.cs",
            "HyphyOregonConferences/favicon.ico"
        };

        foreach (string relativePath in retiredPaths)
        {
            Assert.IsFalse(
                File.Exists(Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar))),
                relativePath);
        }
    }

    [TestMethod]
    public void ModernSolutionReferencesOnlyModernProjects()
    {
        string solution = File.ReadAllText(
            Path.Combine(FindRepositoryRoot(), "HyphyOregon.ConferenceGenerator.slnx"));

        StringAssert.Contains(solution, "HyphyOregon.ConferenceGenerator.Core.csproj");
        StringAssert.Contains(solution, "HyphyOregon.ConferenceGenerator.Cli.csproj");
        StringAssert.Contains(solution, "HyphyOregon.ConferenceGenerator.Tests.csproj");
        Assert.IsFalse(solution.Contains("HyphyOregonConferences.csproj", StringComparison.Ordinal));
    }

    private static bool IsGeneratedPath(string path)
    {
        string separator = Path.DirectorySeparatorChar.ToString();
        return path.Contains($"{separator}bin{separator}", StringComparison.OrdinalIgnoreCase)
            || path.Contains($"{separator}obj{separator}", StringComparison.OrdinalIgnoreCase);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git"))
                && File.Exists(
                    Path.Combine(directory.FullName, "HyphyOregon.ConferenceGenerator.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        Assert.Fail("Could not locate the repository root.");
        return string.Empty;
    }
}
