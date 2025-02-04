using CodeFirstContracts;

namespace CodeFirstService.Services;

public sealed class GreeterService : IGreeterService
{
    public ValueTask<HelloReply> SayHello(HelloRequest helloRequest)
    {
        HelloReply reply =  new HelloReply();
        reply.Message = "Hello " + helloRequest.Name;
        if (helloRequest.Number > 1)
        {
            reply.Message += "s";
        }
        return ValueTask.FromResult(reply);
    }

    public ValueTask JustSayHello(HelloRequest helloRequest)
    {
        return ValueTask.CompletedTask;
    }
}