namespace HyphyOregon.ConferenceGenerator.Core;

public sealed class StableSeededRandomSource : IBoundedRandomSource
{
    private const ulong Increment = 0x9E3779B97F4A7C15UL;
    private const ulong FirstMixer = 0xBF58476D1CE4E5B9UL;
    private const ulong SecondMixer = 0x94D049BB133111EBUL;
    private ulong _state;

    public StableSeededRandomSource(ulong seed)
    {
        _state = seed;
    }

    public static string AlgorithmContract => "SplitMix64-v1 with rejection-sampled bounded integers";

    public int NextInt32(int exclusiveUpperBound) =>
        UnbiasedRange.NextInt32(exclusiveUpperBound, NextUInt64);

    private ulong NextUInt64()
    {
        unchecked
        {
            _state += Increment;
            ulong value = _state;
            value = (value ^ (value >> 30)) * FirstMixer;
            value = (value ^ (value >> 27)) * SecondMixer;
            return value ^ (value >> 31);
        }
    }
}
