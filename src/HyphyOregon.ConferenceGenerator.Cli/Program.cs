namespace HyphyOregon.ConferenceGenerator.Cli;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        using var cancellationSource = new CancellationTokenSource();
        ConsoleCancelEventHandler cancellationHandler = (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellationSource.Cancel();
        };

        Console.CancelKeyPress += cancellationHandler;
        try
        {
            CliHost host = CompositionRoot.CreateHost();
            return await host.RunAsync(
                args,
                Console.In,
                Console.Out,
                Console.Error,
                cancellationSource.Token).ConfigureAwait(false);
        }
        finally
        {
            Console.CancelKeyPress -= cancellationHandler;
        }
    }
}
