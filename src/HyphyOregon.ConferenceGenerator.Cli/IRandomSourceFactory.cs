using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Cli;

public interface IRandomSourceFactory
{
    public IBoundedRandomSource CreateSystem();

    public IBoundedRandomSource CreateSeeded(ulong seed);
}
