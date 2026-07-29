using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Tests;

[TestClass]
public sealed class ValidationTests
{
    [TestMethod]
    public void NonDivisibleConfigurationFails()
    {
        Owner[] owners = Enumerable.Range(1, 10)
            .Select(index => new Owner($"Owner {index}"))
            .ToArray();
        Conference[] conferences =
        [
            new("North"),
            new("Central"),
            new("South")
        ];

        DomainValidationException exception = Assert.ThrowsExactly<DomainValidationException>(
            () => new AssignmentRequest(owners, conferences));

        Assert.AreEqual(DomainValidationError.UnevenConferenceSizes, exception.Error);
    }

    [TestMethod]
    public void TooFewConferencesFails()
    {
        Owner[] owners = [new("Alex")];
        Conference[] conferences = [new("East")];

        DomainValidationException exception = Assert.ThrowsExactly<DomainValidationException>(
            () => new AssignmentRequest(owners, conferences));

        Assert.AreEqual(DomainValidationError.TooFewConferences, exception.Error);
    }

    [TestMethod]
    public void TooFewOwnersFails()
    {
        Owner[] owners = [new("Alex")];
        Conference[] conferences = [new("East"), new("West")];

        DomainValidationException exception = Assert.ThrowsExactly<DomainValidationException>(
            () => new AssignmentRequest(owners, conferences));

        Assert.AreEqual(DomainValidationError.TooFewOwners, exception.Error);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("\t")]
    [DataRow("Alex\u0001")]
    [DataRow("Alex\rBlake")]
    [DataRow("Alex\nBlake")]
    public void InvalidOwnerNamesFail(string? name)
    {
        DomainValidationException exception = Assert.ThrowsExactly<DomainValidationException>(
            () => new Owner(name));

        Assert.AreEqual(DomainValidationError.InvalidOwnerName, exception.Error);
        Assert.IsFalse(string.IsNullOrWhiteSpace(exception.Message));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("East\u0001")]
    [DataRow("East\nWest")]
    public void InvalidConferenceNamesFail(string? name)
    {
        DomainValidationException exception = Assert.ThrowsExactly<DomainValidationException>(
            () => new Conference(name));

        Assert.AreEqual(DomainValidationError.InvalidConferenceName, exception.Error);
    }

    [TestMethod]
    [DataRow("Élodie")]
    [DataRow("李")]
    [DataRow("Anne Marie")]
    [DataRow("D'Angelo")]
    [DataRow("Jean-Luc")]
    [DataRow("O’Connor")]
    public void SupportedOwnerNamesAreAccepted(string name)
    {
        var owner = new Owner($"  {name}  ");

        Assert.AreEqual(name, owner.Name);
    }

    [TestMethod]
    public void CaseInsensitiveDuplicateOwnersFail()
    {
        Owner[] owners = [new("Alex"), new("ALEX")];
        Conference[] conferences = [new("East"), new("West")];

        DomainValidationException exception = Assert.ThrowsExactly<DomainValidationException>(
            () => new AssignmentRequest(owners, conferences));

        Assert.AreEqual(DomainValidationError.DuplicateOwner, exception.Error);
    }

    [TestMethod]
    public void CaseInsensitiveDuplicateConferencesFail()
    {
        Owner[] owners = [new("Alex"), new("Blake")];
        Conference[] conferences = [new("East"), new("EAST")];

        DomainValidationException exception = Assert.ThrowsExactly<DomainValidationException>(
            () => new AssignmentRequest(owners, conferences));

        Assert.AreEqual(DomainValidationError.DuplicateConference, exception.Error);
    }

    [TestMethod]
    public void DefaultWorkflowRequiresExactlyTenOwners()
    {
        DomainValidationException exception = Assert.ThrowsExactly<DomainValidationException>(
            () => AssignmentRequest.CreateDefault(TestData.DefaultOwnerNames.Take(9)));

        Assert.AreEqual(DomainValidationError.InvalidDefaultOwnerCount, exception.Error);
    }

    [TestMethod]
    public void NamesAreTrimmedAndCanonicallyNormalized()
    {
        var owner = new Owner("  E\u0301lodie  ");
        var conference = new Conference("  Pacific North  ");

        Assert.AreEqual("Élodie", owner.Name);
        Assert.AreEqual("Pacific North", conference.Name);
    }
}
