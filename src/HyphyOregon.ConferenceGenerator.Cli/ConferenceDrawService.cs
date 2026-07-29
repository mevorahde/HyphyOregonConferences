using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Cli;

public sealed class ConferenceDrawService
{
    private readonly IRandomSourceFactory _randomSourceFactory;

    public ConferenceDrawService(IRandomSourceFactory randomSourceFactory)
    {
        _randomSourceFactory =
            randomSourceFactory ?? throw new ArgumentNullException(nameof(randomSourceFactory));
    }

    public ConferenceDraw Generate(DrawRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        Owner[] owners = request.OwnerNames.Select(name => new Owner(name)).ToArray();
        Conference[] conferences = request.ConferenceNames.Count == 0
            ? [new Conference("East"), new Conference("West")]
            : request.ConferenceNames.Select(name => new Conference(name)).ToArray();
        var assignmentRequest = new AssignmentRequest(owners, conferences);
        IBoundedRandomSource randomSource = request.Seed.HasValue
            ? _randomSourceFactory.CreateSeeded(request.Seed.Value)
            : _randomSourceFactory.CreateSystem();
        var assigner = new ConferenceAssigner(randomSource);

        return new ConferenceDraw(assigner.Assign(assignmentRequest), request.Seed);
    }
}
