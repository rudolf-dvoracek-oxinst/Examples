namespace SilaXmlProofOfConceptServer.Services;

/// Simple DTO for broadcasting temperature updates.
public sealed record TemperatureSample(double ValueCelsius, long TimestampUnixMs);