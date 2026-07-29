using System.Globalization;

namespace HyphyOregon.ConferenceGenerator.Cli;

public sealed class CommandLineParser : ICommandLineParser
{
    public CliInvocation Parse(IEnumerable<string>? arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        string[] args = arguments.ToArray();
        if (args.Length == 0)
        {
            return new CliInvocation(CliCommandKind.Interactive);
        }

        var owners = new List<string>();
        var conferences = new List<string>();
        ulong? seed = null;
        bool helpRequested = false;
        bool versionRequested = false;
        bool drawingOptionSeen = false;

        for (int index = 0; index < args.Length; index++)
        {
            string option = args[index];
            switch (option)
            {
                case "--help":
                    helpRequested = true;
                    break;

                case "--version":
                    versionRequested = true;
                    break;

                case "--owner":
                    drawingOptionSeen = true;
                    owners.Add(ReadRequiredValue(args, ref index, option));
                    break;

                case "--conference":
                    drawingOptionSeen = true;
                    conferences.Add(ReadRequiredValue(args, ref index, option));
                    break;

                case "--seed":
                    drawingOptionSeen = true;
                    if (seed.HasValue)
                    {
                        throw new CommandLineUsageException(
                            "The --seed option may be supplied only once.");
                    }

                    seed = ParseSeed(ReadRequiredValue(args, ref index, option));
                    break;

                default:
                    throw new CommandLineUsageException($"Unknown option: {option}");
            }
        }

        if (helpRequested && versionRequested)
        {
            throw new CommandLineUsageException(
                "The --help and --version modes cannot be combined.");
        }

        if ((helpRequested || versionRequested) && drawingOptionSeen)
        {
            throw new CommandLineUsageException(
                "Help or version mode cannot be combined with drawing options.");
        }

        if (helpRequested)
        {
            return new CliInvocation(CliCommandKind.Help);
        }

        if (versionRequested)
        {
            return new CliInvocation(CliCommandKind.Version);
        }

        return new CliInvocation(CliCommandKind.Draw, owners, conferences, seed);
    }

    private static string ReadRequiredValue(string[] args, ref int index, string option)
    {
        int valueIndex = index + 1;
        if (valueIndex >= args.Length || args[valueIndex].StartsWith("--", StringComparison.Ordinal))
        {
            throw new CommandLineUsageException($"{option} requires a value.");
        }

        index = valueIndex;
        return args[valueIndex];
    }

    private static ulong ParseSeed(string value)
    {
        string trimmed = value.Trim();
        if (!ulong.TryParse(
                trimmed,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out ulong seed))
        {
            throw new CommandLineUsageException(
                "The --seed value must be an unsigned integer from 0 through 18446744073709551615.");
        }

        return seed;
    }
}
