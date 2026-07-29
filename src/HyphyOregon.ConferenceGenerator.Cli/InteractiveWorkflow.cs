using System.Globalization;
using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Cli;

public sealed class InteractiveWorkflow : IInteractiveWorkflow
{
    public async ValueTask<DrawRequest> PromptAsync(
        TextReader input,
        TextWriter output,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);

        output.WriteLine(CliMetadata.ProductName);
        output.WriteLine(
            "The default draw assigns 10 owners evenly between the East and West conferences.");

        bool useDefault = await PromptForDefaultChoiceAsync(
            input,
            output,
            cancellationToken).ConfigureAwait(false);

        int ownerCount;
        IReadOnlyList<string> conferenceNames;
        if (useDefault)
        {
            ownerCount = AssignmentRequest.DefaultOwnerCount;
            conferenceNames = ["East", "West"];
        }
        else
        {
            int conferenceCount = await PromptForConferenceCountAsync(
                input,
                output,
                cancellationToken).ConfigureAwait(false);
            ownerCount = await PromptForOwnerCountAsync(
                input,
                output,
                conferenceCount,
                cancellationToken).ConfigureAwait(false);
            conferenceNames = await PromptForConferenceNamesAsync(
                input,
                output,
                conferenceCount,
                cancellationToken).ConfigureAwait(false);
        }

        IReadOnlyList<string> ownerNames = await PromptForOwnerNamesAsync(
            input,
            output,
            ownerCount,
            cancellationToken).ConfigureAwait(false);
        ulong? seed = await PromptForSeedAsync(
            input,
            output,
            cancellationToken).ConfigureAwait(false);

        return new DrawRequest(ownerNames, conferenceNames, seed);
    }

    private static async ValueTask<bool> PromptForDefaultChoiceAsync(
        TextReader input,
        TextWriter output,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            output.Write("Use the default setup? [Y/n]: ");
            string choice = (await ReadLineAsync(input, cancellationToken).ConfigureAwait(false))
                .Trim();
            if (choice.Length == 0
                || choice.Equals("y", StringComparison.OrdinalIgnoreCase)
                || choice.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (choice.Equals("n", StringComparison.OrdinalIgnoreCase)
                || choice.Equals("no", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            output.WriteLine("Please answer yes or no. Press Enter to accept the default.");
        }
    }

    private static async ValueTask<int> PromptForConferenceCountAsync(
        TextReader input,
        TextWriter output,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            output.Write("Conference count (at least 2): ");
            string value = (await ReadLineAsync(input, cancellationToken).ConfigureAwait(false))
                .Trim();
            if (int.TryParse(
                    value,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out int count)
                && count >= 2)
            {
                return count;
            }

            output.WriteLine("Conference count must be a whole number of 2 or greater.");
        }
    }

    private static async ValueTask<int> PromptForOwnerCountAsync(
        TextReader input,
        TextWriter output,
        int conferenceCount,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            output.Write($"Owner count (a positive multiple of {conferenceCount}): ");
            string value = (await ReadLineAsync(input, cancellationToken).ConfigureAwait(false))
                .Trim();
            if (int.TryParse(
                    value,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out int count)
                && count >= conferenceCount
                && count % conferenceCount == 0)
            {
                return count;
            }

            output.WriteLine(
                $"Owner count must provide at least one owner per conference and be exactly divisible by {conferenceCount}.");
        }
    }

    private static async ValueTask<IReadOnlyList<string>> PromptForConferenceNamesAsync(
        TextReader input,
        TextWriter output,
        int count,
        CancellationToken cancellationToken)
    {
        var conferences = new List<Conference>(count);
        for (int index = 0; index < count;)
        {
            output.Write($"Conference {index + 1} name: ");
            string value = await ReadLineAsync(input, cancellationToken).ConfigureAwait(false);
            try
            {
                var candidate = new Conference(value);
                AssignmentValidation.EnsureUniqueConferences(conferences.Append(candidate));
                conferences.Add(candidate);
                index++;
            }
            catch (DomainValidationException exception)
            {
                output.WriteLine($"Conference name rejected: {exception.Message}");
            }
        }

        return conferences.Select(conference => conference.Name).ToArray();
    }

    private static async ValueTask<IReadOnlyList<string>> PromptForOwnerNamesAsync(
        TextReader input,
        TextWriter output,
        int count,
        CancellationToken cancellationToken)
    {
        var owners = new List<Owner>(count);
        for (int index = 0; index < count;)
        {
            output.Write($"Owner {index + 1} name: ");
            string value = await ReadLineAsync(input, cancellationToken).ConfigureAwait(false);
            try
            {
                var candidate = new Owner(value);
                AssignmentValidation.EnsureUniqueOwners(owners.Append(candidate));
                owners.Add(candidate);
                index++;
            }
            catch (DomainValidationException exception)
            {
                output.WriteLine($"Owner name rejected: {exception.Message}");
            }
        }

        return owners.Select(owner => owner.Name).ToArray();
    }

    private static async ValueTask<ulong?> PromptForSeedAsync(
        TextReader input,
        TextWriter output,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            output.Write("Seed (optional; press Enter for system randomization): ");
            string value = (await ReadLineAsync(input, cancellationToken).ConfigureAwait(false))
                .Trim();
            if (value.Length == 0)
            {
                return null;
            }

            if (ulong.TryParse(
                    value,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out ulong seed))
            {
                return seed;
            }

            output.WriteLine(
                "Seed must be an unsigned integer from 0 through 18446744073709551615, or blank.");
        }
    }

    private static async ValueTask<string> ReadLineAsync(
        TextReader input,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string? value = await input.ReadLineAsync(cancellationToken).ConfigureAwait(false);
        return value ?? throw new InteractiveInputEndedException();
    }
}
