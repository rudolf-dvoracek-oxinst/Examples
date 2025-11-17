namespace SilaProofOfConceptServer.GrpcServices;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Sila2.Examples.Greeting.V1;
using SilaProofOfConceptServer.Services;

public sealed class GreetingFeatureService : GreetingFeature.GreetingFeatureBase, IGreetingBroadcaster
{
    private readonly ILogger<GreetingFeatureService> _logger;
    private readonly ConcurrentDictionary<string, ExecutionInfo> _executions = new();
    private GreetingResult? _lastGreeting;
    private readonly List<IServerStreamWriter<GreetingResult>> _subscribers = new();

    public GreetingFeatureService(ILogger<GreetingFeatureService> logger) => _logger = logger;

    public override Task<FeatureDefinition> GetFeatureDefinition(Empty request, ServerCallContext context)
    {
        var def = new FeatureDefinition
        {
            FeatureName = "GreetingFeature"
        };
        def.Commands.Add("SayHello");
        def.Properties.Add("LastGreeting");
        def.Events.Add("GreetingEmitted");
        return Task.FromResult(def);
    }

    public override Task<ExecutionInfo> SayHello(SayHelloParameters request, ServerCallContext context)
    {
        var greeting = new GreetingResult
        {
            Message = $"Hello {request.Name}",
            TimestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        _lastGreeting = greeting;

        var execUuid = Guid.NewGuid().ToString("N");
        var exec = new ExecutionInfo
        {
            ExecutionUuid = execUuid,
            State = ExecutionState.Succeeded,
            Result = greeting
        };
        _executions[execUuid] = exec;

        Broadcast(greeting);

        return Task.FromResult(exec);
    }

    public override Task<ExecutionInfo> GetExecutionInfo(ExecutionReference request, ServerCallContext context)
    {
        if (_executions.TryGetValue(request.ExecutionUuid, out var exec))
            return Task.FromResult(exec);

        return Task.FromResult(new ExecutionInfo
        {
            ExecutionUuid = request.ExecutionUuid,
            State = ExecutionState.Failed,
            Failure = new ExecutionFailure
            {
                Code = 404,
                Reason = "NotFound",
                Detail = "Execution not found."
            }
        });
    }

    public override Task<GreetingResult> GetLastGreeting(Empty request, ServerCallContext context)
        => Task.FromResult(_lastGreeting ?? new GreetingResult { Message = string.Empty, TimestampUnixMs = 0 });

    public override async Task SubscribeGreetingEmitted(Empty request, IServerStreamWriter<GreetingResult> responseStream, ServerCallContext context)
    {
        lock (_subscribers)
        {
            _subscribers.Add(responseStream);
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
            lock (_subscribers)
            {
                _subscribers.Remove(responseStream);
            }
        }
    }

    public void Broadcast(GreetingResult result)
    {
        lock (_subscribers)
        {
            foreach (var sub in _subscribers.ToArray())
            {
                _ = sub.WriteAsync(result);
            }
        }
    }
}