namespace HyphyOregon.ConferenceGenerator.Core;

public sealed record Owner
{
    public Owner(string? name)
    {
        Name = NameRules.NormalizeOwnerName(name);
    }

    public string Name { get; }

    public override string ToString() => Name;
}
