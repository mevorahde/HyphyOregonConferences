namespace HyphyOregon.ConferenceGenerator.Core;

public sealed class ConferenceAssigner
{
    private readonly IBoundedRandomSource _randomSource;

    public ConferenceAssigner(IBoundedRandomSource randomSource)
    {
        _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
    }

    public AssignmentResult Assign(AssignmentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        Owner[] shuffledOwners = request.Owners.ToArray();
        Shuffle(shuffledOwners);

        var assignments = new OwnerAssignment[shuffledOwners.Length];
        int assignmentIndex = 0;

        foreach (Conference conference in request.Conferences)
        {
            for (int position = 0; position < request.OwnersPerConference; position++)
            {
                assignments[assignmentIndex] =
                    new OwnerAssignment(shuffledOwners[assignmentIndex], conference);
                assignmentIndex++;
            }
        }

        return new AssignmentResult(assignments);
    }

    private void Shuffle(Owner[] owners)
    {
        for (int current = owners.Length - 1; current > 0; current--)
        {
            int selected = _randomSource.NextInt32(current + 1);
            (owners[current], owners[selected]) = (owners[selected], owners[current]);
        }
    }
}
