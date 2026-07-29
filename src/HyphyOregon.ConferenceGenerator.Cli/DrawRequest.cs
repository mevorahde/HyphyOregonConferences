using System.Collections.ObjectModel;

namespace HyphyOregon.ConferenceGenerator.Cli;

public sealed record DrawRequest
{
    public DrawRequest(
        IEnumerable<string> ownerNames,
        IEnumerable<string>? conferenceNames = null,
        ulong? seed = null)
    {
        ArgumentNullException.ThrowIfNull(ownerNames);

        OwnerNames = Array.AsReadOnly(ownerNames.ToArray());
        ConferenceNames = Array.AsReadOnly(conferenceNames?.ToArray() ?? []);
        Seed = seed;
    }

    public ReadOnlyCollection<string> OwnerNames { get; }

    public ReadOnlyCollection<string> ConferenceNames { get; }

    public ulong? Seed { get; }
}
