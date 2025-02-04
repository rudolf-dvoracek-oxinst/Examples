namespace ReporterLibrary;

public sealed class Car
{
    private byte[] signature = new byte[32];
    private static readonly Random random = new Random();

    private Tyre[] tyres =
    [
        new(),
        new(),
        new(),
        new()
    ];

    /// <summary>
    /// Fictional byte property to test structured logging with binary data
    /// </summary>
    public byte[] Signature
    {
        get
        {
            random.NextBytes(this.signature);
            return this.signature;
        }
    }

    public IEnumerable<Tyre> Tyres => this.tyres;
}