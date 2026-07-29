namespace HyphyOregon.ConferenceGenerator.Cli;

public interface IInteractiveWorkflow
{
    public ValueTask<DrawRequest> PromptAsync(
        TextReader input,
        TextWriter output,
        CancellationToken cancellationToken);
}
