namespace HyphyOregon.ConferenceGenerator.Cli;

public sealed class CommandLineUsageException : Exception
{
    public CommandLineUsageException()
        : this("The command line is invalid.")
    {
    }

    public CommandLineUsageException(string message)
        : base(message)
    {
    }

    public CommandLineUsageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
