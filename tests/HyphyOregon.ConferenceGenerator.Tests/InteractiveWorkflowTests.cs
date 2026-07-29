using HyphyOregon.ConferenceGenerator.Cli;

namespace HyphyOregon.ConferenceGenerator.Tests;

[TestClass]
[TestCategory("Stage3")]
public sealed class InteractiveWorkflowTests
{
    [TestMethod]
    public async Task EnterAcceptsDefaultTenOwnerEastWestWorkflow()
    {
        string input = CliTestHarness.Lines(
            [string.Empty, .. TestData.DefaultOwnerNames, "20200830"]);

        CliRunResult result = await CliTestHarness.RunAsync([], input);

        Assert.AreEqual(ExitCodes.Success, result.ExitCode);
        StringAssert.Contains(
            result.StandardOutput,
            "The default draw assigns 10 owners",
            StringComparison.Ordinal);
        StringAssert.Contains(result.StandardOutput, "Indigo", StringComparison.Ordinal);
        StringAssert.Contains(result.StandardOutput, "Seed: 20200830", StringComparison.Ordinal);
        Assert.AreEqual(string.Empty, result.StandardError);
    }

    [TestMethod]
    public async Task CustomInteractiveWorkflowSupportsCountsAndConferenceNames()
    {
        string input = CliTestHarness.Lines(
            [
                "n",
                "3",
                "6",
                "North",
                "Central",
                "South",
                "Anne Marie",
                "D'Angelo",
                "Jean-Luc",
                "Élodie",
                "李",
                "O’Connor",
                "9"
            ]);

        CliRunResult result = await CliTestHarness.RunAsync([], input);

        Assert.AreEqual(ExitCodes.Success, result.ExitCode);
        StringAssert.Contains(result.StandardOutput, "\nNorth\n", StringComparison.Ordinal);
        StringAssert.Contains(result.StandardOutput, "\nCentral\n", StringComparison.Ordinal);
        StringAssert.Contains(result.StandardOutput, "\nSouth\n", StringComparison.Ordinal);
        StringAssert.Contains(result.StandardOutput, "Generator: SplitMix64-v1", StringComparison.Ordinal);
    }

    [TestMethod]
    public async Task InvalidAndDuplicateOwnerNamesRepromptOnlyCurrentOwner()
    {
        string input = CliTestHarness.Lines(
            [
                string.Empty,
                string.Empty,
                "Alex",
                "alex",
                "Blake",
                "Casey",
                "Devon",
                "Emery",
                "Finley",
                "Gray",
                "Harper",
                "Indigo",
                "Jordan",
                string.Empty
            ]);
        var randomFactory = new RecordingRandomSourceFactory();

        CliRunResult result = await CliTestHarness.RunAsync(
            [],
            input,
            randomFactory);

        Assert.AreEqual(ExitCodes.Success, result.ExitCode);
        Assert.AreEqual(
            2,
            CountOccurrences(result.StandardOutput, "Owner name rejected:"));
        Assert.AreEqual(1, randomFactory.SystemCreateCount);
    }

    [TestMethod]
    public async Task InvalidCountsAndDuplicateConferenceRepromptAffectedFields()
    {
        string input = CliTestHarness.Lines(
            [
                "maybe",
                "n",
                "1",
                "2",
                "3",
                "4",
                "East",
                "east",
                "West",
                "Alex",
                "Blake",
                "Casey",
                "Devon",
                "2"
            ]);

        CliRunResult result = await CliTestHarness.RunAsync([], input);

        Assert.AreEqual(ExitCodes.Success, result.ExitCode);
        StringAssert.Contains(result.StandardOutput, "Please answer yes or no", StringComparison.Ordinal);
        StringAssert.Contains(result.StandardOutput, "whole number of 2 or greater", StringComparison.Ordinal);
        StringAssert.Contains(result.StandardOutput, "exactly divisible", StringComparison.Ordinal);
        StringAssert.Contains(result.StandardOutput, "Conference name rejected:", StringComparison.Ordinal);
    }

    [TestMethod]
    public async Task InvalidInteractiveSeedReprompts()
    {
        string input = CliTestHarness.Lines(
            [string.Empty, .. TestData.DefaultOwnerNames, "-1", "20200830"]);

        CliRunResult result = await CliTestHarness.RunAsync([], input);

        Assert.AreEqual(ExitCodes.Success, result.ExitCode);
        StringAssert.Contains(result.StandardOutput, "Seed must be an unsigned integer", StringComparison.Ordinal);
        StringAssert.Contains(result.StandardOutput, "Seed: 20200830", StringComparison.Ordinal);
    }

    [TestMethod]
    public async Task EofAtEveryDefaultPromptBoundaryReturnsInputEnded()
    {
        string[] lines = [string.Empty, .. TestData.DefaultOwnerNames, string.Empty];

        for (int suppliedLineCount = 0; suppliedLineCount < lines.Length; suppliedLineCount++)
        {
            string input = CliTestHarness.Lines(lines.Take(suppliedLineCount));
            CliRunResult result = await CliTestHarness.RunAsync([], input);

            Assert.AreEqual(
                ExitCodes.InputEndedOrCancelled,
                result.ExitCode,
                $"Boundary {suppliedLineCount}");
            StringAssert.Contains(
                result.StandardError,
                "Input ended",
                StringComparison.Ordinal);
        }
    }

    [TestMethod]
    public async Task EofAtEveryCustomPromptBoundaryReturnsInputEnded()
    {
        string[] lines =
        [
            "n", "3", "6",
            "North", "Central", "South",
            "Alex", "Blake", "Casey", "Devon", "Emery", "Finley",
            string.Empty
        ];

        for (int suppliedLineCount = 0; suppliedLineCount < lines.Length; suppliedLineCount++)
        {
            string input = CliTestHarness.Lines(lines.Take(suppliedLineCount));
            CliRunResult result = await CliTestHarness.RunAsync([], input);

            Assert.AreEqual(
                ExitCodes.InputEndedOrCancelled,
                result.ExitCode,
                $"Boundary {suppliedLineCount}");
        }
    }

    [TestMethod]
    public async Task CancellationMapsToDocumentedExitCode()
    {
        using var cancellationSource = new CancellationTokenSource();
        await cancellationSource.CancelAsync();

        CliRunResult result = await CliTestHarness.RunAsync(
            [],
            cancellationToken: cancellationSource.Token);

        Assert.AreEqual(ExitCodes.InputEndedOrCancelled, result.ExitCode);
        StringAssert.Contains(result.StandardError, "cancelled", StringComparison.Ordinal);
        Assert.DoesNotContain("Exception", result.StandardError, StringComparison.Ordinal);
    }

    private static int CountOccurrences(string text, string value)
    {
        int count = 0;
        int start = 0;
        while ((start = text.IndexOf(value, start, StringComparison.Ordinal)) >= 0)
        {
            count++;
            start += value.Length;
        }

        return count;
    }
}
