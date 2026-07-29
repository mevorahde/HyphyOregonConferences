using HyphyOregon.ConferenceGenerator.Core;

namespace HyphyOregon.ConferenceGenerator.Cli;

public sealed class DrawPresenter : IDrawPresenter
{
    public void Write(ConferenceDraw draw, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(draw);
        ArgumentNullException.ThrowIfNull(output);

        output.WriteLine("Hyphy Oregon Conference Draw");
        output.WriteLine();

        IEnumerable<IGrouping<Conference, OwnerAssignment>> conferenceGroups =
            draw.Result.Assignments.GroupBy(assignment => assignment.Conference);
        foreach (IGrouping<Conference, OwnerAssignment> group in conferenceGroups)
        {
            output.WriteLine(group.Key.Name);
            foreach (OwnerAssignment assignment in group)
            {
                output.WriteLine($"  - {assignment.Owner.Name}");
            }

            output.WriteLine();
        }

        if (draw.Seed.HasValue)
        {
            output.WriteLine($"Seed: {draw.Seed.Value}");
            output.WriteLine("Generator: SplitMix64-v1");
        }
    }
}
