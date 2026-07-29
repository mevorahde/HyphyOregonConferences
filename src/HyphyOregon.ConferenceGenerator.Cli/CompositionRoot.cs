using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Cli;

public static class CompositionRoot
{
    public static ConferenceAssigner CreateProductionAssigner() =>
        new(SystemBoundedRandomSource.Shared);

    public static ConferenceAssigner CreateSeededAssigner(ulong seed) =>
        new(new StableSeededRandomSource(seed));

    public static CliHost CreateHost(Action<Exception>? unexpectedExceptionObserver = null)
    {
        var randomSourceFactory = new RandomSourceFactory();
        var application = new ConferenceGeneratorApplication(
            new InteractiveWorkflow(),
            new ConferenceDrawService(randomSourceFactory),
            new DrawPresenter());
        return new CliHost(
            new CommandLineParser(),
            application,
            unexpectedExceptionObserver);
    }
}
