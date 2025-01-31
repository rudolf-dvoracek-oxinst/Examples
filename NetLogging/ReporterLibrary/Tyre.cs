namespace ReporterLibrary;

public sealed class Tyre
{
    private static readonly Random random = new Random();

    public float Inflation => Math.Abs(random.NextSingle());
}