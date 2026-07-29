namespace HyphyOregon.ConferenceGenerator.Core;

public static class AssignmentValidation
{
    public static void EnsureUniqueOwners(IEnumerable<Owner>? owners)
    {
        if (owners is null)
        {
            throw new DomainValidationException(
                DomainValidationError.MissingCollection,
                "The owner collection cannot be null.");
        }

        var uniqueNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Owner owner in owners)
        {
            if (owner is null)
            {
                throw new DomainValidationException(
                    DomainValidationError.InvalidCollectionItem,
                    "The owner collection cannot contain null items.");
            }

            if (!uniqueNames.Add(owner.Name))
            {
                throw new DomainValidationException(
                    DomainValidationError.DuplicateOwner,
                    $"Owner names must be unique; '{owner.Name}' appears more than once.");
            }
        }
    }

    public static void EnsureUniqueConferences(IEnumerable<Conference>? conferences)
    {
        if (conferences is null)
        {
            throw new DomainValidationException(
                DomainValidationError.MissingCollection,
                "The conference collection cannot be null.");
        }

        var uniqueNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Conference conference in conferences)
        {
            if (conference is null)
            {
                throw new DomainValidationException(
                    DomainValidationError.InvalidCollectionItem,
                    "The conference collection cannot contain null items.");
            }

            if (!uniqueNames.Add(conference.Name))
            {
                throw new DomainValidationException(
                    DomainValidationError.DuplicateConference,
                    $"Conference names must be unique; '{conference.Name}' appears more than once.");
            }
        }
    }
}
