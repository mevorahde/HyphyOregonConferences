using System.Text;

namespace HyphyOregon.ConferenceGenerator.Core;

internal static class NameRules
{
    internal static string NormalizeOwnerName(string? value) =>
        Normalize(value, "Owner", DomainValidationError.InvalidOwnerName);

    internal static string NormalizeConferenceName(string? value) =>
        Normalize(value, "Conference", DomainValidationError.InvalidConferenceName);

    private static string Normalize(
        string? value,
        string kind,
        DomainValidationError error)
    {
        if (value is null)
        {
            throw new DomainValidationException(error, $"{kind} name cannot be null.");
        }

        string trimmed = value.Trim();
        if (trimmed.Length == 0)
        {
            throw new DomainValidationException(
                error,
                $"{kind} name cannot be empty or whitespace.");
        }

        if (trimmed.Any(char.IsControl))
        {
            throw new DomainValidationException(
                error,
                $"{kind} name cannot contain control characters or line breaks.");
        }

        try
        {
            return trimmed.Normalize(NormalizationForm.FormC);
        }
        catch (ArgumentException exception)
        {
            throw new DomainValidationException(
                error,
                $"{kind} name must contain valid Unicode text.",
                exception);
        }
    }
}
