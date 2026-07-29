namespace HyphyOregon.ConferenceGenerator.Cli;

public interface IConferenceGeneratorApplication
{
    public ValueTask ExecuteAsync(
        CliInvocation invocation,
        TextReader input,
        TextWriter output,
        CancellationToken cancellationToken);
}
