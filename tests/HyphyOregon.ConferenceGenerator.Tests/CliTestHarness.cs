using HyphyOregon.ConferenceGenerator.Cli;
using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Tests;

internal static class CliTestHarness
{
    internal static CliHost CreateHost(
        IRandomSourceFactory? randomSourceFactory = null,
        Action<Exception>? unexpectedExceptionObserver = null)
    {
        var application = new ConferenceGeneratorApplication(
            new InteractiveWorkflow(),
            new ConferenceDrawService(randomSourceFactory ?? new RandomSourceFactory()),
            new DrawPresenter());
        return new CliHost(
            new CommandLineParser(),
            application,
            unexpectedExceptionObserver);
    }

    internal static async Task<CliRunResult> RunAsync(
        IEnumerable<string> arguments,
        string input = "",
        IRandomSourceFactory? randomSourceFactory = null,
        CancellationToken cancellationToken = default)
    {
        CliHost host = CreateHost(randomSourceFactory);
        var output = new StringWriter { NewLine = "\n" };
        var error = new StringWriter { NewLine = "\n" };
        int exitCode = await host.RunAsync(
            arguments,
            new StringReader(input),
            output,
            error,
            cancellationToken);
        return new CliRunResult(exitCode, output.ToString(), error.ToString());
    }

    internal static string[] DefaultOwnerArguments(ulong? seed = null)
    {
        var arguments = new List<string>();
        foreach (string owner in TestData.DefaultOwnerNames)
        {
            arguments.Add("--owner");
            arguments.Add(owner);
        }

        if (seed.HasValue)
        {
            arguments.Add("--seed");
            arguments.Add(seed.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        return arguments.ToArray();
    }

    internal static string Lines(params IEnumerable<string> lines) =>
        string.Concat(lines.Select(line => $"{line}\n"));
}

internal sealed record CliRunResult(int ExitCode, string StandardOutput, string StandardError);

internal sealed class RecordingRandomSourceFactory : IRandomSourceFactory
{
    internal int SystemCreateCount { get; private set; }

    internal int SeededCreateCount { get; private set; }

    internal ulong? LastSeed { get; private set; }

    public IBoundedRandomSource CreateSystem()
    {
        SystemCreateCount++;
        return new ZeroRandomSource();
    }

    public IBoundedRandomSource CreateSeeded(ulong seed)
    {
        SeededCreateCount++;
        LastSeed = seed;
        return new StableSeededRandomSource(seed);
    }

    private sealed class ZeroRandomSource : IBoundedRandomSource
    {
        public int NextInt32(int exclusiveUpperBound)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exclusiveUpperBound);

            return 0;
        }
    }
}
