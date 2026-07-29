namespace HyphyOregon.ConferenceGenerator.Core;

public sealed class DomainValidationException : Exception
{
    public DomainValidationException()
        : this(DomainValidationError.Unknown, "The conference assignment request is invalid.")
    {
    }

    public DomainValidationException(string message)
        : this(DomainValidationError.Unknown, message)
    {
    }

    public DomainValidationException(string message, Exception innerException)
        : this(DomainValidationError.Unknown, message, innerException)
    {
    }

    public DomainValidationException(DomainValidationError error, string message)
        : base(message)
    {
        Error = error;
    }

    public DomainValidationException(
        DomainValidationError error,
        string message,
        Exception innerException)
        : base(message, innerException)
    {
        Error = error;
    }

    public DomainValidationError Error { get; }
}
