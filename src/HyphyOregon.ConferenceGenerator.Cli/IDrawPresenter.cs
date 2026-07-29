namespace HyphyOregon.ConferenceGenerator.Cli;

public interface IDrawPresenter
{
    public void Write(ConferenceDraw draw, TextWriter output);
}
