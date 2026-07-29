using System.Collections.ObjectModel;

namespace HyphyOregon.ConferenceGenerator.Core;

public sealed record AssignmentRequest
{
    public const int DefaultOwnerCount = 10;

    public AssignmentRequest(
        IEnumerable<Owner>? owners,
        IEnumerable<Conference>? conferences)
    {
        Owner[] ownerSnapshot = SnapshotOwners(owners);
        Conference[] conferenceSnapshot = SnapshotConferences(conferences);

        Validate(ownerSnapshot, conferenceSnapshot);

        Owners = Array.AsReadOnly(ownerSnapshot);
        Conferences = Array.AsReadOnly(conferenceSnapshot);
    }

    public ReadOnlyCollection<Owner> Owners { get; }

    public ReadOnlyCollection<Conference> Conferences { get; }

    public int OwnersPerConference => Owners.Count / Conferences.Count;

    public static AssignmentRequest CreateDefault(IEnumerable<string?>? ownerNames)
    {
        if (ownerNames is null)
        {
            throw new DomainValidationException(
                DomainValidationError.MissingCollection,
                "The owner collection cannot be null.");
        }

        Owner[] owners = ownerNames.Select(name => new Owner(name)).ToArray();
        if (owners.Length != DefaultOwnerCount)
        {
            throw new DomainValidationException(
                DomainValidationError.InvalidDefaultOwnerCount,
                $"The default workflow requires exactly {DefaultOwnerCount} owners.");
        }

        Conference[] conferences =
        [
            new Conference("East"),
            new Conference("West")
        ];

        return new AssignmentRequest(owners, conferences);
    }

    private static Owner[] SnapshotOwners(IEnumerable<Owner>? owners)
    {
        if (owners is null)
        {
            throw new DomainValidationException(
                DomainValidationError.MissingCollection,
                "The owner collection cannot be null.");
        }

        Owner[] snapshot = owners.ToArray();
        if (snapshot.Any(owner => owner is null))
        {
            throw new DomainValidationException(
                DomainValidationError.InvalidCollectionItem,
                "The owner collection cannot contain null items.");
        }

        return snapshot;
    }

    private static Conference[] SnapshotConferences(IEnumerable<Conference>? conferences)
    {
        if (conferences is null)
        {
            throw new DomainValidationException(
                DomainValidationError.MissingCollection,
                "The conference collection cannot be null.");
        }

        Conference[] snapshot = conferences.ToArray();
        if (snapshot.Any(conference => conference is null))
        {
            throw new DomainValidationException(
                DomainValidationError.InvalidCollectionItem,
                "The conference collection cannot contain null items.");
        }

        return snapshot;
    }

    private static void Validate(Owner[] owners, Conference[] conferences)
    {
        if (conferences.Length < 2)
        {
            throw new DomainValidationException(
                DomainValidationError.TooFewConferences,
                "At least two conferences are required.");
        }

        if (owners.Length < conferences.Length)
        {
            throw new DomainValidationException(
                DomainValidationError.TooFewOwners,
                "At least one owner is required for every conference.");
        }

        if (owners.Length % conferences.Length != 0)
        {
            throw new DomainValidationException(
                DomainValidationError.UnevenConferenceSizes,
                "Owner count must be exactly divisible by conference count.");
        }

        AssignmentValidation.EnsureUniqueOwners(owners);
        AssignmentValidation.EnsureUniqueConferences(conferences);
    }
}
