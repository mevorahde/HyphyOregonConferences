namespace HyphyOregon.ConferenceGenerator.Cli;

public sealed class InteractiveInputEndedException : Exception
{
    public InteractiveInputEndedException()
        : base("Input ended before the conference draw was complete.")
    {
    }
}
