namespace HyphyOregon.ConferenceGenerator.Cli;

public sealed class ConferenceGeneratorApplication : IConferenceGeneratorApplication
{
    private readonly IInteractiveWorkflow _interactiveWorkflow;
    private readonly ConferenceDrawService _drawService;
    private readonly IDrawPresenter _presenter;

    public ConferenceGeneratorApplication(
        IInteractiveWorkflow interactiveWorkflow,
        ConferenceDrawService drawService,
        IDrawPresenter presenter)
    {
        _interactiveWorkflow =
            interactiveWorkflow ?? throw new ArgumentNullException(nameof(interactiveWorkflow));
        _drawService = drawService ?? throw new ArgumentNullException(nameof(drawService));
        _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
    }

    public async ValueTask ExecuteAsync(
        CliInvocation invocation,
        TextReader input,
        TextWriter output,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(invocation);
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);

        cancellationToken.ThrowIfCancellationRequested();

        switch (invocation.Command)
        {
            case CliCommandKind.Help:
                output.WriteLine(CliMetadata.HelpText);
                return;

            case CliCommandKind.Version:
                output.WriteLine($"{CliMetadata.ProductName} {CliMetadata.Version}");
                return;

            case CliCommandKind.Interactive:
                DrawRequest interactiveRequest = await _interactiveWorkflow.PromptAsync(
                    input,
                    output,
                    cancellationToken).ConfigureAwait(false);
                _presenter.Write(_drawService.Generate(interactiveRequest), output);
                return;

            case CliCommandKind.Draw:
                var commandLineRequest = new DrawRequest(
                    invocation.OwnerNames,
                    invocation.ConferenceNames,
                    invocation.Seed);
                _presenter.Write(_drawService.Generate(commandLineRequest), output);
                return;

            default:
                throw new InvalidOperationException("Unsupported CLI command.");
        }
    }
}
