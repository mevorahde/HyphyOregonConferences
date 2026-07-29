using System.Collections.ObjectModel;

namespace HyphyOregon.ConferenceGenerator.Core;

public sealed record AssignmentResult
{
    public AssignmentResult(IEnumerable<OwnerAssignment> assignments)
    {
        ArgumentNullException.ThrowIfNull(assignments);

        OwnerAssignment[] snapshot = assignments.ToArray();
        if (snapshot.Any(assignment => assignment is null))
        {
            throw new ArgumentException(
                "Assignments cannot contain null items.",
                nameof(assignments));
        }

        Assignments = Array.AsReadOnly(snapshot);
    }

    public ReadOnlyCollection<OwnerAssignment> Assignments { get; }
}
