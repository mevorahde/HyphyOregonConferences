using System.Security.Cryptography;

namespace HyphyOregon.ConferenceGenerator.Core;

public sealed class SystemBoundedRandomSource : IBoundedRandomSource
{
    private SystemBoundedRandomSource()
    {
    }

    public static SystemBoundedRandomSource Shared { get; } = new();

    public int NextInt32(int exclusiveUpperBound)
    {
        if (exclusiveUpperBound <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(exclusiveUpperBound),
                "The exclusive upper bound must be positive.");
        }

        return RandomNumberGenerator.GetInt32(exclusiveUpperBound);
    }
}
