namespace ReporterLibrary;

public sealed class Car
{
    private Tyre[] tyres =
    [
        new(),
        new(),
        new(),
        new()
    ];

    public IEnumerable<Tyre> Tyres => this.tyres;
}