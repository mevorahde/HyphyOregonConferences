using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Cli;

public sealed class RandomSourceFactory : IRandomSourceFactory
{
    public IBoundedRandomSource CreateSystem() => SystemBoundedRandomSource.Shared;

    public IBoundedRandomSource CreateSeeded(ulong seed) => new StableSeededRandomSource(seed);
}
