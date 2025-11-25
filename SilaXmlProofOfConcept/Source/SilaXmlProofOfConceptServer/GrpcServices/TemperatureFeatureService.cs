namespace SilaXmlProofOfConceptServer.GrpcServices;

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Sila2.Oxfordinstruments.Examples.Temperaturefeature.V1;
using SilaXmlProofOfConceptServer.Services;
using Sila2.Org.Silastandard;
using Sila2.Oxfordinstruments.Examples.Temperaturefeature.V1;

public sealed class TemperatureFeatureService : TemperatureFeature.TemperatureFeatureBase
{
    private readonly ILogger<TemperatureFeatureService> logger;
    private readonly ITemperatureState state;
    private readonly ITemperatureBroadcaster broadcaster;

    public TemperatureFeatureService(
        ILogger<TemperatureFeatureService> logger,
        ITemperatureState state,
        ITemperatureBroadcaster broadcaster)
    {
        this.logger = logger;
        this.state = state;
        this.broadcaster = broadcaster;
    }

    // Proto: rpc MeasureTemperature (...) returns MeasureTemperature_Responses
    // The generated message MeasureTemperature_Responses is empty, so we just log.
    public override Task<MeasureTemperature_Responses> MeasureTemperature(
        MeasureTemperature_Parameters request,
        ServerCallContext context)
    {
        this.logger.LogInformation("MeasureTemperature invoked. Current temp = {Temp:F3} °C", this.state.CurrentTemperature);
        // Optionally you could trigger an immediate sample broadcast here if desired.
        return Task.FromResult(new MeasureTemperature_Responses());
    }

    // Proto: rpc Subscribe_CurrentTemperature (...) returns (stream Subscribe_CurrentTemperature_Responses)
    // Stream periodic or event-driven updates of the observable property.
    public override async Task Subscribe_CurrentTemperature(
        Subscribe_CurrentTemperature_Parameters request,
        IServerStreamWriter<Subscribe_CurrentTemperature_Responses> responseStream,
        ServerCallContext context)
    {
        this.logger.LogInformation("Client subscribed to CurrentTemperature.");

        // Use broadcaster for push updates.
        using var subscription = this.broadcaster.Subscribe(async sample =>
        {
            try
            {
                var msg = new Subscribe_CurrentTemperature_Responses
                {
                    CurrentTemperature = new Subscribe_CurrentTemperature_Responses.Types.CurrentTemperature_Struct
                    {
                        ValueCelsius = new Real { Value = sample.ValueCelsius },
                        TimestampUnixMs = new Integer { Value = sample.TimestampUnixMs }
                    }
                };
                await responseStream.WriteAsync(msg);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                this.logger.LogError(ex, "Error writing CurrentTemperature sample.");
            }
        });

        // Keep method alive until client cancels.
        try
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal cancellation.
        }

        this.logger.LogInformation("Client unsubscribed from CurrentTemperature.");
    }
}