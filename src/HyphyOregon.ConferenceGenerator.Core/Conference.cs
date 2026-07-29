namespace HyphyOregon.ConferenceGenerator.Core;

public sealed record Conference
{
    public Conference(string? name)
    {
        Name = NameRules.NormalizeConferenceName(name);
    }

    public string Name { get; }

    public override string ToString() => Name;
}
