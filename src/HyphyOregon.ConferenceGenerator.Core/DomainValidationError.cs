namespace HyphyOregon.ConferenceGenerator.Core;

public enum DomainValidationError
{
    Unknown = 0,
    MissingCollection,
    InvalidCollectionItem,
    InvalidOwnerName,
    InvalidConferenceName,
    DuplicateOwner,
    DuplicateConference,
    TooFewConferences,
    TooFewOwners,
    UnevenConferenceSizes,
    InvalidDefaultOwnerCount
}
