using HyphyOregon.ConferenceGenerator.Cli;

namespace HyphyOregon.ConferenceGenerator.Tests;

[TestClass]
[TestCategory("Stage3")]
public sealed class CommandLineParserTests
{
    private static readonly string[] ExpectedTwoOwners = ["Alex", "Blake"];
    private static readonly string[] ExpectedTwoConferences = ["East", "West"];
    private readonly CommandLineParser _parser = new();

    [TestMethod]
    public void ZeroArgumentsSelectInteractiveMode()
    {
        CliInvocation invocation = _parser.Parse([]);

        Assert.AreEqual(CliCommandKind.Interactive, invocation.Command);
    }

    [TestMethod]
    public void HelpAndVersionSelectTheirOwnModes()
    {
        Assert.AreEqual(CliCommandKind.Help, _parser.Parse(["--help"]).Command);
        Assert.AreEqual(CliCommandKind.Version, _parser.Parse(["--version"]).Command);
    }

    [TestMethod]
    public void RepeatedDrawingOptionsArePreservedInOrder()
    {
        string[] arguments =
        [
            "--owner", "Alex",
            "--owner", "Blake",
            "--conference", "East",
            "--conference", "West",
            "--seed", "20200830"
        ];

        CliInvocation invocation = _parser.Parse(arguments);

        Assert.AreEqual(CliCommandKind.Draw, invocation.Command);
        CollectionAssert.AreEqual(ExpectedTwoOwners, invocation.OwnerNames);
        CollectionAssert.AreEqual(ExpectedTwoConferences, invocation.ConferenceNames);
        Assert.AreEqual(20200830UL, invocation.Seed);
    }

    [TestMethod]
    public void ParserDoesNotRetainCallerOwnedCollection()
    {
        var arguments = new List<string> { "--owner", "Alex", "--owner", "Blake" };

        CliInvocation invocation = _parser.Parse(arguments);
        arguments.Clear();

        CollectionAssert.AreEqual(ExpectedTwoOwners, invocation.OwnerNames);
    }

    [TestMethod]
    public void UnknownOptionFails()
    {
        CommandLineUsageException exception =
            Assert.ThrowsExactly<CommandLineUsageException>(() => _parser.Parse(["--unknown"]));

        StringAssert.Contains(exception.Message, "Unknown option", StringComparison.Ordinal);
    }

    [TestMethod]
    [DataRow("--owner")]
    [DataRow("--conference")]
    [DataRow("--seed")]
    public void MissingOptionValueFails(string option)
    {
        CommandLineUsageException exception =
            Assert.ThrowsExactly<CommandLineUsageException>(() => _parser.Parse([option]));

        StringAssert.Contains(exception.Message, "requires a value", StringComparison.Ordinal);
    }

    [TestMethod]
    public void OptionFollowedByAnotherOptionHasNoValue()
    {
        _ = Assert.ThrowsExactly<CommandLineUsageException>(
            () => _parser.Parse(["--owner", "--seed", "1"]));
    }

    [TestMethod]
    public void DuplicateSeedFails()
    {
        CommandLineUsageException exception = Assert.ThrowsExactly<CommandLineUsageException>(
            () => _parser.Parse(["--seed", "1", "--seed", "2"]));

        StringAssert.Contains(exception.Message, "only once", StringComparison.Ordinal);
    }

    [TestMethod]
    [DataRow("-1")]
    [DataRow("+1")]
    [DataRow("1.0")]
    [DataRow("abc")]
    [DataRow("18446744073709551616")]
    public void InvalidOrOverflowingSeedFails(string seed)
    {
        _ = Assert.ThrowsExactly<CommandLineUsageException>(
            () => _parser.Parse(["--seed", seed]));
    }

    [TestMethod]
    public void HelpAndVersionConflict()
    {
        _ = Assert.ThrowsExactly<CommandLineUsageException>(
            () => _parser.Parse(["--help", "--version"]));
    }

    [TestMethod]
    [DataRow("--help")]
    [DataRow("--version")]
    public void InformationalModeConflictsWithDrawingOptions(string informationOption)
    {
        _ = Assert.ThrowsExactly<CommandLineUsageException>(
            () => _parser.Parse([informationOption, "--owner", "Alex"]));
    }
}
