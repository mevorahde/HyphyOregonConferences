namespace HyphyOregon.ConferenceGenerator.Core;

public interface IBoundedRandomSource
{
    public int NextInt32(int exclusiveUpperBound);
}
