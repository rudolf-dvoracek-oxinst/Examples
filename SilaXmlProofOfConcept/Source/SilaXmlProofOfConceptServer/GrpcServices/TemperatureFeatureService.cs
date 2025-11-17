namespace SilaXmlProofOfConceptServer.GrpcServices;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Sila2.Examples.Temperature.V1;
using SilaXmlProofOfConceptServer.Services;

public sealed class TemperatureFeatureService : TemperatureFeature.TemperatureFeatureBase, ITemperatureBroadcaster
{
    private readonly ILogger<TemperatureFeatureService> _logger;
    private readonly ConcurrentDictionary<string, TempExecutionInfo> _executions = new();
    private readonly List<IServerStreamWriter<TemperatureReading>> _subscribers = new();

    // Current temperature maintained from broadcast updates
    private TemperatureReading? _latestReading;

    public TemperatureFeatureService(ILogger<TemperatureFeatureService> logger)
    {
        this._logger = logger;
    }

    public override Task<TemperatureFeatureDefinition> GetFeatureDefinition(Empty request, ServerCallContext context)
    {
        var def = new TemperatureFeatureDefinition
        {
            FeatureName = "TemperatureFeature"
        };
        def.Commands.Add("MeasureTemperature");
        def.Properties.Add("CurrentTemperature");
        def.Events.Add("TemperatureUpdated");
        return Task.FromResult(def);
    }

    public override Task<TempExecutionInfo> MeasureTemperature(Empty request, ServerCallContext context)
    {
        // Use latest reading if available, else synthesize an empty one.
        var reading = this._latestReading ?? new TemperatureReading
        {
            ValueCelsius = 0,
            TimestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        var execUuid = Guid.NewGuid().ToString("N");
        var exec = new TempExecutionInfo
        {
            ExecutionUuid = execUuid,
            State = TempExecutionState.Succeeded,
            Reading = reading
        };

        this._executions[execUuid] = exec;
        return Task.FromResult(exec);
    }

    public override Task<TempExecutionInfo> GetExecutionInfo(TempExecutionReference request, ServerCallContext context)
    {
        if (this._executions.TryGetValue(request.ExecutionUuid, out var info))
            return Task.FromResult(info);

        return Task.FromResult(new TempExecutionInfo
        {
            ExecutionUuid = request.ExecutionUuid,
            State = TempExecutionState.Failed,
            Failure = new TempExecutionFailure
            {
                Code = 404,
                Reason = "NotFound",
                Detail = "Execution not found."
            }
        });
    }

    public override Task<TemperatureReading> GetCurrentTemperature(Empty request, ServerCallContext context)
    {
        return Task.FromResult(this._latestReading ?? new TemperatureReading
        {
            ValueCelsius = 0,
            TimestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    public override async Task SubscribeTemperatureUpdated(Empty request, IServerStreamWriter<TemperatureReading> responseStream, ServerCallContext context)
    {
        lock (this._subscribers)
        {
            this._subscribers.Add(responseStream);
        }

        try
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, context.CancellationToken);
            }
        }
        catch (TaskCanceledException) { }
        finally
        {
            lock (this._subscribers)
            {
                this._subscribers.Remove(responseStream);
            }
        }
    }

    // Called by TemperatureBackgroundService via ITemperatureBroadcaster
    public void Broadcast(TemperatureReading reading)
    {
        this._latestReading = reading;

        lock (this._subscribers)
        {
            foreach (var sub in this._subscribers.ToArray())
            {
                _ = sub.WriteAsync(reading);
            }
        }
    }
}