using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Cli;

public sealed class CliHost
{
    private readonly ICommandLineParser _parser;
    private readonly IConferenceGeneratorApplication _application;
    private readonly Action<Exception>? _unexpectedExceptionObserver;

    public CliHost(
        ICommandLineParser parser,
        IConferenceGeneratorApplication application,
        Action<Exception>? unexpectedExceptionObserver = null)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _application = application ?? throw new ArgumentNullException(nameof(application));
        _unexpectedExceptionObserver = unexpectedExceptionObserver;
    }

    public async ValueTask<int> RunAsync(
        IEnumerable<string> arguments,
        TextReader input,
        TextWriter output,
        TextWriter error,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(error);

        try
        {
            CliInvocation invocation = _parser.Parse(arguments);
            await _application.ExecuteAsync(
                invocation,
                input,
                output,
                cancellationToken).ConfigureAwait(false);
            return ExitCodes.Success;
        }
        catch (CommandLineUsageException exception)
        {
            error.WriteLine($"Error: {exception.Message}");
            error.WriteLine("Use --help for usage.");
            return ExitCodes.UsageOrValidationError;
        }
        catch (DomainValidationException exception)
        {
            error.WriteLine($"Error: {exception.Message}");
            return ExitCodes.UsageOrValidationError;
        }
        catch (InteractiveInputEndedException exception)
        {
            error.WriteLine(exception.Message);
            return ExitCodes.InputEndedOrCancelled;
        }
        catch (OperationCanceledException)
        {
            error.WriteLine("The operation was cancelled.");
            return ExitCodes.InputEndedOrCancelled;
        }
        catch (Exception exception)
        {
            _unexpectedExceptionObserver?.Invoke(exception);
            error.WriteLine("An unexpected internal error occurred.");
            return ExitCodes.InternalError;
        }
    }
}
