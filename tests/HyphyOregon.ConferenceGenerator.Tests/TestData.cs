using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Tests;

internal static class TestData
{
    internal static readonly string[] DefaultOwnerNames =
    [
        "Alex",
        "Blake",
        "Casey",
        "Devon",
        "Emery",
        "Finley",
        "Gray",
        "Harper",
        "Indigo",
        "Jordan"
    ];

    internal static AssignmentRequest CreateDefaultRequest() =>
        AssignmentRequest.CreateDefault(DefaultOwnerNames);
}
