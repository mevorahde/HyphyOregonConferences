using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Cli;

public sealed record ConferenceDraw(AssignmentResult Result, ulong? Seed)
{
    public AssignmentResult Result { get; } =
        Result ?? throw new ArgumentNullException(nameof(Result));
}
