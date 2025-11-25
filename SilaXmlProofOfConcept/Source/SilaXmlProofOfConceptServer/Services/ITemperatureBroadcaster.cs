namespace SilaXmlProofOfConceptServer.Services;

using System;

public interface ITemperatureBroadcaster
{
    void Broadcast(TemperatureSample sample);
    IDisposable Subscribe(Action<TemperatureSample> handler);
}    