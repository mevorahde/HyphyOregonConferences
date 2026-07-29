using System.Reflection;
using HyphyOregon.ConferenceGenerator.Cli;

namespace HyphyOregon.ConferenceGenerator.Tests;

[TestClass]
[TestCategory("Stage3")]
public sealed class CliHostSafetyTests
{
    [TestMethod]
    public async Task UnknownOptionsUseStderrAndExitTwo()
    {
        CliRunResult result = await CliTestHarness.RunAsync(["--unknown"]);

        Assert.AreEqual(ExitCodes.UsageOrValidationError, result.ExitCode);
        Assert.AreEqual(string.Empty, result.StandardOutput);
        StringAssert.Contains(result.StandardError, "Unknown option", StringComparison.Ordinal);
        StringAssert.Contains(result.StandardError, "Use --help", StringComparison.Ordinal);
    }

    [TestMethod]
    public async Task UnexpectedFailureIsGenericButObservableThroughInjectedBoundary()
    {
        Exception? observed = null;
        var host = new CliHost(
            new CommandLineParser(),
            new ThrowingApplication(),
            exception => observed = exception);
        var output = new StringWriter { NewLine = "\n" };
        var error = new StringWriter { NewLine = "\n" };

        int exitCode = await host.RunAsync(
            [],
            new StringReader(string.Empty),
            output,
            error);

        Assert.AreEqual(ExitCodes.InternalError, exitCode);
        Assert.IsInstanceOfType<InvalidOperationException>(observed);
        Assert.AreEqual(string.Empty, output.ToString());
        Assert.AreEqual("An unexpected internal error occurred.\n", error.ToString());
        Assert.DoesNotContain("sensitive diagnostic", error.ToString(), StringComparison.Ordinal);
        Assert.DoesNotContain("Exception", error.ToString(), StringComparison.Ordinal);
    }

    [TestMethod]
    public void LoadingProgramTypeDoesNotExecuteWorkflow()
    {
        Assembly assembly = typeof(CliMetadata).Assembly;

        Type? programType = assembly.GetType(
            "HyphyOregon.ConferenceGenerator.Cli.Program",
            throwOnError: true);

        Assert.IsNotNull(programType);
        Assert.IsNull(programType.TypeInitializer);
    }

    [TestMethod]
    public async Task HelpDoesNotReadFromRedirectedInput()
    {
        CliHost host = CliTestHarness.CreateHost();
        var output = new StringWriter { NewLine = "\n" };
        var error = new StringWriter { NewLine = "\n" };

        int exitCode = await host.RunAsync(
            ["--help"],
            new ThrowingTextReader(),
            output,
            error);

        Assert.AreEqual(ExitCodes.Success, exitCode);
        Assert.AreEqual(string.Empty, error.ToString());
    }

    private sealed class ThrowingApplication : IConferenceGeneratorApplication
    {
        public ValueTask ExecuteAsync(
            CliInvocation invocation,
            TextReader input,
            TextWriter output,
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException("sensitive diagnostic");
    }

    private sealed class ThrowingTextReader : TextReader
    {
        public override string? ReadLine() =>
            throw new InvalidOperationException("Input must not be read.");

        public override ValueTask<string?> ReadLineAsync(CancellationToken cancellationToken) =>
            throw new InvalidOperationException("Input must not be read.");
    }
}
