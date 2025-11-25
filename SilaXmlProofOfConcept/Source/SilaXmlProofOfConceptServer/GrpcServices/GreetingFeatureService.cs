namespace SilaXmlProofOfConceptServer.GrpcServices;

using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Sila2.Oxfordinstruments.Examples.Greetingfeature.V1;

public sealed class GreetingFeatureService : GreetingFeature.GreetingFeatureBase
{
    private readonly ILogger<GreetingFeatureService> _logger;

    public GreetingFeatureService(ILogger<GreetingFeatureService> logger) => this._logger = logger;

    // Definition: SayHello returns an empty SayHello_Responses (no result fields in proto).
    public override Task<SayHello_Responses> SayHello(SayHello_Parameters request, ServerCallContext context)
    {
        var name = request.Name?.Value ?? string.Empty;
        this._logger.LogInformation("Received SayHello for '{Name}'", name);
        // Nothing to populate – response is empty by definition.
        return Task.FromResult(new SayHello_Responses());
    }

    // Definition: Subscribe_LastGreeting streams Subscribe_LastGreeting_Responses containing an empty LastGreeting_Struct.
    public override async Task Subscribe_LastGreeting(
        Subscribe_LastGreeting_Parameters request,
        IServerStreamWriter<Subscribe_LastGreeting_Responses> responseStream,
        ServerCallContext context)
    {
        this._logger.LogInformation("Client subscribed to LastGreeting stream.");

        try
        {
            // Emit a placeholder empty structure periodically since proto defines no fields.
            while (!context.CancellationToken.IsCancellationRequested)
            {
                var payload = new Subscribe_LastGreeting_Responses
                {
                    LastGreeting = new Subscribe_LastGreeting_Responses.Types.LastGreeting_Struct()
                };

                await responseStream.WriteAsync(payload);
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
        }
        catch (TaskCanceledException)
        {
            // Normal cancellation on client disconnect.
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error during Subscribe_LastGreeting streaming.");
        }
    }
}