using System.Collections.ObjectModel;

namespace HyphyOregon.ConferenceGenerator.Cli;

public sealed record CliInvocation
{
    public CliInvocation(
        CliCommandKind command,
        IEnumerable<string>? ownerNames = null,
        IEnumerable<string>? conferenceNames = null,
        ulong? seed = null)
    {
        Command = command;
        OwnerNames = Array.AsReadOnly(ownerNames?.ToArray() ?? []);
        ConferenceNames = Array.AsReadOnly(conferenceNames?.ToArray() ?? []);
        Seed = seed;
    }

    public CliCommandKind Command { get; }

    public ReadOnlyCollection<string> OwnerNames { get; }

    public ReadOnlyCollection<string> ConferenceNames { get; }

    public ulong? Seed { get; }
}
