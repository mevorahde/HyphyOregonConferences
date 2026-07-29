using System.Security.Cryptography;
using System.Text;
using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Tests;

[TestClass]
public sealed class ArchitectureAndPreservationTests
{
    private static readonly IReadOnlyDictionary<string, string> LegacyBlobIds =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [".gitattributes"] = "1ff0c423042b46cb1d617b81efb715defbe8054d",
            [".gitignore"] = "3c4efe206bd0e7230ad0ae8396a3c883c8207906",
            ["HyphyOregonConferences.sln"] = "32a042f25dbf535da246b400df10dd1beb343821",
            ["HyphyOregonConferences/App.config"] = "88fa4027bda397de6bf19f0940e5dd6026c877f9",
            ["HyphyOregonConferences/HyphyOregonConferences.csproj"] =
                "9933cfa2fa3390f7c55524e6192f9759f5ba9d0c",
            ["HyphyOregonConferences/Program.cs"] =
                "888549fe6a8981c123f60b2c95f2f6de4684d964",
            ["HyphyOregonConferences/Properties/AssemblyInfo.cs"] =
                "7b7aaf5c52d9b1c4f9f18f35daac6ed51ef2e990",
            ["HyphyOregonConferences/favicon.ico"] =
                "dcc2142adb099f9ba8648392d3c05712c7ade94b",
            ["README.md"] = "9ee2050fc406279d720024fa44740cadb081fda3"
        };

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
    public void EveryLegacyTrackedBlobRemainsUnchanged()
    {
        string root = FindRepositoryRoot();

        foreach ((string relativePath, string expectedBlobId) in LegacyBlobIds)
        {
            string path = Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
            byte[] content = Path.GetExtension(path).Equals(".ico", StringComparison.OrdinalIgnoreCase)
                ? File.ReadAllBytes(path)
                : NormalizeTextForGit(path);

            Assert.AreEqual(expectedBlobId, ComputeGitBlobId(content), relativePath);
        }
    }

    [TestMethod]
    public void LegacyIconHashRemainsUnchanged()
    {
        string iconPath = Path.Combine(
            FindRepositoryRoot(),
            "HyphyOregonConferences",
            "favicon.ico");

        string hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(iconPath)));

        Assert.AreEqual(
            "459FA2C9FC39DADF195D480826FA8B9A7632259DD23BD4557BC5C1E394208025",
            hash);
    }

    private static string ComputeGitBlobId(byte[] content)
    {
        byte[] header = Encoding.ASCII.GetBytes($"blob {content.Length}\0");
        byte[] blob = new byte[header.Length + content.Length];
        Buffer.BlockCopy(header, 0, blob, 0, header.Length);
        Buffer.BlockCopy(content, 0, blob, header.Length, content.Length);
#pragma warning disable CA5350 // Git's legacy object ID format specifically requires SHA-1.
        return Convert.ToHexString(SHA1.HashData(blob)).ToLowerInvariant();
#pragma warning restore CA5350
    }

    private static bool IsGeneratedPath(string path)
    {
        string separator = Path.DirectorySeparatorChar.ToString();
        return path.Contains($"{separator}bin{separator}", StringComparison.OrdinalIgnoreCase)
            || path.Contains($"{separator}obj{separator}", StringComparison.OrdinalIgnoreCase);
    }

    private static byte[] NormalizeTextForGit(string path)
    {
        byte[] raw = File.ReadAllBytes(path);
        bool hasUtf8Bom = raw.AsSpan().StartsWith(Encoding.UTF8.Preamble);
        ReadOnlySpan<byte> textBytes = hasUtf8Bom
            ? raw.AsSpan(Encoding.UTF8.Preamble.Length)
            : raw;
        string normalized = Encoding.UTF8.GetString(textBytes)
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal);
        byte[] normalizedBytes = Encoding.UTF8.GetBytes(normalized);

        if (!hasUtf8Bom)
        {
            return normalizedBytes;
        }

        byte[] result = new byte[Encoding.UTF8.Preamble.Length + normalizedBytes.Length];
        Encoding.UTF8.Preamble.CopyTo(result);
        normalizedBytes.CopyTo(result, Encoding.UTF8.Preamble.Length);
        return result;
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git"))
                && File.Exists(Path.Combine(directory.FullName, "HyphyOregonConferences.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        Assert.Fail("Could not locate the repository root.");
        return string.Empty;
    }
}
