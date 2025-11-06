namespace SilaProofOfConceptServer.GrpcServices;

using System.Threading.Tasks;
using global::HelloWorldService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service is inheriting code generated from the *.proto definition
/// </summary>
public sealed class HelloWorldService : HelloWorld.HelloWorldBase
{
    /// <summary>
    /// Logger for the HelloWorldService
    /// </summary>
    private readonly ILogger<HelloWorldService> _logger;

    public HelloWorldService(ILogger<HelloWorldService> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// SayHello method is called by the client
    /// </summary>
    /// <param name="request"> The request object containing the name of the person to greet </param>
    /// <param name="context"> The server call context, which contains information about the call </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the reply object <see cref="HelloReply"/> with the greeting message.
    /// </returns>
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        string message = $"Hello {request.Name}";
        var reply = new HelloReply(message);
        return Task.FromResult(reply);
    }

    /// <summary>
    /// JustSayHello method is called by the client
    /// </summary>
    /// <param name="request"> The request object containing the name of the person to greet </param>
    /// <param name="context"> The server call context, which contains information about the call </param>
    /// <returns> A task that represents the asynchronous operation. The task result contains an empty reply object <see cref="Empty"/>. </returns>
    public override Task<Empty> JustSayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }
}