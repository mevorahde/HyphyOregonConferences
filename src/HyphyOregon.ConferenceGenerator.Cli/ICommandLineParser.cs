namespace HyphyOregon.ConferenceGenerator.Cli;

public interface ICommandLineParser
{
    public CliInvocation Parse(IEnumerable<string>? arguments);
}
