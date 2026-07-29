using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Cli;

public static class CompositionRoot
{
    public static ConferenceAssigner CreateProductionAssigner() =>
        new(SystemBoundedRandomSource.Shared);

    public static ConferenceAssigner CreateSeededAssigner(ulong seed) =>
        new(new StableSeededRandomSource(seed));
}
