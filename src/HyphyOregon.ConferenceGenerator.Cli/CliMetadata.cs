using System.Reflection;

namespace HyphyOregon.ConferenceGenerator.Cli;

public static class CliMetadata
{
    public static string ProductName { get; } = "Hyphy Oregon Conference Generator";

    public static string Version { get; } =
        typeof(CliMetadata).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion
        ?? throw new InvalidOperationException("Assembly version metadata is unavailable.");

    public static string Description { get; } =
        "Creates fair, reproducible fantasy-football conference assignments.";

    public static string HelpText { get; } =
        """
        Hyphy Oregon Conference Generator

        Usage:
          hyphy-conferences
          hyphy-conferences --help
          hyphy-conferences --version
          hyphy-conferences --owner <name> [--owner <name> ...]
                            [--conference <name> --conference <name> ...]
                            [--seed <unsigned-integer>]

        Options:
          --owner <name>       Add an owner. Repeat for every owner.
          --conference <name>  Add a conference. Repeat as needed.
                               East and West are used when omitted.
          --seed <value>       Use the stable SplitMix64-v1 generator.
          --help               Show this help.
          --version            Show version information.
        """;
}
