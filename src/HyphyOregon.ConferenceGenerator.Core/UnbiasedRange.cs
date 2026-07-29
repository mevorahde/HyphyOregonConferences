namespace HyphyOregon.ConferenceGenerator.Core;

internal static class UnbiasedRange
{
    internal static int NextInt32(int exclusiveUpperBound, Func<ulong> nextUInt64)
    {
        if (exclusiveUpperBound <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(exclusiveUpperBound),
                "The exclusive upper bound must be positive.");
        }

        ArgumentNullException.ThrowIfNull(nextUInt64);

        ulong bound = (uint)exclusiveUpperBound;
        ulong rejectionThreshold = unchecked(0UL - bound) % bound;

        ulong value;
        do
        {
            value = nextUInt64();
        }
        while (value < rejectionThreshold);

        return (int)(value % bound);
    }
}
