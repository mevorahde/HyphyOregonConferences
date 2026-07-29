namespace HyphyOregon.ConferenceGenerator.Core;

public sealed record OwnerAssignment
{
    public OwnerAssignment(Owner owner, Conference conference)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Conference = conference ?? throw new ArgumentNullException(nameof(conference));
    }

    public Owner Owner { get; }

    public Conference Conference { get; }
}
