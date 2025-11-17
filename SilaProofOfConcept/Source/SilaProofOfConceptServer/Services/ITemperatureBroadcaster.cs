namespace SilaProofOfConceptServer.Services;

using Sila2.Examples.Temperature.V1;

public interface ITemperatureBroadcaster
{
    void Broadcast(TemperatureReading reading);
}