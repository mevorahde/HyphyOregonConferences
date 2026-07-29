using System.Reflection;
using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Tests;

[TestClass]
public sealed class RandomizationTests
{
    [TestMethod]
    public void SameSeedAndInputProduceIdenticalAssignments()
    {
        AssignmentRequest request = TestData.CreateDefaultRequest();

        string[] first = AssignWithSeed(request, 20200830);
        string[] second = AssignWithSeed(request, 20200830);

        CollectionAssert.AreEqual(first, second);
    }

    [TestMethod]
    public void FixedSeedProducesCheckedGoldenAssignment()
    {
        AssignmentRequest request = TestData.CreateDefaultRequest();

        string[] actual = AssignWithSeed(request, 20200830);

        string[] expected =
        [
            "Indigo:East",
            "Devon:East",
            "Casey:East",
            "Finley:East",
            "Blake:East",
            "Emery:West",
            "Jordan:West",
            "Alex:West",
            "Gray:West",
            "Harper:West"
        ];
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void StableSeededSourceDoesNotContainSystemRandomState()
    {
        FieldInfo[] fields = typeof(StableSeededRandomSource).GetFields(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        Assert.IsFalse(fields.Any(field => field.FieldType == typeof(Random)));
        StringAssert.StartsWith(
            StableSeededRandomSource.AlgorithmContract,
            "SplitMix64-v1",
            StringComparison.Ordinal);
    }

    [TestMethod]
    public void DeterministicBoundsAreAlwaysRespected()
    {
        var source = new StableSeededRandomSource(ulong.MaxValue);
        int[] bounds = [1, 2, 3, 10, 257, int.MaxValue];

        foreach (int bound in bounds)
        {
            for (int sample = 0; sample < 2_000; sample++)
            {
                int value = source.NextInt32(bound);
                Assert.IsGreaterThanOrEqualTo(0, value);
                Assert.IsLessThan(bound, value);
            }
        }
    }

    [TestMethod]
    public void UnbiasedReductionRejectsTheIncompleteModuloRange()
    {
        var values = new Queue<ulong>([0UL, 5UL]);
        int calls = 0;

        int result = UnbiasedRange.NextInt32(
            3,
            () =>
            {
                calls++;
                return values.Dequeue();
            });

        Assert.AreEqual(2, calls);
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void RandomSourcesRejectNonPositiveBounds()
    {
        var stable = new StableSeededRandomSource(1);

        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => stable.NextInt32(0));
        _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => SystemBoundedRandomSource.Shared.NextInt32(-1));
    }

    [TestMethod]
    public void SystemBackedSourceRespectsBounds()
    {
        for (int sample = 0; sample < 100; sample++)
        {
            int value = SystemBoundedRandomSource.Shared.NextInt32(7);
            Assert.IsGreaterThanOrEqualTo(0, value);
            Assert.IsLessThan(7, value);
        }
    }

    private static string[] AssignWithSeed(AssignmentRequest request, ulong seed)
    {
        var assigner = new ConferenceAssigner(new StableSeededRandomSource(seed));
        return assigner.Assign(request).Assignments
            .Select(assignment => $"{assignment.Owner.Name}:{assignment.Conference.Name}")
            .ToArray();
    }
}
