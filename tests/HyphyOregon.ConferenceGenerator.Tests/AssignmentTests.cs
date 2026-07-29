using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Tests;

[TestClass]
public sealed class AssignmentTests
{
    private static readonly int[] ExpectedFisherYatesBounds = [10, 9, 8, 7, 6, 5, 4, 3, 2];

    [TestMethod]
    public void DefaultWorkflowAllocatesFiveOwnersToEastAndWest()
    {
        AssignmentRequest request = TestData.CreateDefaultRequest();
        var assigner = new ConferenceAssigner(new StableSeededRandomSource(42));

        AssignmentResult result = assigner.Assign(request);

        Assert.AreEqual(10, result.Assignments.Count);
        Assert.AreEqual(
            5,
            result.Assignments.Count(assignment => assignment.Conference.Name == "East"));
        Assert.AreEqual(
            5,
            result.Assignments.Count(assignment => assignment.Conference.Name == "West"));
    }

    [TestMethod]
    public void EveryOwnerAppearsExactlyOnce()
    {
        AssignmentRequest request = TestData.CreateDefaultRequest();
        var assigner = new ConferenceAssigner(new StableSeededRandomSource(7));

        AssignmentResult result = assigner.Assign(request);

        string[] assignedNames = result.Assignments
            .Select(assignment => assignment.Owner.Name)
            .Order(StringComparer.Ordinal)
            .ToArray();
        string[] expectedNames = TestData.DefaultOwnerNames
            .Order(StringComparer.Ordinal)
            .ToArray();

        CollectionAssert.AreEqual(expectedNames, assignedNames);
        Assert.AreEqual(assignedNames.Length, assignedNames.Distinct().Count());
    }

    [TestMethod]
    public void CustomTwelveOwnerThreeConferenceAllocationWorks()
    {
        Owner[] owners = Enumerable.Range(1, 12)
            .Select(index => new Owner($"Owner {index}"))
            .ToArray();
        Conference[] conferences =
        [
            new Conference("North"),
            new Conference("Central"),
            new Conference("South")
        ];
        var request = new AssignmentRequest(owners, conferences);
        var assigner = new ConferenceAssigner(new StableSeededRandomSource(12));

        AssignmentResult result = assigner.Assign(request);

        Assert.AreEqual(12, result.Assignments.Count);
        foreach (Conference conference in conferences)
        {
            Assert.AreEqual(
                4,
                result.Assignments.Count(assignment => assignment.Conference == conference));
        }
    }

    [TestMethod]
    public void CallerOwnedCollectionsRemainUnchanged()
    {
        var owners = TestData.DefaultOwnerNames.Select(name => new Owner(name)).ToList();
        var conferences = new List<Conference>
        {
            new("East"),
            new("West")
        };
        Owner[] originalOwnerOrder = owners.ToArray();
        Conference[] originalConferenceOrder = conferences.ToArray();
        var request = new AssignmentRequest(owners, conferences);

        owners.Reverse();
        conferences.Clear();
        var assigner = new ConferenceAssigner(new StableSeededRandomSource(99));
        _ = assigner.Assign(request);

        CollectionAssert.AreEqual(originalOwnerOrder, request.Owners);
        CollectionAssert.AreEqual(originalConferenceOrder, request.Conferences);
        CollectionAssert.AreEqual(originalOwnerOrder.Reverse().ToArray(), owners);
        Assert.AreEqual(0, conferences.Count);
    }

    [TestMethod]
    public void FisherYatesRequestsEachRequiredExclusiveBound()
    {
        AssignmentRequest request = TestData.CreateDefaultRequest();
        var random = new RecordingRandomSource();
        var assigner = new ConferenceAssigner(random);

        _ = assigner.Assign(request);

        CollectionAssert.AreEqual(
            ExpectedFisherYatesBounds,
            random.RequestedBounds);
    }

    private sealed class RecordingRandomSource : IBoundedRandomSource
    {
        internal List<int> RequestedBounds { get; } = [];

        public int NextInt32(int exclusiveUpperBound)
        {
            RequestedBounds.Add(exclusiveUpperBound);
            return 0;
        }
    }
}
