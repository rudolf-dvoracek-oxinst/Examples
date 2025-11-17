namespace SilaXmlProofOfConceptServer.Services;

using Sila2.Examples.Greeting.V1;

public interface IGreetingBroadcaster
{
    void Broadcast(GreetingResult result);
}